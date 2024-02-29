using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Interactions.Behaviors {
    /// <summary>
    /// An object that can be picked up and placed within the world.
    /// </summary>
    [Serializable]
    public class PickAndPutInteraction : InteractionBehavior {
        public PickAndPutInteraction(Interaction parent) : base(parent) { }
        protected bool isPicking = false;

        GameObject player;
        Collider playerCollider;
        Collider c;
        Bounds colliderBounds;

        public override void Interact() {
            if (!isPicking) {
                player = GameObject.FindGameObjectWithTag("Player");
                isPicking = true;
                interactionObject.interactionEnabled = false;
                c = interactionObject.GetComponent<Collider>();
                playerCollider = player.GetComponent<Collider>();
                colliderBounds = c.bounds;
                c.enabled = false;
            } else {
                // Will force InteractionManager to call EndInteraction.
                isPicking = false;
            }
        }

        public override void EndInteraction() {
            interactionObject.transform.position = player.transform.position + player.transform.forward * (0.5f +
                Mathf.Max(
                Vector3.Dot(Vector3.forward, colliderBounds.extents),
                playerCollider.bounds.size.z
                ));
            // Doesn't work rn because of player.
            if (Physics.Raycast(interactionObject.transform.position, Vector3.down, out RaycastHit hit)) {
                interactionObject.transform.position += new Vector3(0, (hit.point.y - interactionObject.transform.position.y) + colliderBounds.size.y / 2);
            }
            interactionObject.interactionEnabled = true;
            c.enabled = true;
        }


        public override bool Update() {
            interactionObject.transform.position = player.transform.position + new Vector3(0, playerCollider.bounds.size.y);
            interactionObject.transform.rotation = player.transform.rotation;
            return isPicking;
        }
    }

    /// <summary>
    /// An object where a <see cref="PickAndPutInteraction"/> can be placed.
    /// TODO: Not finished, needs to be choosy.
    /// </summary>
    [Serializable]
    public class PutTrigger : InteractionBehavior {
        public PutTrigger(Interaction parent) : base(parent) { }

        /// <summary>
        /// Calls with the object that just interacted with this trigger. Use this to define custom place behavior.
        /// </summary>
        [Tooltip("Calls with the object that just interacted with this trigger. Use this to define custom place behavior.")]
        public UnityEvent<GameObject> onChained = new UnityEvent<GameObject>();

        /// <summary>
        /// A list of interactions with tags that are allowed to interact with this placement.
        /// </summary>
        [Tooltip("A list of interactions with tags that are allowed to interact with this placement.")]
        public List<string> allowedTags = new List<string>();

        public override void Interact() { }

        public override void Interact(Interaction other) {
            onChained.Invoke(other.gameObject);
            // FIXME: Probably a more elegant solution based on what we want later on.
            // This is fine for now.
            ISingleton<InteractionManager>.Instance.StopCurrentInteraction();
            GameObject.Destroy(other.gameObject);
        }

        public override bool CanInteract(Interaction other = null) {
            if (other == null) return false;
            foreach (var tag in allowedTags) {
                if (tag == other.tag) {
                    return true;
                }
            }
            return false;
        }
    }

    public class Seed : PickAndPutInteraction {
        public Seed(Interaction parent) : base(parent) { }

        /// <summary>
        /// The object to instantiate when this is planted.
        /// </summary>
        [Tooltip("The object to instantiate when this is planted.")]
        public GameObject instantiateOnPlant;
        /// <summary>
        /// The name of the seed. Used by <see cref="DirtPatch"/> in <see cref="DirtPatch.seedsToAccept"/>.
        /// </summary>
        [Tooltip("The name of the seed. Used by DirtPatch in seedsToAccept.")]
        public string seedName;
    }

    public class DirtPatch : InteractionBehavior {
        /// <summary>
        /// The <see cref="Seed.seedName"/> to accept. Will allow any seeds if blank.
        /// </summary>
        [Tooltip("The seedNames to accept. Will allow any seeds if blank.")]
        public List<string> seedsToAccept;

        /// <summary>
        /// A reference to a GameObject to set active when something has been planted here.
        /// </summary>
        [Tooltip("A reference to a GameObject to set active when something has been planted here.")]
        public GameObject activateOnPlant;
        public DirtPatch(Interaction parent) : base(parent) { }

        public override void Interact() {
            throw new NotImplementedException();
        }

        public override void Interact(Interaction other) {
            Seed s = other.behavior as Seed;
            if (s.instantiateOnPlant != null) {
                GameObject.Instantiate(s.instantiateOnPlant, interactionObject.transform);
            }
            if (activateOnPlant != null) {
                activateOnPlant.SetActive(true);
            }

            GameObject.Destroy(other.gameObject);
        }

        public override bool CanInteract(Interaction other = null) {
            if (other != null && other.behavior is Seed s) {
                if (seedsToAccept.Count == 0) {
                    return true;
                } else {
                    foreach (var seedName in seedsToAccept) {
                        if (seedName == s.seedName) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
