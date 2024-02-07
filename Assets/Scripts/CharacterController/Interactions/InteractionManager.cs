using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Interactions {
    public class InteractionManager : MonoBehaviour, ISingleton<InteractionManager> {
        public float interactionRange = 3.0f; //raycast length
        Ray ray; //raycast
        protected Camera mainCamera;

        protected GameObject player;
        protected Interaction[] interactionsInScene;
        protected RectTransform interactionButton;

        Interaction closestInteraction;
        // Start is called before the first frame update
        void Start() {
            interactionsInScene = GameObject.FindObjectsByType<Interaction>(FindObjectsSortMode.None);
            player = GameObject.FindGameObjectWithTag("Player");
            interactionButton = transform.GetChild(0).GetComponent<RectTransform>();

            mainCamera = Camera.main;

            ((ISingleton<InteractionManager>)this).Initialize();
            ISingleton<UIController>.Instance.onInteract.AddListener(Interact);
        }

        bool interactPressed = false;

        void Interact(bool interacted) {
            interactPressed = interacted;
            if (interactPressed && closestInteraction != null && CanInteract()) {
                closestInteraction.Interact();
            }
        }

        protected bool CanInteract() {
            return closestInteraction == null || !closestInteraction.IsInteracting();
        }
        
        //TODO: alter so that this uses a raycast system instead of looping through all
        public Interaction FindClosestInteraction() {
            Interaction closest = null;
            // TODO: Fix so that objects get filtered better without having to run this each loop.
            foreach (Interaction interaction in interactionsInScene) {
                if (interaction != null && interaction.gameObject.activeInHierarchy && interaction.interactionEnabled) {
                    var dist = Vector3.Distance(player.transform.position, interaction.transform.position);
                    if (dist <= interactionRange) {
                        interactionButton.position = mainCamera.WorldToScreenPoint(interaction.transform.position);
                        interactionButton.gameObject.SetActive(true);
                        closest = interaction;
                        break;
                    }
                }
            }
            if (closest == null) {
                interactionButton.gameObject.SetActive(false);
            }
            return closest;
        }

        // Update is called once per frame
        void Update() {
            if (closestInteraction == null || closestInteraction.interactionEnabled) {
                interactionButton.gameObject.SetActive(false);
            }
            if (CanInteract()) {
                closestInteraction = FindClosestInteraction();
            }
        }
    }
}