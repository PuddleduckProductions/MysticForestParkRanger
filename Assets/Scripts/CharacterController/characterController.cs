using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterController : MonoBehaviour
{
    CharacterController c;
    Vector2 input;
    public float movementSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        c.Move(new Vector3(input.x*3f*Time.deltaTime,0,input.y*3f*Time.deltaTime));
        Vector3 horizontalVelocity = c.velocity;
        horizontalVelocity = new Vector3(c.velocity.x, 0, c.velocity.z);

        float horizontalSpeed = horizontalVelocity.magnitude;

        float verticalSpeed = c.velocity.y;

        float overallSpeed = c.velocity.magnitude;
    }
    void OnWalking(InputValue value)
    {
        input = value.Get<Vector2>();
       // animator.SetBool("walking", true);
    }
}
