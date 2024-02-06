using System;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using InkTools;
using Interactions.Behaviors;

namespace Interactions {
    namespace Behaviors {
        /// <summary>
        /// A serialized class meant to control different interaction behaviors when space is pressed on one.
        /// This is to avoid having to attach multiple monobehaviors for anything with one interaction.
        /// If you want to add your own, you can either use Custom (in progress), or create a subclass of InteractionBehavior.
        /// </summary>
        [Serializable]
        public abstract class InteractionBehavior {
            /// <summary>
            /// Reference to the MonoBehaviour <see cref="Interaction"/> for accessing things like position.
            /// It's a serialized field so we don't lose the reference to the parent when the scene starts.
            /// </summary>
            [SerializeField, HideInInspector]
            protected Interaction interactionObject;

            public InteractionBehavior(Interaction parent) {
                interactionObject = parent;
            }

            /// <summary>
            /// Are we currently having an interaction happen?
            /// If this is true, this supresses all other interactions in the scene.
            /// If active for more than one frame, <see cref="Update"/> is called.
            /// </summary>
            public abstract bool isInteracting { get; }

            /// <summary>
            /// Function to call when the object is interacted with (i.e., Space is pressed)
            /// Called by <see cref="InteractionManager"/>.
            /// </summary>
            public abstract void Interact();
            /// <summary>
            /// While <see cref="isInteracting"/> is true, call this function.
            /// </summary>
            public virtual void Update() { }

            /// <summary>
            /// Called when another interaction wants to do something with this.
            /// </summary>
            /// <param name="other">The other interaction calling this.</param>
            public virtual void ChainInteraction(Interaction other) { }

        }

        /// <summary>
        /// For displaying dialog in Ink. Should be customizable, but right now just loads `interact_Name` for the Name of the current GameObject.
        /// </summary>
        [Serializable]
        public class InkInteraction : InteractionBehavior {
            public InkInteraction(Interaction parent) : base(parent) { }

            /// <summary>
            /// Active as long as <see cref="InkManager.storyActive"/> is active.
            /// </summary>
            public override bool isInteracting => InkTools.InkManager.storyActive;

            /// <summary>
            /// Call interact_<see cref="UnityEngine.Object.name"/> in Ink.
            /// TODO: Make customizable
            /// </summary>
            public override void Interact() {
                ISingleton<InkManager>.Instance.StartDialog("interact_" + interactionObject.name);
                ISingleton<UIController>.Instance.onInteract.AddListener(InteractAdvance);
                ISingleton<InkManager>.Instance.dialogEnd.AddListener(EndDialog);
            }

            public void EndDialog() {
                ISingleton<UIController>.Instance.onInteract.RemoveListener(InteractAdvance);
            }

            public void InteractAdvance(bool pressed) {
                if (pressed && InkManager.storyActive) {
                    ISingleton<InkManager>.Instance.AdvanceStory();
                }
            }
        }

        /// <summary>
        /// Push an object around.
        /// TODO: Disable collider on push.
        /// </summary>
        [Serializable]
        public class PushableInteraction : InteractionBehavior {
            public PushableInteraction(Interaction parent) : base(parent) { }

            public override bool isInteracting => isPushing;
            /// <summary>
            /// Whether player is still pushing. Active until space is pressed.
            /// </summary>

            protected bool isPushing;
            /// <summary>
            /// Our reference to the player.
            /// </summary>
            GameObject player;

            /// <summary>
            /// Stored offset between the player and pushed object.
            /// </summary>
            Vector3 offset;

            /// <summary>
            /// Set ourselves to push, and hook into the interaction system to get when space is pressed again (to <see cref="ReleasePush(bool)"/>
            /// </summary>
            public override void Interact() {
                player = GameObject.FindGameObjectWithTag("Player");
                isPushing = true;
                ISingleton<UIController>.Instance.onInteract.AddListener(ReleasePush);
                offset = interactionObject.transform.position - player.transform.position;
                var Collider = interactionObject.GetComponent<Collider>();
                Collider.enabled = false;
            }

