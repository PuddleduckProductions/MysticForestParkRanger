using Interactions.Behaviors;
using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions {
    /// <summary>
    /// Super hacky. TODO: Replace with better grid system.
    /// </summary>
    public class WaterSource : MonoBehaviour {
        void Update() {
            // I hate this so much.
            var interactions = GameObject.FindObjectsOfType<Interaction>();
            foreach(var interaction in interactions) {
                if (interaction.behavior is WaterPipe p && Vector3.Distance(interaction.transform.position, this.transform.position) <= 1.5f) {
                    p.SetGrown();
                }
            }
        }
    }
}