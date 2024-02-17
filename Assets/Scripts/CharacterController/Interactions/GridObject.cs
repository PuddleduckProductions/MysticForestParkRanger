using UnityEngine;

namespace Interactions {
    public class GridObject : MonoBehaviour {
        /// <summary>
        /// Set by <see cref="Interactions.GridEditor"/>.
        /// Local copy of the cells we own. We use these to access <see cref="manager"/>'s cells.
        /// </summary>
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

        /// <summary>
        /// Attempt to move this object in a given direction.
        /// </summary>
        /// <param name="direction">The direction to move in. Should be a normalized vector.</param>
        /// <returns>Whether or not the move was successful.</returns>
        public bool Move(Vector3 direction) {
            var gridMove = new Vector2Int(Mathf.FloorToInt(direction.x), Mathf.FloorToInt(direction.z));
            return manager.MoveObject(this, gridMove);
        }
    }
}