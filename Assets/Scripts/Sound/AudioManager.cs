using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using FMODUnity;
using FMOD.Studio;

//public class AudioManager : MonoBehaviour, ISingleton<AudioManager>
public class AudioManager : MonoBehaviour
{

    //public static AudioManager Instance { get; protected set; }
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        //(this as ISingleton<AudioManager>).Initialize();
        if (Instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene");
        }
        Instance = this;
    }


    public EventInstance CreateFMODInstance(EventReference soundEventPath)//, Transform objectTransform)
    {
        EventInstance soundInstance = RuntimeManager.CreateInstance(soundEventPath);
        //RuntimeManager.AttachInstanceToGameObject(soundInstance, objectTransform);
        return soundInstance;
    }

    public bool isPlaybackStatePaused(EventInstance soundInstance)
    {
        PLAYBACK_STATE playbackState;
        soundInstance.getPlaybackState(out playbackState);
        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            return true;
        }
        return false;
    }


    public void PlayOneShot(EventReference soundEventPath, Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(soundEventPath, position);
    }

    public void StartFMODEvent(EventInstance soundInstance)
    {
        soundInstance.start();
    }

    public void StopFMODEvent(EventInstance soundInstance)
    {
        soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}
