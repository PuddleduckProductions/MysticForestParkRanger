using UnityEngine;

namespace Interactions.Behaviors {
    public class TeleportInteraction : InteractionBehavior {
        public TeleportInteraction(Interaction parent) : base(parent) { }

        public Vector3 teleportLocal;

        public override void Interact() {
            GameObject.FindGameObjectWithTag("Player").transform.position = interactionObject.transform.position + teleportLocal;
        }

    }
}