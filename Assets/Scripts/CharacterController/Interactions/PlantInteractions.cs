using Interactions.Behaviors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Interactions.Behaviors {
    public class WaterPipe : PickAndPutInteraction {
        // Idea: what if a pipe was a seed first?
        // And then a pipe carries water to grow a new seed?
        // Or like a system of roots?

        // TODO: Show this in Editor (and clean up in general)
        public enum PipeType { Straight, Corner, FourLeaf, ThreeLeaf };

        public PipeType type = PipeType.Straight;

        [HideInInspector]
        public LineRenderer renderer;

        public bool isGrown = false;

        protected Vector3[] ends;
        protected struct Source {
            GameObject gameObject;
            int index;

            public Source(GameObject sourceObject, int index) {
                gameObject = sourceObject;
                this.index = index;
            }
        }
        protected Source? source = null;

        public WaterPipe(Interaction parent) : base(parent) {
            if (!parent.gameObject.TryGetComponent<LineRenderer>(out renderer)) {
                renderer = parent.gameObject.AddComponent<LineRenderer>();
            }
        }


        List<WaterPipe> chainedObjects;
        public override void GameObjectStart() {
            chainedObjects = new List<WaterPipe>();
            DrawPipe();
        }

        public override void Interact() {
            base.Interact();
            ClearSource();
        }

        static Vector3[] FOUR_LEAF = new Vector3[] { new Vector3(-1, 1), new Vector3(1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, -1) };

        protected void DrawPipe() {
            if (!isGrown) {
                interactionObject.transform.localScale = Vector3.one * 0.5f;
                return;
            }
            renderer.numCapVertices = 1;
            ends = new Vector3[] { };
            switch (type) {
                case PipeType.Straight:
                    ends = new Vector3[] { new Vector3(-1, 1), new Vector3(1, 1) };
                    break;
                case PipeType.Corner:
                    ends = new Vector3[] { new Vector3(-1, 1), new Vector3(0, 1), new Vector3(0, 1, 1) };
                    break;
                case PipeType.FourLeaf:
                    ends = FOUR_LEAF;
                    break;
                case PipeType.ThreeLeaf:
                    ends = new Vector3[] { new Vector3(-1, 1), new Vector3(1, 1), new Vector3(0, 1, 1) };
                    break;
            }

            for (int i = 0; i < ends.Length; i++) {
                ends[i] = interactionObject.transform.rotation * ends[i];
                ends[i] += interactionObject.transform.position;
            }
            renderer.positionCount = ends.Length;
            renderer.SetPositions(ends);
        }

        public void SetGrown() {
            isGrown = true;
            interactionObject.transform.localScale = Vector3.one;
            DrawPipe();
        }

        public void SetSource(GameObject sourceObject, Vector3 hitPoint) {
            // Quick hack. Should advantage grid system when that gets made.
            int i = 0;
            foreach (var end in ends) {
                if (Vector3.Distance(end, hitPoint) <= 0.1f) {
                    source = new Source(sourceObject, i);
                    renderer.startColor = Color.blue;
                    renderer.endColor = Color.blue;
                } else {
                    var newEnd = new Vector3(end.x, interactionObject.transform.position.y, end.z);
                    if (Physics.Raycast(newEnd, newEnd - interactionObject.transform.position, out RaycastHit nextSource) && 
                        nextSource.collider.gameObject.TryGetComponent<Interaction>(out Interaction interaction) && interaction != interactionObject && 
                        interaction.behavior is WaterPipe w) {
                        chainedObjects.Add(w);
                        w.SetSource(interactionObject.gameObject, nextSource.point);
                    }
                }
                i++;
            }
        }

        public bool HasSource() {
            return source != null;
        }

        public void ClearSource() {
            source = null;
            renderer.startColor = Color.white;
            renderer.endColor = Color.white;
            foreach (var w in chainedObjects) {
                w.ClearSource();
            }
            chainedObjects.Clear();
        }

        public override bool Update() {
            DrawPipe();

            return base.Update();
        }

        public override void EndInteraction() {
            base.EndInteraction();
            DrawPipe();

            var endList = ends;
            if (endList == null) {
                endList = FOUR_LEAF;
                for (int i = 0; i < endList.Length; i++) {
                    endList[i] = interactionObject.transform.rotation * endList[i];
                    endList[i] += interactionObject.transform.position;
                }
            }
            foreach (var end in endList) {
                var newEnd = new Vector3(end.x, interactionObject.transform.position.y, end.z);
                if (Physics.Raycast(newEnd, newEnd - interactionObject.transform.position, out RaycastHit possibleSource)) {
                    if (possibleSource.collider.TryGetComponent(out Interaction i) &&
                    i.behavior is WaterPipe p) {
                        if (p.HasSource()) {
                            SetSource(i.gameObject, end);
                            break;
                        }
                    } else if (possibleSource.collider.TryGetComponent(out WaterSource s)) {
                        SetSource(s.gameObject, end);
                    }
                }
            }
        }


    }
}
