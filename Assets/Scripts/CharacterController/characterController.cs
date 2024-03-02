using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character {
    public class characterController : MonoBehaviour {
        public bool moveEnabled = true;
        CharacterController c;
        [HideInInspector]
        public Vector2 input;
        public float movementSpeed = 3f;
        public float rotationSpeed = 75f;
        public bool relativeDirectionalMovement = true;

        Camera mainCamera;

        PlayerAnimator playerAnimator;

        Vector3 velocity; // velocity variable
        public float friction = 0.99f; // friction value

        // Start is called before the first frame update
        void Start() {
            c = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            playerAnimator = GetComponent<PlayerAnimator>();

            AudioManager.Instance.RegisterSound("footsteps", "footsteps");//, this.transform);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(AudioManager.Instance["footsteps"], this.transform);
            //animator = GetComponent<Animator>();
        }

        Vector3 intendedForward = Vector3.zero;

        void FixedUpdate() {
            var playerForward = intendedMove;
            if (moveEnabled) {
                // adding acceleration
                velocity += movementSpeed * Time.deltaTime * playerForward;

                //applying friction
                velocity *= friction;

                c.SimpleMove(velocity);
            } else {
                velocity = Vector3.zero;
            }


            if (playerForward != Vector3.zero) {
                intendedForward = playerForward;
            }

            if (relativeDirectionalMovement) {
                transform.Rotate(Vector3.up, input.x * rotationSpeed * Time.deltaTime);
            } else if (intendedForward != Vector3.zero) {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(intendedForward, transform.up), rotationSpeed * Time.deltaTime);
            }
        }

        public Vector3 intendedMove {
            get {
                Quaternion simplifiedRot = Quaternion.AngleAxis(mainCamera.transform.eulerAngles.y, Vector3.up);

                Vector3 simplifiedForward = relativeDirectionalMovement ? transform.forward : simplifiedRot * Vector3.forward;
                Vector3 simplifiedRight = relativeDirectionalMovement ? transform.right : simplifiedRot * Vector3.right;

                Vector3 move = (simplifiedForward * input.y + simplifiedRight * input.x);
                move.Normalize();

                return move;
            }

        }

        void OnWalking(InputValue value) {
            input = value.Get<Vector2>();
            // animator.SetBool("walking", true);
        }
    }
}