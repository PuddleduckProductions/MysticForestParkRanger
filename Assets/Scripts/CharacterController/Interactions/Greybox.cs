using UnityEngine;
using FMODUnity;

namespace Interactions.Behaviors {
    [InteractionType("Misc/Teleport")]
    public class TeleportInteraction : InteractionBehavior {
        public TeleportInteraction(Interaction parent) : base(parent) { }

        public Vector3 teleportLocal;

        [SerializeField]
        public EventReference bounceSound;

        public override void Interact() {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
            var worldPos = interactionObject.transform.position + teleportLocal;
            player.enabled = false;
            player.transform.position = worldPos;
            AudioManager.Instance.PlayOneShot("mushroomBounce", bounceSound, player.transform);
            player.enabled = true;
        }

    }
}