using UnityEngine;
using Interactions.Behaviors;
using System.Collections;
using System.Collections.Generic;
using Interactions;
using System;

namespace Interactions {
    public class GridGroup : MonoBehaviour {
        [Serializable]
        public struct Cell {
            public enum CellType { EMPTY, FULL };
            public CellType type;
            public Vector2Int pos;
            public Vector2Int scale;

            public Cell(CellType type, Vector2Int pos, Vector2Int scale) {
                this.type = type;
                this.pos = pos;
                this.scale = scale;
            }
        }

        public struct Box {
            public Vector3 scale;
            public Vector3 center;
            public Box(Vector3 center, Vector3 scale) {
                this.center = center;
                this.scale = scale;
            }
        }

        /// <summary>
        /// How big the cells are in the world.
        /// </summary>
        public Vector3 cellSize = Vector3.one;

        /// <summary>
        /// How far the cells are apart.
        /// </summary>
        public Vector3 cellSpacing = Vector3.zero;

        /// <summary>
        /// Where to position the grid in space.
        /// </summary>
        public Vector3 gridOffset;

        public Vector2Int gridDimensions = new Vector2Int(10, 10);

        public Cell[] cells;

        /// <summary>
        /// For ease of syntax
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Cell this[int x, int y] {
            get { return cells[y * gridDimensions.x + x]; }
            set { cells[y * gridDimensions.x + x] = value; }
        }

        /// <summary>
        /// returns the cell in a specific direction from a current cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Cell cellInDirection(Cell cell, Vector2Int direction) {
            int X = cell.pos.x + direction.x;
            int Y = cell.pos.y + direction.y;
            Debug.Log($"X: {X}, Y: {Y}");
            return this[X, Y];
        }

        public Box CellToWorld(Cell cell) {
            var corner = transform.position + gridOffset + new Vector3(cell.pos.x * (cellSize.x + cellSpacing.x), 0, cell.pos.y * (cellSize.z + cellSpacing.z));
            var size = new Vector3(cell.scale.x * cellSize.x, 1, cell.scale.y * cellSize.y);
            return new Box(corner + size/2, size);
        }

        // checks if a cell is valid & unoccupied
        private bool IsCellValid(int x, int y) {

            return false;
        }

        void OnDrawGizmos() {
            foreach (var cell in cells) {
                var box = CellToWorld(cell);
                Gizmos.color = Color.black;
                if (cell.type == Cell.CellType.FULL) {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawWireCube(box.center, box.scale);
            }
        }

    }
}