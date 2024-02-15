using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interactions {
    [CustomEditor(typeof(GridGroup))]
    public class GridEditor : Editor {

        SerializedProperty cells;
        SerializedProperty cellSize;
        SerializedProperty cellSpacing;
        SerializedProperty gridDimensions;
        private void OnEnable() {
            cells = serializedObject.FindProperty("cells");
            cellSize = serializedObject.FindProperty("cellSize");
            cellSpacing = serializedObject.FindProperty("cellSpacing");
            gridDimensions = serializedObject.FindProperty("gridDimensions");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            Vector2Int gridSize = gridDimensions.vector2IntValue;
            cells.arraySize = gridSize.y * gridSize.x;

            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    // TODO: Get game objects involved.
                    var cell = cells.GetArrayElementAtIndex(y * gridSize.x + x);

                    var pos = new Vector2Int(x, y);
                    var scale = new Vector2Int(1, 1);
                    cell.FindPropertyRelative("pos").vector2IntValue = pos;
                    cell.FindPropertyRelative("scale").vector2IntValue = scale;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
