using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

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

    public EventReference this[string name] {
        get {
            return eventDict[name];
        }
    }

    public void Init() {
        foreach (var e in events) {
            if (!eventDict.ContainsKey(e.Guid.ToString())) {
                eventDict.Add(e.Guid.ToString(), e);
            }
        }
    }

}
