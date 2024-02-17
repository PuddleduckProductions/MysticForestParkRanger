using UnityEngine;
using System;

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
            return new Box(corner + cellSize/2, cellSize);
        }

        public Cell? WorldToCell(Vector3 pos) {
            Vector3 localPos = new Vector3(pos.x, 0, pos.z) - (transform.position + gridOffset);
            int gridX = Mathf.FloorToInt(localPos.x / (cellSize.x + cellSpacing.x));
            int gridY = Mathf.FloorToInt(localPos.z / (cellSize.z + cellSpacing.z));
            if (gridX < 0 || gridX >= gridDimensions.x || gridY < 0 || gridY >= gridDimensions.y) {
                return null;
            }
            // TODO: Figure out multiple cells together to make one object.
            return new Cell(GridGroup.Cell.CellType.FULL, new Vector2Int(gridX, gridY));
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
                cells[i].pos += direction;
                GetCell(cell.pos + direction).type = cell.type;
            }
        }

        /// <summary>
        /// Attempt to move the given gridObject in a direction.
        /// Assumes no diagonals.
        /// Called by <see cref="GridObject.Move(Vector3)"/>, which you should call instead of this function.
        /// </summary>
        /// <param name="direction">The direction to move in. Assumes no diagonals.</param>
        /// <returns>Whether or not the move was successful.</returns>
        public bool MoveObject(GridObject gridObject, Vector2Int direction) {
            // Test the edges of a move.
            int testMin = 0;
            int testMax = 0;
            Vector2Int increment = Vector2Int.zero;
            Vector2Int startPos = Vector2Int.zero;
            if (direction.x != 0) {
                testMin = gridObject.min.y;
                testMax = gridObject.max.y;
                if (direction.x < 0) {
                    startPos = gridObject.min;
                    increment = Vector2Int.left;
                } else {
                    startPos = gridObject.max;
                    increment = Vector2Int.right;
                }
            } else if (direction.y != 0) {
                testMin = gridObject.min.x;
                testMax = gridObject.max.x;
                if (direction.y < 0) {
                    startPos = gridObject.min;
                    increment = Vector2Int.down;
                } else {
                    startPos = gridObject.max;
                    increment = Vector2Int.up;
                }
            }

            for (int i = testMin; i <= testMax; i++) {
                if (!MoveValid(startPos + increment * i, direction)) {
                    return false;
                }
            }

            // TODO: Lerp
            gridObject.transform.position += new Vector3(direction.x * (cellSpacing.x + cellSize.x), 0, direction.y * (cellSpacing.z + cellSize.z));

            MoveCells(ref gridObject.cells, direction);
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
                if (cell.pos == Vector2Int.zero) {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawWireCube(box.center, box.scale);
            }
        }

    }
}