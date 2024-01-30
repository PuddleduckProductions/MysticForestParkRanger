using System.Collections;
using System.Collections.Generic;
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

    //SerializedObject behavior;
    private void OnEnable() {
        //properties(interaction.behavior.SerializedProperties());

    }

    public override void OnInspectorGUI() {
        Interaction interaction = (Interaction)target;
        //InteractionBehavior behavior = 
        InteractionType initialType = interaction.type;
        base.DrawDefaultInspector();
        if (interaction.behavior == null || initialType != interaction.type) {
            switch (interaction.type) {
                case InteractionType.Ink:
                    interaction.behavior = new InkInteraction(interaction);
                    break;
                case InteractionType.Pushable:
                    interaction.behavior = new PushableInteraction(interaction);
                    break;
                case InteractionType.Custom:
                    interaction.behavior = new CustomInteraction(interaction);
                    break;
            }
        }
        //interaction.type = (Interaction.InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interaction.type);
        interaction.behavior.OnInspectorGUI();
    }
}
