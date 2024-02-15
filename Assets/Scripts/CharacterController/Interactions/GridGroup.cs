using UnityEngine;
using Interactions.Behaviors;
using System.Collections;
using System.Collections.Generic;
using Interactions;
using Utility;
[ExecuteInEditMode]
[RequireComponent(typeof(test))]
public class GridGroup : MonoBehaviour {
    private test gridHolder;
    public Vector3 cellSize;
    public Vector3 cellGap;
    public Cell[] items;

    void Awake() {
        // gets reference to test component (holds the BoolArray2D)
        gridHolder = GetComponent<test>();
        items = new Cell[gridHolder.shape.width * gridHolder.shape.height];
        Debug.Log($"height: {gridHolder.shape.height}, width: {gridHolder.shape.width}");

        initItems();

    }
    // wrapper func, gets logical pos coord of a grid cell (locally)
    public Vector3 GetCellCenterLocal(int x, int y) {
        if (!IsCellValid(x, y)) {
            Debug.LogWarning("Invalid cell coordinates.");
            return Vector3.zero;
        }
        // calcs the position of the center of the cell in local space
        Vector3 position = new Vector3(x * (cellSize.x + cellGap.x), 0, y * (cellSize.z + cellGap.z)) + (cellSize / 2f) + (cellGap / 2f);
        return position;
    }

    public Cell cellInDirection(Cell cell, Vector2 direction){
        int X = cell.x + (int)direction.x;
        int Y = cell.y + (int)direction.y;
        Debug.Log($"X: {X}, Y: {Y}");
        if (IsCellValid(X, Y)) {
            return this[X,Y];
        }else{
            return null;
        }
    }
    public void moveObjFromAToB(Cell a, Cell b){
        if (a.obj == null){
            Debug.LogWarning("cell A does not contain an object to move.");
            return;
        }
        b.obj = a.obj;
        a.obj = null;
    }
    
    // wrapper func, gets logical center coord of a grid cell (in world space)
    public Vector3 GetCellCenterWorld(int x, int y) {
        Vector3 position = new Vector3(x * (cellSize.x + cellGap.x), 0, y * (cellSize.z + cellGap.z))
                                      + transform.position + (cellSize / 2f) + (cellGap / 2f); 
        return position;
    }

    // checks if a cell is valid
    private bool IsCellValid(int x, int y) {
        if (gridHolder == null || gridHolder.shape == null) {
            Debug.LogWarning("Grid or shape is not initialized.");
            return false;
        }
        if(x >= 0 && x < gridHolder.shape.width && y >= 0 && y < gridHolder.shape.height){
            return (gridHolder.shape[x,y] && (this[x,y].obj == null));
        }
        return false;
    }

    public void initItems(){
        for (int y = 0; y < gridHolder.shape.height; y++) {
            for (int x = 0; x < gridHolder.shape.width; x++) {
                Cell cell = new Cell(x,y,GetCellCenterWorld(x, y), this);
                //set cell.parent to a reference to this.
                Collider[] colliders = Physics.OverlapBox(cell.pos, cellSize * 0.5f, Quaternion.identity);
                foreach(Collider collider in colliders){
                    if (collider.TryGetComponent<Interaction>(out Interaction interaction)){
                        if(interaction != null && interaction.behavior is PushableInteraction pushableBehavior){
                            Vector3 cellMin = cell.pos - cellSize * 0.5f;
                            Vector3 cellMax = cell.pos + cellSize * 0.5f;
                            Vector3 objCenter = interaction.gameObject.transform.position;
                            
                            if (objCenter.x >= cellMin.x && objCenter.x <= cellMax.x && objCenter.z >= cellMin.z && objCenter.z <= cellMax.z) {
                                cell.obj = interaction.gameObject;
                                pushableBehavior.parentCell = cell;
                                cell.obj.transform.position = new Vector3(cell.pos.x, objCenter.y, cell.pos.z);
                            }
                        }
                    }else {
                        cell.obj = null; // No pushable in this cell
                    }
                }
                this[x,y] = cell;
            }
        }
    }
    void OnDrawGizmos() {
        if (gridHolder == null || gridHolder.shape == null) return;

        BoolArray2D matrix = gridHolder.shape;

        for (int y = 0; y < matrix.height; y++) {
            for (int x = 0; x < matrix.width; x++) {
                if (matrix[x, y]) { // this cell is "callable"
                    // calcs position to draw Gizmo @
                    Vector3 position = new Vector3(x * (cellSize.x + cellGap.x), 0, y * (cellSize.z + cellGap.z))
                                      + transform.position + (cellSize / 2f) + (cellGap / 2f); 

                    // draws wire cube @ the position
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(position, cellSize);
                }
            }
        }
    }


    public Cell this[int x, int y] {
        get { return items[y * gridHolder.shape.width + x]; }
        set { items[y * gridHolder.shape.width + x]= value; }
    }

}
public class Cell {
    public GameObject obj;
    public bool valid;
    public Vector3 pos; // World space center of the grid
    public int x;
    public int y;
    public GridGroup parent; // Reference to the parent GridGroup

    // Constructor to set initial values
    public Cell(int x, int y, Vector3 pos, GridGroup parent) {
        this.x = x;
        this.y = y;
        this.pos = pos;
        this.parent = parent;
    }
}
