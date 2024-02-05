using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;

public class UIController : MonoBehaviour, ISingleton<UIController>
{
    public UnityEvent<bool> onInteract;
    private void Awake() {
        ((ISingleton<UIController>)this).Initialize();
    }

    /// <summary>
    /// New input system control. DO NOT RENAME (Looking at you, Tyler)
    /// </summary>
    public void OnInteract(InputValue value) {
        onInteract.Invoke(value.isPressed);
    }
}
