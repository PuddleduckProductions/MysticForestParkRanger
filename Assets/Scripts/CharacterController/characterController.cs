using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterController : MonoBehaviour
{
    CharacterController c;
    Camera camera;
    Vector2 input;
    public float movementSpeed = 3f;
    public float rotationSpeed = 75f;
    public bool relativeDirectionalMovement = true;
    public float rotationSpeedMultiplier = 0.75f;
    public float movementSpeedMultiplier = 0.5f;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        camera = Camera.main;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        float currRotationSpeed = rotationSpeed;
        float currMoveSpeed = movementSpeed;
        if (input.y <= 0) {
            currRotationSpeed *= rotationSpeedMultiplier; // Decrease rotation speed if not moving forward
            currMoveSpeed *= movementSpeed * movementSpeedMultiplier;
        }

        transform.Rotate(Vector3.up, input.x * currRotationSpeed  * Time.deltaTime);
        Quaternion simplifiedRot = Quaternion.AngleAxis(camera.transform.eulerAngles.y, Vector3.up);

        Vector3 simplifiedForward = relativeDirectionalMovement ? transform.forward : simplifiedRot * Vector3.forward;
        Vector3 simplifiedRight = relativeDirectionalMovement ? transform.right : simplifiedRot * Vector3.right;

        Vector3 move = (simplifiedForward * input.y + simplifiedRight * input.x);
        move.Normalize();
        c.SimpleMove(move * movementSpeed);
        
        // Check if the magnitude of input is greater than a threshold (e.g., 0.1)
        bool isWalking = input.magnitude > 0.01f;

        // Set the "walking" parameter in the animator based on the input magnitude
        animator.SetBool("walking", isWalking);
    }

    void OnWalking(InputValue value)
    {
        input = value.Get<Vector2>();
    }
}
