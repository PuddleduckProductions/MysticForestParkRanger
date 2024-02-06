using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Interactions {
    public class InteractionManager : MonoBehaviour, ISingleton<InteractionManager> {
        public float interactionRange = 3.0f;

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

        public Interaction FindClosestInteraction() {
            Interaction closest = null;
            // TODO: Fix so that objects get filtered better without having to run this each loop.
            foreach (Interaction interaction in interactionsInScene) {
                if (interaction != null && interaction.gameObject.activeInHierarchy && interaction.interactionEnabled) {
                    var dist = Vector3.Distance(player.transform.position, interaction.transform.position);
                    if (dist <= interactionRange) {
                        interactionButton.position = mainCamera.WorldToScreenPoint(closest.transform.position);
                        interactionButton.gameObject.SetActive(true);
                        closest = interaction;
                        break;
                    }
                }
            }
            return closest;
        }

        // Update is called once per frame
        void Update() {
            if (CanInteract()) {
                closestInteraction = FindClosestInteraction();
                if (closestInteraction == null) {
                    interactionButton.gameObject.SetActive(false);
                }
            } else if (interactionButton.gameObject.activeInHierarchy) {
                interactionButton.gameObject.SetActive(false);
            }
        }
    }
}