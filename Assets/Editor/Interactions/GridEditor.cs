using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Interactions;
using NUnit;
using static Interactions.GridGroup;

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

        protected void addGridObject(ref List<GridGroup.Cell> children, Collider c, GridGroup group) {

            List<GridGroup.Cell> toAdd = new List<GridGroup.Cell>();
            var start = c.bounds.center - c.bounds.extents;
            var end = c.bounds.center + c.bounds.extents;

            for (float x = start.x; x < end.x; x += cellSize.vector3Value.x + cellSpacing.vector3Value.x) {
                for (float y = start.z; y < end.z; y += cellSize.vector3Value.z + cellSpacing.vector3Value.z) {
                    // TODO: Figure out multiple cells together to make one object.
                    var cell = group.WorldToCell(new Vector3(x, 0, y));
                    if (cell == null) {
                        Debug.LogWarning($"{c.name} does not fit in grid at {x},{y}");
                        return;
                    }
                    toAdd.Add((GridGroup.Cell)cell);
                }
            }

            if (c.TryGetComponent(out GridObject go)) {
                go.cells = toAdd.ToArray();
                go.manager = group;
            }

            children.AddRange(toAdd);
        }

        protected List<GridGroup.Cell> gridObjectsToAdd() {
            var group = (GridGroup)serializedObject.targetObject;
            List<GridGroup.Cell> children = new List<GridGroup.Cell>();
            for (int i = 0; i < group.transform.childCount; i++) {
                if (group.transform.GetChild(i).TryGetComponent(out Collider c)) {
                    addGridObject(ref children, c, group);
                }
            }
            return children;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            Vector2Int gridSize = gridDimensions.vector2IntValue;
            cells.arraySize = gridSize.y * gridSize.x;

            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    // TODO: Get game objects involved.
                    var cell = cells.GetArrayElementAtIndex(y * gridSize.x + x);
                    var typeValue = cell.FindPropertyRelative("type").enumValueIndex;
                    // Since we no longer clear the array (to store MAP_FULL enums),
                    // This is a way to avoid unnecessary info as we extend the cells:
                    if (typeValue == 1) {
                        cell.FindPropertyRelative("type").enumValueIndex = 0;
                    }

                    var pos = new Vector2Int(x, y);
                    cell.FindPropertyRelative("pos").vector2IntValue = pos;

                    cell.FindPropertyRelative("rotation").quaternionValue = Quaternion.identity;
                }
            }

            var children = gridObjectsToAdd();
            foreach (var child in children) {
                var cell = cells.GetArrayElementAtIndex((child.pos.y * gridSize.x) + child.pos.x);
                cell.FindPropertyRelative("type").enumValueIndex = 1;
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI() {
            var group = (GridGroup)target;
            foreach (var cell in group.cells) {
                var box = group.CellToWorld(cell);
                if (cell.type == Cell.CellType.FULL) {
                    Handles.color = Color.red;
                } else if (cell.pos == Vector2Int.zero) {
                    Handles.color = Color.blue;
                } else {
                    Handles.color = Color.yellow;
                    var smallest = Mathf.Min(box.scale.x, box.scale.y, box.scale.z);
                    if (Handles.Button(box.center, Quaternion.identity, smallest/4f, smallest/2f, Handles.DotHandleCap)) {
                        var cellToChange = cells.GetArrayElementAtIndex((cell.pos.y * gridDimensions.vector2IntValue.x) + cell.pos.x);

                        var type = cellToChange.FindPropertyRelative("type");
                        if (type.enumValueIndex == 2) {
                            type.enumValueIndex = 0;
                        } else {
                            type.enumValueIndex = 2;
                        }
                        serializedObject.ApplyModifiedProperties();
                    }

                    if (cell.type == Cell.CellType.MAP_FULL) {
                        Handles.color = Color.red;
                    } else {
                        Handles.color = Color.black;
                    }
                }

                // Handles.DrawWireCube does not support rotation:
                Handles.DrawLines(box.edges);
                //Handles.DrawWireCube(box.center, box.scale);
            }
        }

        
    }
}
