//learned how to do this w/ this:
//https://github.com/Eldoir/Array2DEditor/tree/master

using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    public BoolArray2D shape = null;

    [SerializeField]
    private GameObject prefabToInstantiate = null;
    void Start()
    {
        if (shape == null || prefabToInstantiate == null) {
            Debug.LogError("Fill in all the fields in order to start this example.");
            return;
        }
        GameObject piece = new GameObject("Piece");

        for (int y = 0; y < shape.height; y++) {
            for (int x = 0; x < shape.width; x++) {
                if (shape[x, y]) {
                    var matrixPrefab = Instantiate(prefabToInstantiate, new Vector3(x, 0, y), Quaternion.identity, piece.transform);
                    matrixPrefab.name = $"({x}, {y})";
                }
            }
        }
    }

}

