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
                c.SimpleMove(intendedMove * movementSpeed);
            }

            var xzVel = new Vector3(c.velocity.x, 0, c.velocity.z);

            if (relativeDirectionalMovement) {
                transform.Rotate(Vector3.up, input.x * rotationSpeed * Time.deltaTime);
            } else if (xzVel != Vector3.zero) {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(xzVel, transform.up), rotationSpeed * Time.deltaTime);
            }

            animator.SetBool("walking", xzVel.magnitude > 0.01f);
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