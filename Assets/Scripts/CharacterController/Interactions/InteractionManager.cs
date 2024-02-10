using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Interactions {
    /// <summary>
    /// A State Machine for searching for interactions and updating based on their behavior.
    /// </summary>
    public class InteractionManager : MonoBehaviour, ISingleton<InteractionManager> {
        public float interactionRange = 3.0f;

        protected Camera mainCamera;

        protected GameObject player;
        protected Interaction[] interactionsInScene;
        protected RectTransform interactionButton;

        public enum InteractionState {
            EMPTY,
            HOLDING_INTERACTION
        }

        public InteractionState interactionMode = InteractionState.EMPTY;

        Interaction closestInteraction;
        Interaction usingInteraction;
        // Start is called before the first frame update
        void Start() {
            interactionsInScene = GameObject.FindObjectsByType<Interaction>(FindObjectsSortMode.None);
            player = GameObject.FindGameObjectWithTag("Player");
            interactionButton = transform.GetChild(0).GetComponent<RectTransform>();

            mainCamera = Camera.main;

            ((ISingleton<InteractionManager>)this).Initialize();
            ISingleton<UIController>.Instance.onInteract.AddListener(Interact);
        }

        protected void UpdateMode(InteractionState newState) {
            switch (newState) {
                case InteractionState.EMPTY:
                    
                    break;
            }
            interactionMode = newState;
        }

        bool interactPressed = false;

        /// <summary>
        /// Should be called by <see cref="Interactions.Behaviors.InteractionBehavior"/> whenever we want to stop
        /// the current interaction.
        /// </summary>
        public void StopCurrentInteraction() {
            if (interactionMode == InteractionState.HOLDING_INTERACTION) {
                usingInteraction.EndInteraction();
                usingInteraction = null;
                interactionMode = InteractionState.EMPTY;
            }
        }

        void Interact(bool interacted) {
            interactPressed = interacted;
            if (interactPressed) {
                if (closestInteraction != null) {
                    if (interactionMode == InteractionState.HOLDING_INTERACTION) {
                        closestInteraction.Interact(usingInteraction);
                    } else if (interactionMode == InteractionState.EMPTY) {
                        closestInteraction.Interact();

                        usingInteraction = closestInteraction;
                        closestInteraction = null;
                        interactionMode = InteractionState.HOLDING_INTERACTION;
                    }
                } else if (interactionMode == InteractionState.HOLDING_INTERACTION) {
                    usingInteraction.Interact();
                }
            }
        }

        protected bool CanInteract() {
            return usingInteraction == null;
        }

        public Interaction FindClosestInteraction() {
            Interaction closest = null;
            // TODO: Fix so that objects get filtered better without having to run this each loop.
            foreach (Interaction interaction in interactionsInScene) {
                if (interaction != null && interaction.gameObject.activeInHierarchy && interaction.interactionEnabled 
                    && interaction.CanInteract(usingInteraction)) {
                    var dist = Vector3.Distance(player.transform.position, interaction.transform.position);
                    if (dist <= interactionRange) {
                        closest = interaction;
                        break;
                    }
                }
            }
            return closest;
        }

        // Update is called once per frame
        void Update() {
            if (interactionMode == InteractionState.HOLDING_INTERACTION) {
                var shouldContinue = usingInteraction.InteractionUpdate();
                if (!shouldContinue) {
                    StopCurrentInteraction();
                }
            }

            closestInteraction = FindClosestInteraction();
            if (closestInteraction != null) {
                interactionButton.position = mainCamera.WorldToScreenPoint(closestInteraction.transform.position);
                interactionButton.gameObject.SetActive(true);
            } else if (closestInteraction == null || !closestInteraction.interactionEnabled) {
                interactionButton.gameObject.SetActive(false);
            }
        }
    }
}