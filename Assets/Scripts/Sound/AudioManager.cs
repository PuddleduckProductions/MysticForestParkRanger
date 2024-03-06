using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// System for loading in and keeping track of sounds.
/// </summary>
public class AudioManager
{
    protected static AudioManager _instance;
    public static AudioManager Instance { get {
            if (_instance == null) {
                _instance = new AudioManager();
            }
            return _instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() {
        _instance = null;
    }

    SoundList eventRefs;

    public AudioManager() {
        eventRefs = (SoundList)Resources.Load("MasterSoundList");
        eventRefs.Init();
    }

    protected Dictionary<string, EventInstance> sounds = new Dictionary<string, EventInstance>()    ;

    public EventInstance this[string name] {
        get {
            return sounds[name];
        }
    }

    public EventInstance RegisterSound(string name, EventReference soundEventPath)//, Transform objectTransform)
    {
        EventInstance soundInstance = RuntimeManager.CreateInstance(soundEventPath);
        //RuntimeManager.AttachInstanceToGameObject(soundInstance, objectTransform);

        //UPDATE: Check to see if the name is already being used, if so, replace the old instance with the new one
        if (!sounds.ContainsKey(name)) {
            sounds.Add(name, soundInstance);
        } else {
            sounds[name] = soundInstance;
        }
        return soundInstance;
    }

    public EventInstance RegisterSound(string instanceName, string eventReferenceName) {
        EventInstance soundInstance = RuntimeManager.CreateInstance(eventRefs[eventReferenceName]);
        sounds.Add(instanceName, soundInstance);
        return soundInstance;
    }

    public delegate EventInstance ModifySound(EventInstance inst);

    public void UpdateSound(string name, ModifySound updateFunc) {
        var sound = sounds[name];
        sounds[name] = updateFunc(sound);
    }

    public bool SoundExists(string name) {
        return sounds.ContainsKey(name);
    }



    public bool isPlaybackStatePaused(string name)
    {
        PLAYBACK_STATE playbackState;
        EventInstance soundInstance = sounds[name];
        soundInstance.getPlaybackState(out playbackState);
        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            return true;
        }
        return false;
    }

    //public void PlayOneShotWithParameters(EventReference soundEventPath, string parameterName, float parameterValue)//, Vector3 position)
    //{
    //    EventInstance soundInstance = RuntimeManager.CreateInstance(soundEventPath);
    //    soundInstance.setParameterByName(parameterName, parameterValue);

    //    //instance.set3DAttributes(position.To3DAttributes());
    //    soundInstance.start();
    //    soundInstance.release();
    //}

    public void StartFMODEvent(string name)
    {
        sounds[name].start();
    }

    public void StopFMODEvent(string name)
    {
        sounds[name].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}
