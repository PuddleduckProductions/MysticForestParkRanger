using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    /// <summary>
    /// Singleton Interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISingleton<T> where T : MonoBehaviour {
        public static T Instance { get; protected set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init() {
            Instance = null;
        }

        public void Initialize() {
            if (Instance == null) {
                Instance = (T)this;
            } else {
                Debug.LogWarning("Singleton " + typeof(T).Name + " already exists.");
                Object.Destroy((T)this);
            }
        }
    }
}