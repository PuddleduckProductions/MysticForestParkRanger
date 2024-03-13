using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(fileName = "SoundList", menuName ="ScriptableObjects/SoundList")]
public class SoundList : ScriptableObject
{
    protected Dictionary<string, EventReference> eventDict = new Dictionary<string, EventReference>();
    public List<EventReference> events = new List<EventReference>();

    // Music & Ambience are played in Event Emitters attached to game objects on and within the 'AudioManager' game object

    // SFX
    //EventReference playerFootsteps;
    //EventReference dugongVoice;
    //EventReference dragOnSand;
    //EventReference dragOnConcrete;
    //EventReference grabObject;

    public EventReference this[string name] {
        get {
            return eventDict[name];
        }
    }

    //public void DefineEventRef(EventReference soundEvent, string path)
    //{
    //    soundEvent = RuntimeManager.PathToEventReference(path);
    //    events.Add(soundEvent);
    //}

    public void Init() {
        //DefineEventRef(playerFootsteps, "event:/SFX/footsteps");
        //DefineEventRef(dugongVoice, "event:/Voice/dugongVoice");
        //DefineEventRef(dragOnSand, "event:/SFX/Grab&Drag/dragSand_Short");
        //DefineEventRef(dragOnConcrete, "event:/SFX/Grab&Drag/dragConcrete_Short");
        //DefineEventRef(grabObject, "event:/SFX/Grab&Drag/grabObject");

        foreach (var e in events) {
            if (!eventDict.ContainsKey(e.Guid.ToString())) {
                eventDict.Add(e.Guid.ToString(), e);
            }
        }
    }

}
