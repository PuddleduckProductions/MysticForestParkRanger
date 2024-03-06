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
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

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
                    if (AudioManager.Instance.isPlaybackStatePaused("footsteps")) {
                        footsteps.start();
                    }
                } else {
                    footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                break;
        }
    }
}
