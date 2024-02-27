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

        Animator animator;

        Vector3 velocity; // velocity variable
        public float friction = 0.99f; // friction value

        //private Animator animator;

        //Fmod call
        public FMODUnity.EventReference footstepsEvent;

        FMOD.Studio.EventInstance footSteps;

        // Start is called before the first frame update
        void Start() {
            c = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            animator = GetComponentInChildren<Animator>();

                //footSteps = FMODUnity.RuntimeManager.CreateInstance(footstepsEvent);
                //FMODUnity.RuntimeManager.AttachInstanceToGameObject(footSteps, this.transform);
                //footSteps.start();
            //animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update() {
            if (moveEnabled) {
                // adding acceleration
                velocity += intendedMove * movementSpeed * Time.deltaTime;

                //applying friction
                velocity *= friction;

                c.SimpleMove(velocity);
            } else {
                velocity = Vector3.zero;
            }

            var xzVel = new Vector3(c.velocity.x, 0, c.velocity.z);

            if (relativeDirectionalMovement) {
                transform.Rotate(Vector3.up, input.x * rotationSpeed * Time.deltaTime);
            } else if (xzVel != Vector3.zero) {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(xzVel, transform.up), rotationSpeed * Time.deltaTime);
            }

            bool isWalking = xzVel.magnitude > 0.01f;
            
            animator.SetBool("walking", isWalking);

                //if (isWalking){
                //    footSteps.setPaused(false);
                //} else {
                //    //footSteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                //    footSteps.setPaused(true);
                //    footSteps.setTimelinePosition(0);
                //}
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