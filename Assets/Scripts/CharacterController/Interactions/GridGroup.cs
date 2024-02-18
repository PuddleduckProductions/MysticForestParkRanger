using UnityEngine;
using System;
using System.Collections.Generic;

namespace Interactions {
    public class GridGroup : MonoBehaviour {
        [Serializable]
        public struct Cell {
            /// <summary>
            /// EMPTY represents nothing. FULL represents an object (controlled by <see cref="GridObject"/>. MAP_FULL indicates a cell that's been set to full that can be
            /// reset to EMPTY from the scene editor.
            /// </summary>
            public enum CellType { EMPTY, FULL, MAP_FULL };
            public CellType type;
            public Vector2Int pos;
            public Quaternion rotation;
            
            public Cell(CellType type, Vector2Int pos, Quaternion rot) {
                this.type = type;
                this.pos = pos;
                this.rotation = rot;
            }
        }

        public struct Box {
            public Vector3 scale;
            public Vector3 center;
            public Vector3[] edges;
            public Box(Vector3 center, Vector3 scale, Vector3[] edges) {
                this.center = center;
                this.scale = scale;
                this.edges = edges;
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

            var forward = Vector3.forward * cellSize.z;
            var up = Vector3.up * cellSize.y;
            var right = Vector3.right * cellSize.x;

            var edges = new Vector3[] {
                // Left face
                corner, corner + forward,
                corner + forward, corner + forward + up,
                corner + forward + up, corner + up,
                corner + up, corner,

                // Right face
                corner, corner + right,
                corner + right, corner + right + forward,
                corner + right + forward, corner + right + forward + up,
                corner + right + forward + up, corner + right + up,
                corner + right + up, corner + right,

                // Connect the three missing points
                corner + up, corner + up + right,
                corner + up + forward, corner + up + forward + right,
                corner + forward, corner + forward + right

            };

            Vector3 applyRot(Vector3 point) {
                return (transform.rotation * (point - transform.position)) + transform.position;
            }
            for (int i = 0; i < edges.Length; i++) {
                edges[i] = applyRot(edges[i]);
            }

            var center = applyRot(corner + cellSize / 2);
            
            return new Box(center, cellSize, edges);
        }

        public Cell? LocalToCell(Vector3 pos) {
            //Vector3 localPos = PointToLocal(pos);
            //localPos -= gridOffset;
            //Debug.Log($"{localPos} {pos}");
            //Vector3 localPos = transform.rotation * (pos - (transform.position + gridOffset));
            int gridX = Mathf.FloorToInt(pos.x / (cellSize.x + cellSpacing.x));
            int gridY = Mathf.FloorToInt(pos.z / (cellSize.z + cellSpacing.z));
            if (gridX < 0 || gridX >= gridDimensions.x || gridY < 0 || gridY >= gridDimensions.y) {
                return null;
            }
            // TODO: Figure out multiple cells together to make one object.
            return new Cell(GridGroup.Cell.CellType.FULL, new Vector2Int(gridX, gridY), this.transform.rotation);
        }

        public Vector3 PointToLocal(Vector3 pos) {
            return transform.InverseTransformPoint(pos) - gridOffset;
        }

        public List<Cell> BoundsToCells(BoxCollider c) {
            var toAdd = new List<Cell>();

            var start = PointToLocal(c.transform.TransformPoint(c.center - c.size/2));
            var end = PointToLocal(c.transform.TransformPoint(c.center + c.size/2));

            for (float x = start.x; x < end.x; x += cellSize.x + cellSpacing.x) {
                for (float y = start.z; y < end.z; y += cellSize.z + cellSpacing.z) {
                    // TODO: Figure out multiple cells together to make one object.
                    var cell = LocalToCell(new Vector3(x, 0, y));
                    if (cell == null) {
                        Debug.LogWarning($"{c.name} does not fit in grid at {x},{y}");
                        return null;
                    }
                    toAdd.Add((GridGroup.Cell)cell);
                }
            }
            return toAdd;
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
        /// Does not move the object physically in the space. You have to edit the transform yourself.
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
                // Test along the y-axis:
                testMin = gridObject.min.y;
                testMax = gridObject.max.y;
                // If we're testing to the left:
                if (direction.x < 0) {
                    // Start from the bottom left,
                    startPos = gridObject.min;
                    // And move up to the top left.
                    increment = Vector2Int.up;
                } else {
                    startPos = gridObject.max;
                    increment = Vector2Int.down;
                }
            } else if (direction.y != 0) {
                testMin = gridObject.min.x;
                testMax = gridObject.max.x;
                if (direction.y < 0) {
                    startPos = gridObject.min;
                    increment = Vector2Int.right;
                } else {
                    startPos = gridObject.max;
                    increment = Vector2Int.left;
                }
            }

            for (int i = 0; i <= testMax - testMin; i++) {
                if (!MoveValid(startPos + increment * i, direction)) {
                    return false;
                }
            }

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

    }
}