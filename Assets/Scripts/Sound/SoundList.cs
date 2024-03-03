using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName ="ScriptableObjects/SoundList")]
public class SoundList : ScriptableObject
{
    protected Dictionary<string, FMODUnity.EventReference> eventDict = new Dictionary<string, FMODUnity.EventReference>();
    public List<FMODUnity.EventReference> events = new List<FMODUnity.EventReference>();

    public FMODUnity.EventReference this[string name] {
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
