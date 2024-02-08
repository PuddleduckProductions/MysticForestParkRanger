using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Interactions {
    public class InteractionManager : MonoBehaviour, ISingleton<InteractionManager> {
        public float interactionRange = 0f; //the distance the box can reach to

        [Tooltip("determines size of boxcast relative to player size")]
        public Vector3 extentsScale = new Vector3(1f, 1f, 1f);
        private Vector3 interactionExtents;
        
        protected Camera mainCamera;

        protected GameObject player;
        protected Interaction[] interactionsInScene;
        protected RectTransform interactionButton;

        RaycastHit closestHit; // stores hit info (of closest one)
        Interaction closestInteraction; // stores the interaction info (of closest one)
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
                if (hit.collider != null) { //if collider is not null
                    Interaction interaction = hit.collider.GetComponent<Interaction>(); //get interaction component of hit
                    if(interaction != null && interaction.gameObject.activeInHierarchy && interaction.interactionEnabled){ //if inter. not null & inter enabled
                        var distance = Vector3.Distance(playerPosition, hit.point); // get distance from player --> hit
                        if (distance < closestDistance) { //if closest distance so far
                            closestDistance = distance; //change closest distance
                            interactionButton.position = mainCamera.WorldToScreenPoint(interaction.transform.position); //canvas stuff
                            interactionButton.gameObject.SetActive(true); //ui stuff
                            closest = interaction;
                            break;
                        }
                    }
                }
            }
            
            if (closest == null) {
                interactionButton.gameObject.SetActive(false);
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
            if (closestInteraction == null || closestInteraction.interactionEnabled) {
                interactionButton.gameObject.SetActive(false);
            }
            if (CanInteract()) {
                closestInteraction = FindClosestInteraction();
            }
        }
    }
}