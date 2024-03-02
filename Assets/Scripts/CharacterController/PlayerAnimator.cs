using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    CharacterController controller;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (ISingleton<InteractionManager>.Instance.currentInteractionType) {
            case "Grid/Pushable":
                // Pushing interactions here
                break;
            default: //If the value is null or something else, we can stick with regular animations
                var xzVel = new Vector3(controller.velocity.x, 0, controller.velocity.z);
                bool isWalking = xzVel.magnitude > 0.1f;
                animator.SetBool("walking", isWalking);

                if (isWalking) {
                    if (AudioManager.Instance.isPlaybackStatePaused("footsteps")) {
                        AudioManager.Instance["footsteps"].start();
                    }
                } else {
                    AudioManager.Instance["footsteps"].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                break;
        }
    }
}