            /// <summary>
            /// Set pushed to false if pressed is true.
            /// </summary>
            /// <param name="pressed">Whether or not interact was pressed.</param>
            protected void ReleasePush(bool pressed) {
                if (pressed) {
                    isPushing = false;
                    ISingleton<UIController>.Instance.onInteract.RemoveListener(ReleasePush);
                    var Collider = interactionObject.GetComponent<Collider>();
                    Collider.enabled = true;
                }
            }

            /// <summary>
            /// Update the pushed object to move with us.
            /// </summary>
            public override void Update() {
                interactionObject.transform.position = player.transform.position + offset;
            }
        }

        /// <summary>
        /// Custom interaction setup. Define your own interaction behavior through other scripts.
        /// Called once.
        /// TODO: Updates.
        /// </summary>
        [Serializable]
        public class CustomInteraction : InteractionBehavior {
            public CustomInteraction(Interaction parent) : base(parent) { }

            /// <summary>
            /// Functions to call when space is pressed on this object.
            /// </summary>
            [SerializeField]
            protected UnityEvent onInteract = new UnityEvent();

            public override bool isInteracting => false;

            public override void Interact() {
                onInteract.Invoke();
            }

            /*public override void Update() {
                onUpdate(ref interactRef);
            }*/
        }
        
        [Serializable]
        public class PickAndPutInteraction : InteractionBehavior {
            public PickAndPutInteraction(Interaction parent) : base(parent) { }

            public override bool isInteracting => isPicking;
            protected bool isPicking = false;

            GameObject player;

            public override void Interact() {
                player = GameObject.FindGameObjectWithTag("Player");
                isPicking = true;
                interactionObject.interactionEnabled = false;
                ISingleton<UIController>.Instance.onInteract.AddListener(PlaceDown);
            }

            Interaction closest = null;
            public void PlaceDown(bool pressed) {
                if (pressed) {
                    ISingleton<UIController>.Instance.onInteract.RemoveListener(PlaceDown);
                    interactionObject.transform.position = player.transform.position + player.transform.forward;
                    interactionObject.interactionEnabled = true;
                    isPicking = false;

                    if (closest != null) {
                        closest.behavior.ChainInteraction(this.interactionObject);
                        GameObject.Destroy(interactionObject.gameObject);
                    }
                }
            }


            public override void Update() {
                interactionObject.transform.position = player.transform.position + new Vector3(0, 1.5f, 0.0f);
                closest = ISingleton<InteractionManager>.Instance.FindClosestInteraction();
            }
        }

        [Serializable]

        public class PutTrigger : InteractionBehavior {
            public PutTrigger(Interaction parent) : base(parent) { }

            public override bool isInteracting => false;

            /// <summary>
            /// Calls with the object that just interacted with this trigger. Use this to define custom place behavior.
            /// </summary>
            [Tooltip("Calls with the object that just interacted with this trigger. Use this to define custom place behavior.")]
            public UnityEvent<GameObject> onChained = new UnityEvent<GameObject>();

            public override void Interact() {
                Debug.Log("No interaction");
            }

            public override void ChainInteraction(Interaction other) {
                onChained.Invoke(other.gameObject);
            }
        }
    } 

    public class Interaction : MonoBehaviour {
        /// <summary>
        /// Should we allow interaction with this object?
        /// If this is set to false while <see cref="IsInteracting"/> is true,
        /// this will allow control over <see cref="InteractionManager.interactionButton"/>
        /// </summary>
        public bool interactionEnabled = true;
        public enum InteractionType { Ink, Pushable, PickAndPut, PutTrigger, Custom };
        public InteractionType type;
        [SerializeReference]
        public Behaviors.InteractionBehavior behavior;

        public bool HasInteractionBehavior() {
            return behavior != null;
        }

        public bool IsInteracting() {
            return behavior.isInteracting;
        }

        public void Interact() {
            behavior.Interact();
        }

        private void Update() {
            if (behavior != null && IsInteracting()) {
                behavior.Update();
            }
        }
    }
}