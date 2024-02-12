using UnityEngine;

[RequireComponent(typeof(test))]
[RequireComponent(typeof(Grid))]
public class GridGroup : MonoBehaviour
{
    private test gridHolder;
    private Grid grid;

    void Awake() {
        // gets reference to test component (holds the BoolArray2D)
        grid = GetComponent<Grid>();
        gridHolder = GetComponent<test>();
    }

    // wrapper func, gets logical center coord of a grid cell (locally)
    public Vector3 GetCellCenterLocal(int x, int y) {
        if (!IsCellValid(x, y)) {
            Debug.LogWarning("Invalid cell coordinates.");
            return Vector3.zero;
        }

        Vector3 cellSize = grid.cellSize;
        Vector3 cellGap = grid.cellGap;

        // calcs the position of the center of the cell in local space
        Vector3 position = new Vector3(x * (cellSize.x + cellGap.x), 0, y * (cellSize.z + cellGap.z)) + (cellSize / 2f) + (cellGap / 2f);

        return position;
    }

    // wrapper func, gets logical center coord of a grid cell (in world space)
    public Vector3 GetCellCenterWorld(int x, int y) {
        Vector3 localCenter = GetCellCenterLocal(x, y);

        // transforms local center coord to world space
        Vector3 worldCenter = transform.TransformPoint(localCenter);

        return worldCenter;
    }

    // checks if a cell is valid
    private bool IsCellValid(int x, int y) {
        if (gridHolder == null || gridHolder.shape == null) {
            Debug.LogWarning("Grid or shape is not initialized.");
            return false;
        }

        return x >= 0 && x < gridHolder.shape.width && y >= 0 && y < gridHolder.shape.height;
    }

    void OnDrawGizmos() {
        if (gridHolder == null || gridHolder.shape == null || grid == null) return;

        BoolArray2D matrix = gridHolder.shape;
        Vector3 cellSize = grid.cellSize;
        Vector3 cellGap = grid.cellGap;

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
}
