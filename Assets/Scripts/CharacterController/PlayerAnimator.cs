using FMOD.Studio;
using FMODUnity;
using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    CharacterController controller;

    EventInstance footsteps;
    string footstepName;
    [SerializeField]
    EventReference footstepRef;

    Ray floorRay;
    public LayerMask floorLayer;
    private int dist = 10;
    public int previousFloor = 0;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        //TODO: when the level ends, release 'footsteps' from memory
        footstepName = "footsteps";
        footsteps = AudioManager.Instance.RegisterSound(footstepName, footstepRef);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(footsteps, this.transform);

    }

    Vector2Int pushDir;
    public void UpdatePush(Vector2Int dir) {
        // Temp fix, this should be a trigger:
        //animator.SetBool("walking", false);
        pushDir = dir;
    }
    // Update is called once per frame
    void Update()
    {
        switch (ISingleton<InteractionManager>.Instance.currentInteractionType) {
            case "Grid/Pushable":
                // Pushing interactions here
                animator.SetBool("forwardHold", pushDir.magnitude > 0);
                break;
            case null:
            default: //If the value is null or something else, we can stick with regular animations
                // Reset forwardHold in case we were on that before:
                // ARTISTS, YOU SHOULD BE MAKING THIS A TRIGGER SO I DON'T HAVE TO DO THIS.
                animator.SetBool("forwardHold", false);

                var xzVel = new Vector3(controller.velocity.x, 0, controller.velocity.z);
                bool isWalking = xzVel.magnitude > 2f;
                animator.SetBool("walking", isWalking);
                
                //FMOD
                if (isWalking) {
                    //AudioManager.Instance.PlayEventLoop(footstepName, footstepRef);
                    if (AudioManager.Instance.isPlaybackStatePaused(footsteps))
                    {
                        footsteps.start();
                    }
                    FloorCheck();
                } else {
                    //AudioManager.Instance.StopEventLoop(footstepName);
                    if (!AudioManager.Instance.isPlaybackStatePaused(footsteps))
                    {
                        footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    }
                    
                }
                break;
  
        }
    }



    //Floor Layer Check Function for FMOD footstep changing
    void FloorCheck()
    {
        //casts a ray from the player to the floor
        floorRay = new Ray(transform.position, -Vector3.up);

        //checks to see if that ray is colliding with layers 10-14
        if (Physics.Raycast(floorRay, out RaycastHit hit, dist, floorLayer))
        {
            int currentFloor = hit.collider.gameObject.layer;

            //TODO: refine the system so it doesn't set the parameter constantly. Only when the ground changes, or when the event turns on
           if (currentFloor != previousFloor)
           {
                switch (currentFloor)
                {
                    case 10: //grass
                        //AudioManager.Instance.SetLocalParameter(footstepName, "floorType", 0f);
                        footsteps.setParameterByName("floorType", 0f);
                        previousFloor = currentFloor;
                        break;
                    case 11: //water
                        //AudioManager.Instance.SetLocalParameter(footstepName, "floorType", 1f);
                        footsteps.setParameterByName("floorType", 1f);
                        previousFloor = currentFloor;
                        break;
                    case 12: //sand
                        //AudioManager.Instance.SetLocalParameter(footstepName, "floorType", 2f);
                        footsteps.setParameterByName("floorType", 2f);
                        previousFloor = currentFloor;
                        break;
                    case 13: //wood
                        //AudioManager.Instance.SetLocalParameter(footstepName, "floorType", 3f);
                        footsteps.setParameterByName("floorType", 3f);
                        previousFloor = currentFloor;
                        break;
                    case 14: //concrete
                        //AudioManager.Instance.SetLocalParameter(footstepName, "floorType", 4f);
                        footsteps.setParameterByName("floorType", 3f);
                        previousFloor = currentFloor;
                        break;
                }
            }
        } 
    }
}
