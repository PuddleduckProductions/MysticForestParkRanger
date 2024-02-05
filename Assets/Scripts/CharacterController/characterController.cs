using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterController : MonoBehaviour
{
    public bool moveEnabled = true;
    CharacterController c;
    Vector2 input;
    //private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveEnabled) {
            c.Move(new Vector3(input.x * Time.deltaTime, 0, input.y * Time.deltaTime));
        }
    
    }
    void OnWalking(InputValue value)
    {
        input = value.Get<Vector2>();
       // animator.SetBool("walking", true);
    }
}
