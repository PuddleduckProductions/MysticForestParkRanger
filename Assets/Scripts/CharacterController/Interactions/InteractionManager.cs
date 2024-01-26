using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactionRange = 5.0f;

    protected Camera mainCamera;

    protected GameObject player;
    protected GameObject[] interactionsInScene;
    protected GameObject interactionButton;
    // Start is called before the first frame update
    void Start()
    {
        interactionsInScene = GameObject.FindGameObjectsWithTag("Interactive");
        player = GameObject.FindGameObjectWithTag("Player");
        interactionButton = transform.GetChild(0).gameObject;

        mainCamera = Camera.main;

        UIController.onInteract.AddListener(Interact);
    }

    bool interactPressed = false;

    void Interact(bool interacted) {
        interactPressed = interacted;
    }

    // Update is called once per frame
    void Update()
    {
        var isActive = false;
        if (!InkManager.storyActive) {
            // TODO: Fix so that objects get filtered better without having to run this each loop.
            foreach (GameObject interaction in interactionsInScene) {
                if (interaction != null && interaction.activeInHierarchy) {
                    var dist = Vector3.Distance(player.transform.position, interaction.transform.position);
                    if (dist <= interactionRange) {
                        isActive = true;
                        interactionButton.transform.position = mainCamera.WorldToScreenPoint(interaction.transform.position);
                        if (interactPressed) {
                            InkManager.startDialog.Invoke("interact_" + interaction.name);
                            interactPressed = false;
                        }
                        break;
                    }
                }
            }
        }
        interactionButton.SetActive(isActive);
    }
}
