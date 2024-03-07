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
    [SerializeField]
    EventReference footstepRef;

    Ray floorRay;
    public LayerMask floorLayer;
    private int dist = 10;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        //TODO: when the level ends, release 'footsteps' from memory
        footsteps = AudioManager.Instance.RegisterSound("footsteps", footstepRef);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(footsteps, this.transform);

    }

    // Update is called once per frame
    void Update()
    {
        switch (ISingleton<InteractionManager>.Instance.currentInteractionType) {
            case "Grid/Pushable":
                // Pushing interactions here
                break;
            case null:
            default: //If the value is null or something else, we can stick with regular animations
                var xzVel = new Vector3(controller.velocity.x, 0, controller.velocity.z);
                bool isWalking = xzVel.magnitude > 0.1f;
                animator.SetBool("walking", isWalking);
                
                if (isWalking) {
                    FloorCheck();
                    if (AudioManager.Instance.isPlaybackStatePaused("footsteps")) {
                        footsteps.start();

                    }
                } else {
                    footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
            switch (currentFloor)
            {
                case 10:
                    footsteps.setParameterByNameWithLabel("floorType", "Grass");
                    break;
                case 11:
                    footsteps.setParameterByNameWithLabel("floorType", "Water");
                    break;
                case 12:
                    footsteps.setParameterByNameWithLabel("floorType", "Sand");
                    break;
                case 13:
                    footsteps.setParameterByNameWithLabel("floorType", "Wood");
                    break;
                case 14:
                    footsteps.setParameterByNameWithLabel("floorType", "Concrete");
                    break;
            }


        } 
    }
}
