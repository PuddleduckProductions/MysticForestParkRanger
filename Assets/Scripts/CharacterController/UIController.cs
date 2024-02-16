using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;

namespace Character {
    public class UIController : MonoBehaviour, ISingleton<UIController> {
        // TODO: Is this called by anything but the InteractionManager? Avoid making it a UnityEvent.
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

        public void OnPause(InputValue value) {
            if (value.isPressed) {
                if (ISingleton<PauseManager>.Instance.isPaused) {
                    ISingleton<PauseManager>.Instance.Resume();
                } else {
                    ISingleton<PauseManager>.Instance.Pause();
                }
            }
        }
    }
}