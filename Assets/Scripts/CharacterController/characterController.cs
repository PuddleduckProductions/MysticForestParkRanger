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
        Quaternion simplifiedRot = Quaternion.AngleAxis(camera.transform.eulerAngles.y, Vector3.up);
        Vector3 simplifiedForward = simplifiedRot * Vector3.forward;
        Vector3 simplifiedRight = simplifiedRot * Vector3.right;
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
