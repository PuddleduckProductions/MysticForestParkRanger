using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;

namespace Interactions {
    using Behaviors;
    using static Interaction;
    /// <summary>
    /// Editor for <see cref="Interaction"/>, to make selecting <see cref="InteractionBehavior"/> automated and easy for designers.
    /// TODO: Need to figure out how to access the serialized properties of InteractionBehavior.
    /// </summary>
    [CustomEditor(typeof(Interaction))]
    [CanEditMultipleObjects]
    public class InteractionEditor : Editor {
        SerializedProperty behavior;

        private void OnEnable() {
            behavior = serializedObject.FindProperty(nameof(Interaction.behavior));
        }

        /// <summary>
        /// Assign our <see cref="Interaction.behavior"/> based on the given <see cref="Interaction.type"/>.
        /// </summary>
        /// <param name="interaction">The parent to give our <see cref="InteractionBehavior"/></param>
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
        }
    }
}