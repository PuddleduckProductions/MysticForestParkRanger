using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerFootsteps : MonoBehaviour
{

    [SerializeField] public GameObject player;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 lastScale;

    private FMOD.Studio.EventInstance instance;


    // Start is called before the first frame update
    void Start()
    {
        lastPosition = player.transform.position;
        lastRotation = player.transform.rotation;
        lastScale = player.transform.localScale;
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/footsteps");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.hasChanged)
        {
            instance.start();
            lastPosition = player.transform.position;
            lastRotation = player.transform.rotation;
            lastScale = player.transform.localScale;
        } 
        else
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
