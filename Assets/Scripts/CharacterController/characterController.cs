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
        public float rotationSpeedMultiplier = 0.75f;
        public float movementSpeedMultiplier = 0.5f;

        //pushing force, make player stronger
        public float pushForce = 1f;

        Camera mainCamera;

        public Animator animator;
        //private Animator animator;
        // Start is called before the first frame update
        void Start() {
            c = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            animator = GetComponentInChildren<Animator>();
            //animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update() {
            if (moveEnabled) {
                float currRotationSpeed = rotationSpeed;
                float currMoveSpeed = movementSpeed;
                if (input.y <= 0) {
                    currRotationSpeed *= rotationSpeedMultiplier; // Decrease rotation speed if not moving forward
                    currMoveSpeed *= movementSpeed * movementSpeedMultiplier;
                }

                Quaternion simplifiedRot = Quaternion.AngleAxis(mainCamera.transform.eulerAngles.y, Vector3.up);

                Vector3 simplifiedForward = relativeDirectionalMovement ? transform.forward : simplifiedRot * Vector3.forward;
                Vector3 simplifiedRight = relativeDirectionalMovement ? transform.right : simplifiedRot * Vector3.right;

                Vector3 move = (simplifiedForward * input.y + simplifiedRight * input.x);


                if (relativeDirectionalMovement) {
                    transform.Rotate(Vector3.up, input.x * currRotationSpeed * Time.deltaTime);
                } else if (move != Vector3.zero) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move, transform.up), currRotationSpeed * Time.deltaTime);
                }
                move.Normalize();
                c.SimpleMove(move * movementSpeed);

                // Check if the magnitude of input is greater than a threshold (e.g., 0.1)
                bool isWalking = input.magnitude > 0.01f;

                // Set the "walking" parameter in the animator based on the input magnitude
                animator.SetBool("walking", isWalking);
            }
        }

        void OnWalking(InputValue value) {
            input = value.Get<Vector2>();
            // animator.SetBool("walking", true);
        }
    }
}