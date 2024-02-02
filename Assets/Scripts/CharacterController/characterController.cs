using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterController : MonoBehaviour
{
    CharacterController c;
    Vector2 input;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        c.Move(new Vector3(input.x*Time.deltaTime,0,input.y*Time.deltaTime));

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
