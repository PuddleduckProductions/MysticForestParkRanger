using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEventList : MonoBehaviour
{
    //[field: Header("Ambience")]
    [field: SerializeField] public FMODUnity.EventReference playerFootsteps {  get; private set; }


    public static FMODEventList Instance { get; private set; }

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FMODEventList instance in the scene");
        }
        Instance = this;
    }

}
