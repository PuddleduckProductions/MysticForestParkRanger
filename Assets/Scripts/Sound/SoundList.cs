using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "SoundList", menuName ="ScriptableObjects/SoundList")]
public class SoundList : ScriptableObject
{
    protected Dictionary<string, EventReference> eventDict = new Dictionary<string, EventReference>();
    public List<EventReference> events = new List<EventReference>();

    public EventReference this[string name] {
        get {
            return eventDict[name];
        }
    }

    public void Init() {
        foreach (var e in events) {
            if (!eventDict.ContainsKey(e.Path)) {
                eventDict.Add(e.Path, e);
            }
        }
    }

}
