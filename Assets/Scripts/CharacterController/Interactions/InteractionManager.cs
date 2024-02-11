using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Interactions {
    /// <summary>
    /// A State Machine for searching for interactions and updating based on their behavior.
    /// </summary>
    public class InteractionManager : MonoBehaviour, ISingleton<InteractionManager> {
        public float interactionRange = 0f; //the distance the box can reach to

        [Tooltip("determines size of boxcast relative to player size")]
        public Vector3 extentsScale = new Vector3(1f, 1f, 1f);
        private Vector3 interactionExtents;
        
        protected Camera mainCamera;

        protected GameObject player;
        protected Interaction[] interactionsInScene;
        protected RectTransform interactionButton;

        public enum InteractionState {
            EMPTY,
            HOLDING_INTERACTION
        }

        public InteractionState interactionMode = InteractionState.EMPTY;

        /// <summary>
        /// Stores closest interaction to be displayed with a contextual button prompt.
        /// </summary>
        Interaction closestInteraction;
        /// <summary>
        /// The active interaction that we're using.
        /// </summary>
        Interaction usingInteraction;

        // Start is called before the first frame update
        void Start() {
            interactionsInScene = GameObject.FindObjectsByType<Interaction>(FindObjectsSortMode.None);
            player = GameObject.FindGameObjectWithTag("Player");
            interactionButton = transform.GetChild(0).GetComponent<RectTransform>();

            mainCamera = Camera.main;
            
            ((ISingleton<InteractionManager>)this).Initialize();
            ISingleton<Character.UIController>.Instance.onInteract.AddListener(Interact);
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
            /* performs a boxcast from the player's pos in the direction they're facing, w/ box volume defined by interactionRange. 
            then iterates through all hits & checks if any of the colliders' hit contains Interaction component. 
            If so, returns the closest interaction accordingly.*/
            float closestDistance = float.MaxValue;
            Interaction closest = null;
            Vector3 playerPosition = player.transform.position;

            interactionExtents = new Vector3( //a more dynamic version of player scale * 0.5f
                player.transform.localScale.x * extentsScale.x, 
                player.transform.localScale.y * extentsScale.y, 
                player.transform.localScale.z * extentsScale.z
            );
            //params: center, half extents, forward, orientation
            RaycastHit[] hits = Physics.BoxCastAll(playerPosition, interactionExtents* .5f, player.transform.forward, player.transform.rotation, interactionRange);
            foreach (RaycastHit hit in hits) {
                if (hit.collider != null && hit.collider.TryGetComponent<Interaction>(out Interaction interaction)) {
                    if(interaction.gameObject.activeInHierarchy
                        // CanInteract accounts for the possibility that usingInteraction is null.
                        && interaction.interactionEnabled && interaction.CanInteract(usingInteraction)){
                        var distance = Vector3.Distance(playerPosition, hit.point); // get distance from player --> hit
                        if (distance < closestDistance) { //if closest distance so far
                            closestDistance = distance; //change closest distance
                            interactionButton.position = mainCamera.WorldToScreenPoint(interaction.transform.position); //canvas stuff
                            closest = interaction;
                        }
                    }
                }
            }
            return closest;
        }

        void OnDrawGizmos() {
            if(player == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(player.transform.position, player.transform.forward * interactionRange);
            Gizmos.DrawWireCube(player.transform.position + player.transform.forward * interactionRange, interactionExtents); //shows interaction range
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