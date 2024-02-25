using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class AudioManager : MonoBehaviour, ISingleton<AudioManager> 
{
    // Start is called before the first frame update
    void Awake()
    {
        (this as ISingleton<AudioManager>).Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
