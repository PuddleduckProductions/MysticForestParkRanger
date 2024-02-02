using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterController : MonoBehaviour
{
    CharacterController c;
    Vector2 input;
    public int movementScalar;
    //private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        movementScalar = 5;//animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        c.Move(new Vector3(input.x*movementScalar*Time.deltaTime,0,input.y*movementScalar*Time.deltaTime));
    
    }
    void OnWalking(InputValue value)
    {
        input = value.Get<Vector2>();
       // animator.SetBool("walking", true);
    }
}
