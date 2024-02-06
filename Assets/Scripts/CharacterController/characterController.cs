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

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        camera = Camera.main;
        //animator = GetComponent<Animator>();
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
    }
    void OnWalking(InputValue value)
    {
        input = value.Get<Vector2>();
       // animator.SetBool("walking", true);
    }
}
