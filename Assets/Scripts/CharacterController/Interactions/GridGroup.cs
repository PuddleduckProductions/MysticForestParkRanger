using UnityEngine;
using Interactions.Behaviors;
using System.Collections;
using System.Collections.Generic;
using Interactions;
using System;
using static Interactions.GridGroup;

namespace Interactions {
    public class GridGroup : MonoBehaviour {
        [Serializable]
        public struct Cell {
            public enum CellType { EMPTY, FULL };
            public CellType type;
            public Vector2Int pos;
            
            public Cell(CellType type, Vector2Int pos) {
                this.type = type;
                this.pos = pos;
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

        public Cell this[Vector2Int idx] { 
            get { return cells[idx.y * gridDimensions.x + idx.x]; }
            set { cells[idx.y * gridDimensions.x + idx.x] = value; }
        }

        public Cell this[int idx] {
            get { return cells[idx]; }
            set { cells[idx] = value; }
        }

        public ref Cell GetCell(Vector2Int idx) {
            return ref cells[idx.y * gridDimensions.x + idx.x];
        }

        public Box CellToWorld(Cell cell) {
            var corner = transform.position + gridOffset + new Vector3(cell.pos.x * (cellSize.x + cellSpacing.x), 0, cell.pos.y * (cellSize.z + cellSpacing.z));
            var size = new Vector3(cellSize.x, 1, cellSize.y);
            return new Box(corner + size/2, size);
        }


        /// <summary>
        /// Move all cells of a given array in a specific direction.
        /// Does NOT check if the move will overwrite existing cells.
        /// Should call <see cref="MoveValid(Vector2Int, Vector2Int)"/> for the edges of the move to see if you should call this.
        /// </summary>
        /// <param name="cells">Cells to move.</param>
        /// <param name="direction">Direction to move the cells in.</param>
        /// <returns></returns>
        protected void MoveCells(ref Cell[] cells, Vector2Int direction) {
            for (int i = 0; i < cells.Length; i++) {
                var cell = cells[i];
                GetCell(cell.pos).type = Cell.CellType.EMPTY;
                cell.pos += direction;
                GetCell(cell.pos).type = cell.type;
                i++;
            }
        }

        /// <summary>
        /// Attempt to move this object in a given direction.
        /// </summary>
        /// <param name="direction">The direction to move in.</param>
        /// <returns>Whether or not the move was successful.</returns>
        public bool MoveObject(GridObject gridObject, Vector2Int direction) {
            for (int i = gridObject.min.x; i < gridObject.max.x; i++) {
                for (int j = gridObject.min.y; j < gridObject.max.y; j++) {
                    if (!MoveValid(new Vector2Int(i, j), direction)) {
                        return false;
                    }
                }
            }

            // TODO: Lerp
            gridObject.transform.position += new Vector3(direction.x * (cellSpacing.x + cellSize.x), 0, direction.y * (cellSpacing.z + cellSize.z));

            MoveCells(ref cells, direction);
            return true;
        } 

        /// <summary>
        /// Return whether or not the given move from one cell to another would be valid.
        /// Does not count for overlapping moves (i.e., you're moving one cell owned by one object onto another cell owned by the same object).
        /// </summary>
        /// <param name="pos">The position of the cell.</param>
        /// <param name="direction">The direction the cell is moving.</param>
        /// <returns>Whether or not the move is valid.</returns>
        public bool MoveValid(Vector2Int pos, Vector2Int direction) {
            var target = pos + direction;
            if (target.x >= 0 && target.x < gridDimensions.x && target.y >= 0 && target.y < gridDimensions.y) {
                return this[target].type == Cell.CellType.EMPTY; 
            } else {
                return false;
            }
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