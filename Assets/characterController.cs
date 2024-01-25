using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterController : MonoBehaviour
{
    CharacterController c;
    Vector2 input;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        c.Move(new Vector3(input.x*Time.deltaTime,0,input.y*Time.deltaTime));
        
    }
    void OnWalking(InputValue value)
    {
        input = value.Get<Vector2>();
    }
}
