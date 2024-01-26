using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactionRange = 3.0f;

    protected Camera mainCamera;

    protected GameObject player;
    protected Interaction[] interactionsInScene;
    protected RectTransform interactionButton;

    Interaction closestInteraction;
    // Start is called before the first frame update
    void Start()
    {
        interactionsInScene = GameObject.FindObjectsByType<Interaction>(FindObjectsSortMode.None);
        player = GameObject.FindGameObjectWithTag("Player");
        interactionButton = transform.GetChild(0).GetComponent<RectTransform>();

        mainCamera = Camera.main;

        UIController.onInteract.AddListener(Interact);
    }

    bool interactPressed = false;

    void Interact(bool interacted) {
        interactPressed = interacted && !InkManager.storyActive;
        if (interactPressed && closestInteraction != null) {
            closestInteraction.Interact();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var isActive = false;
        closestInteraction = null;
        if (!InkManager.storyActive) {
            // TODO: Fix so that objects get filtered better without having to run this each loop.
            foreach (Interaction interaction in interactionsInScene) {
                if (interaction != null && interaction.gameObject.activeInHierarchy) {
                    var dist = Vector3.Distance(player.transform.position, interaction.transform.position);
                    if (dist <= interactionRange) {
                        isActive = true;
                        interactionButton.position = mainCamera.WorldToScreenPoint(interaction.transform.position);
                        closestInteraction = interaction;
                        break;
                    }
                }
            }
        }
        interactionButton.gameObject.SetActive(isActive);
    }
}
