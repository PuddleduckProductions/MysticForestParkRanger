using System.Collections;
using System.Collections.Generic;
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
        Vector2Int min;
        /// <summary>
        /// Largest position of the given cells.
        /// Assumes a square shape.
        /// </summary>
        Vector2Int max;

        public GridGroup manager;

        public void Start() {
            min = new Vector2Int(int.MaxValue, int.MaxValue);
            max = new Vector2Int(int.MinValue, int.MinValue);
            foreach (var cell in cells) {
                if (cell.pos.x < min.x) {
                    min.x = cell.pos.x;
                }
                if (cell.pos.x > max.x) { 
                    max.x = cell.pos.x;
                }
                if (cell.pos.y < min.y) {
                    min.y = cell.pos.y;
                }
                if (cell.pos.y > max.y) {
                    max.y = cell.pos.y;
                }
            }
        }

        /// <summary>
        /// Attempt to move this object in a given direction.
        /// </summary>
        /// <param name="direction">The direction to move in.</param>
        /// <returns>Whether or not the move was successful.</returns>
        // TODO: Move the actual object along the direction.
        public bool Move(Vector2Int direction) {
            for (int i = min.x; i < max.x; i++) {
                for (int j = min.y; j < max.y; j++) {
                    if (!manager.MoveValid(new Vector2Int(i, j), direction)) {
                        return false;
                    }
                }
            }

            manager.MoveCells(ref cells, direction);
            return true;
        }
    }
}