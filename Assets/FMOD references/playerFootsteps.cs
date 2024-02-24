using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerFootsteps : MonoBehaviour
{

    public GameObject player;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 lastScale;

    public FMOD.Studio.EventInstance instance;


    // Start is called before the first frame update
    void Start()
    {
        lastPosition = player.transform.position;
        lastRotation = player.transform.rotation;
        lastScale = player.transform.localScale;
        instance = FMODUnity.RuntimeManager.CreateInstance("");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position != lastPosition || player.transform.rotation != lastRotation || player.transform.localScale != lastScale)
        {
            instance.start();
        } 
        else
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
