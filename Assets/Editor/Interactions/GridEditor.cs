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
        SerializedProperty gridDimensions;
        private void OnEnable() {
            cells = serializedObject.FindProperty("cells");
            gridDimensions = serializedObject.FindProperty("gridDimensions");
        }

        HashSet<Vector2Int> objectColors = new HashSet<Vector2Int>();
        HashSet<Vector2Int> errorCells = new HashSet<Vector2Int>();
        protected void addGridObject(ref List<GridGroup.Cell> children, BoxCollider c, GridGroup group) {
            List<GridGroup.Cell> toAdd = group.BoundsToCells(c);
            if (toAdd == null) {
                return;
            }

            foreach (var cell in toAdd) {
                if (objectColors.Contains(cell.pos)) {
                    Debug.LogError($"{c.name} shares a cell with another object at {cell.pos}", c.gameObject);
                    errorCells.Add(cell.pos);
                    return;
                } else {
                    objectColors.Add(cell.pos);
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
                if (group.transform.GetChild(i).TryGetComponent(out BoxCollider c)) {
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

            objectColors.Clear();
            errorCells.Clear();

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
                if (errorCells.Contains(cell.pos)) {
                    Handles.color = Color.red;
                    Handles.DrawDottedLines(box.edges, 0.3f);
                    Handles.DrawSolidDisc(box.center, group.transform.up, box.scale.x/4f);
                    continue;
                } else if (cell.type == Cell.CellType.FULL) {
                    Handles.color = Color.green;
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
                        Handles.color = Color.green;
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
