using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using static Interaction;

/// <summary>
/// Editor for <see cref="Interaction"/>, to make selecting <see cref="InteractionBehavior"/> automated and easy for designers.
/// TODO: Need to figure out how to access the serialized properties of InteractionBehavior.
/// </summary>
[CustomEditor(typeof(Interaction))]
[CanEditMultipleObjects]
public class InteractionEditor : Editor {
    Dictionary<string, SerializedProperty> properties;

    SerializedProperty behavior;

    private void OnEnable() {
        behavior = serializedObject.FindProperty(nameof(Interaction.behavior));
    }

    //SerializedObject behavior;
    /*private void OnEnable() {
        //properties(interaction.behavior.SerializedProperties());
        Interaction interaction = (Interaction)target;
        if (interaction == null) {
            CreateBehavior(interaction);
        }
    }

    private void Awake() {
        Interaction interaction = (Interaction)target;
        if (interaction == null) {
            CreateBehavior(interaction);
        }
    }

    private void Reset() {
        Interaction interaction = (Interaction)target;
        if (interaction == null) {
            CreateBehavior(interaction);
        }
    }

    private void OnValidate() {
        Interaction interaction = (Interaction)target;
        if (interaction == null) {
            CreateBehavior(interaction);
        }
    }*/

    private void CreateBehavior(Interaction interaction) {
        switch (interaction.type) {
            case InteractionType.Ink:
                behavior.managedReferenceValue = new InkInteraction(interaction);
                break;
            case InteractionType.Pushable:
                behavior.managedReferenceValue = new PushableInteraction(interaction);
                break;
            case InteractionType.Custom:
                behavior.managedReferenceValue = new CustomInteraction(interaction);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        Interaction interaction = (Interaction)target;
        //InteractionBehavior behavior = 
        InteractionType initialType = interaction.type;
        base.DrawDefaultInspector();
        if (interaction.behavior == null || initialType != interaction.type) {
            CreateBehavior(interaction);
        }
        //interaction.type = (Interaction.InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interaction.type);
    }
}
