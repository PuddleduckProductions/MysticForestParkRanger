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
        public WaterPipe(Interaction parent) : base(parent) {
            if (!parent.gameObject.TryGetComponent<LineRenderer>(out renderer)) {
                renderer = parent.gameObject.AddComponent<LineRenderer>();
            }
            DrawLine();
        }

        protected void DrawLine() {
            renderer.numCapVertices = 1;
            Vector3[] arr = new Vector3[] { };
            switch (type) {
                case PipeType.Straight:
                    arr = new Vector3[] { new Vector3(-1, 1), new Vector3(1, 1) };
                    break;
                case PipeType.Corner:
                    arr = new Vector3[] { new Vector3(-1, 1), new Vector3(0, 1), new Vector3(0, 1, 1) };
                    break;
                case PipeType.FourLeaf:
                    arr = new Vector3[] { new Vector3(-1, 1), new Vector3(1, 1), new Vector3(0, 1, 1), new Vector3(0, 1, -1) };
                    break;
                case PipeType.ThreeLeaf:
                    arr = new Vector3[] { new Vector3(-1, 1), new Vector3(1, 1), new Vector3(0, 1, 1) };
                    break;
            }

            for (int i = 0; i < arr.Length; i++) {
                arr[i] = interactionObject.transform.rotation * arr[i];
                arr[i] += interactionObject.transform.position;
            }
            renderer.positionCount = arr.Length;
            renderer.SetPositions(arr);
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
