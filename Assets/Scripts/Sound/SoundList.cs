using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName ="ScriptableObjects/SoundList")]
public class SoundList : ScriptableObject
{
    public Dictionary<string, FMODUnity.EventReference> events;

}
