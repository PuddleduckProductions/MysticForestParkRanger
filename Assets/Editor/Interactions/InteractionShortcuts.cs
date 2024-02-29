using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Interactions;
using Interactions.Behaviors;
using System.Linq;

public class InteractionShortcuts
{
    [MenuItem("Puddleduck/Interactions/Refresh All Interactions")]
    static void RefreshAll() {
        var interactions = GameObject.FindObjectsOfType<Interaction>();
        Refresh(interactions);
    }

    private static void Refresh(Interaction[] interactions) {
        Undo.SetCurrentGroupName("Refresh Interactions");
        int group = Undo.GetCurrentGroup();

        Undo.RecordObjects(interactions, "Interactions to refresh");
        foreach (var interaction in interactions) {
            var so = new SerializedObject(interaction);
            InteractionEditor.CreateBehavior(interaction.behaviorType, interaction, interaction.behavior.GetType(), so);
            Debug.Log($"Refreshed {interaction.name}");
        }

        Undo.CollapseUndoOperations(group);
    } 

    /// <summary>
    /// Refresh the selected interactions so that they work (if they're giving you errors). Use CTRL+ALT+R.
    /// </summary>
    [MenuItem("Puddleduck/Interactions/Refresh Selected Interactions ^&r")]
    static void RefreshSelected() {
        var interactions = (Interaction[])Selection.gameObjects.Select(o => o.GetComponent<Interaction>()).Where(i => i != null).ToArray();
        Refresh(interactions);
    }
}
