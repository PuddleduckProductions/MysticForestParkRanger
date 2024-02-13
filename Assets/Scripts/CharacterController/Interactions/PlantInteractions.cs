using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            DrawLine();
        }

        protected void DrawLine() {
            if (!isGrown) {
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
                    ends = new Vector3[] { new Vector3(-1, 1), new Vector3(1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, -1) };
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

        public void SetSource(GameObject sourceObject, Vector3 hitPoint) {
            // Quick hack. Should advantage grid system when that gets made.
            int i = 0;
            foreach (var end in ends) {
                if (Vector3.Distance(end, hitPoint) <= 0.1f) {
                    source = new Source(sourceObject, i);
                    renderer.startColor = Color.blue;
                    renderer.endColor = Color.blue;
                } else {
                    Physics.Raycast(end, interactionObject.transform.position - end, out RaycastHit nextSource);
                    if (nextSource.collider.gameObject.TryGetComponent<Interaction>(out Interaction interaction) && interaction.behavior is WaterPipe w) {
                        w.SetSource(interactionObject.gameObject, nextSource.point);
                    }
                }
                i++;
            }
        }

        public void ClearSource() {
            source = null;
            renderer.startColor = Color.white;
            renderer.endColor = Color.white;
            foreach (var end in ends) {
                Physics.Raycast(end, interactionObject.transform.position - end, out RaycastHit nextSource);
                if (nextSource.collider.gameObject.TryGetComponent<Interaction>(out Interaction interaction) && interaction.behavior is WaterPipe w) {
                    w.ClearSource();
                }
            }
        }

        public override bool Update() {
            DrawLine();

            return base.Update();
        }

        public override void EndInteraction() {
            base.EndInteraction();
            DrawLine();
        }


    }
}
