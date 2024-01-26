using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

class UIController : MonoBehaviour
{
    public static UnityEvent<bool> onInteract;
    private void Awake() {
        if (onInteract == null) {
            onInteract = new UnityEvent<bool>();
        }
    }
    void OnInteract(InputValue value) {
        onInteract.Invoke(value.isPressed);
    }
}
