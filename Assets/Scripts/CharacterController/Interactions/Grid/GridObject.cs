using UnityEngine;
using Utility;

namespace Interactions {
    [RequireComponent(typeof(BoxCollider))]
    public class GridObject : MonoBehaviour {
        /// <summary>
        /// Position for when trigger <see cref="onGridUpdate"/> should be called.
        /// If this is (-1, -1), then <see cref="onGridUpdate"/> will activate with every change in position.
        /// </summary>
        [Tooltip("Position for when trigger onGridUpdate should be called. If (-1,-1), then onGridUpdate will activate with every change in position.")]
        public Vector2Int positionToActivate = -1 * Vector2Int.one;

        /// <summary>
        /// Called EITHER when this object's <see cref="cells"/> are updated, or when <see cref="positionToActivate"/> is reached.
        /// If <see cref="positionToActivate"/> is (-1, -1), then this will always be called on a push.
        /// Supports either onGridUpdate(); or onGridUpdate(Vector2Int minBound, Vector2Int maxBound);
        /// </summary>
        [SerializeField, Tooltip("Called when this object's cells are updated. If positionToActive is (-1, -1), then this will always be called on a push.")]
        [SerializedMethod.MethodValidation(typeof(void), new System.Type[] {}),
            SerializedMethod.MethodValidation(typeof(void), new System.Type[] { typeof(Vector2Int), typeof(Vector2Int) } )]
        public SerializedMethod onGridUpdate = new SerializedMethod();

        /// <summary>
        /// Set by <see cref="Interactions.GridEditor"/>.
        /// Local copy of the cells we own. We use these to access <see cref="manager"/>'s cells.
        /// </summary>
        [HideInInspector]
        public GridGroup.Cell[] cells;

        /// <summary>
        /// Smallest position of the given cells.
        /// Assumes a square shape.
        /// </summary>
        public Vector2Int min { get { return _min; } }
        protected Vector2Int _min;

        /// <summary>
        /// Largest position of the given cells.
        /// Assumes a square shape.
        /// </summary>
        public Vector2Int max { get { return _max; } }
        protected Vector2Int _max;

        public GridGroup manager;

        public void Start() {
            if (manager == null) {
                Debug.LogError($"No GridGroup manager found for GridObject {name}. If a GridGroup object exists as the direct parent, select it to auto-set the manager.");
                return;
            }
            _min = new Vector2Int(int.MaxValue, int.MaxValue);
            _max = new Vector2Int(int.MinValue, int.MinValue);
            foreach (var cell in cells) {
                if (cell.pos.x < min.x) {
                    _min.x = cell.pos.x;
                }
                if (cell.pos.x > max.x) { 
                    _max.x = cell.pos.x;
                }
                if (cell.pos.y < min.y) {
                    _min.y = cell.pos.y;
                }
                if (cell.pos.y > max.y) {
                    _max.y = cell.pos.y;
                }
            }
        }

        protected void InvokeOnGridUpdate() {
            if (onGridUpdate.parameters.Length > 0) {
                onGridUpdate.Invoke(new object[] { _min, _max });
            } else {
                onGridUpdate.Invoke(new object[] { });
            }
        }

        /// <summary>
        /// Attempt to move this object in a given direction on the grid.
        /// Does NOT move the object's transform. That's the responsibility of whoever calls this function.
        /// Does NOT check move validity. This should be tested with <see cref="MoveIsValid(Vector2Int)"/>
        /// </summary>
        /// <param name="direction">The direction to move in. Should be a basis vector with exactly ONE non-zero component.</param>
        /// <returns>Whether or not the move was successful.</returns>
        public void Move(Vector2Int direction) {
            manager.MoveObject(this, direction);
            _min += direction;
            _max += direction;
            if (!onGridUpdate.IsNull()) {
                if (positionToActivate.x >= 0 && positionToActivate.y >= 0) {
                    if (positionToActivate.x >= _min.x && positionToActivate.x <= _max.x
                        && positionToActivate.y >= _min.y && positionToActivate.y <= _max.y) {
                        InvokeOnGridUpdate();
                    }
                } else {
                    InvokeOnGridUpdate();
                }
            }
        }

        /// <summary>
        /// Whether or not a move in a given direction is valid.
        /// </summary>
        /// <param name="direction">The direction to move in.</param>
        /// <returns>The validity.</returns>
        public bool MoveIsValid(Vector2Int direction) {
            return manager.MoveObjectIsValid(this, direction);
        }

        /// <summary>
        /// Get the first cell's neighbor cell in a direction.
        /// Not useful for much other than getting the position offsets between two cells.
        /// </summary>
        /// <param name="direction">The direction to pick.</param>
        /// <returns>The position of the neighboring cell.</returns>
        public Vector3? GetSomeAdjacent(Vector2Int direction) {
            var refCell = cells[0].pos + direction;
            if (manager.HasCell(refCell)) {
                return manager.CellToWorld(manager[refCell]).center;
            }
            return null;
        }

        public bool HasCell(Vector2Int pos) {
            foreach (var cell in cells) {
                if (cell.pos == pos) return true;
            }
            return false;
        }

        /// <summary>
        /// Remove this item from the <see cref="manager"/>.
        /// </summary>
        /// <param name="keepGridSpace">Whether or not to keep our object occupying its space in the grid.</param>
        public void RemoveFromGrid(bool keepGridSpace) {
            if (!keepGridSpace) {
                manager.EmptyCells(cells);
            }
            Destroy(this);
        }

        public void RemoveFromGridFull() {
            RemoveFromGrid(false);
        }

        public void RemoveFromGridKeepOccupied() {
            RemoveFromGrid(true);
        }

        /// <summary>
        /// Get a direction in the grid from a world direction.
        /// </summary>
        /// <param name="dir">World direction.</param>
        public Vector3 GetGridDirectionFromWorld(Vector3 dir) {
            return manager.transform.InverseTransformDirection(dir);
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.green;
            foreach (var cell in cells) {
                var box = manager.CellToWorld(cell, 0.5f);
                Gizmos.DrawLineList(box.edges);
            }
        }
    }
}