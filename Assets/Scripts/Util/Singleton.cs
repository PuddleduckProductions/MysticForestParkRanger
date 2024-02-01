using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISingleton<T> where T: MonoBehaviour
{
    public static T Instance { get; protected set; }
    
    public static void Initialize(T instance) {
        if (Instance == null) {
            Instance = instance;
        } else {
            Debug.LogWarning("Singleton " + typeof(T).Name + " already exists.");
            Object.Destroy(instance);
        }
    }
}
