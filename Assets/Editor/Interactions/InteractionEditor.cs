using UnityEditor;

namespace Interactions {
    using Behaviors;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using static Interaction;
    /// <summary>
    /// Editor for <see cref="Interaction"/>, to make selecting <see cref="InteractionBehavior"/> automated and easy for designers.
    /// TODO: Need to figure out how to access the serialized properties of InteractionBehavior.
    /// </summary>
    [CustomEditor(typeof(Interaction))]
    [CanEditMultipleObjects]
    public class InteractionEditor : Editor {
        SerializedProperty behavior;
        SerializedProperty type;

        private void OnEnable() {
            behavior = serializedObject.FindProperty(nameof(Interaction.behavior));
            type = serializedObject.FindProperty(nameof(Interaction.behaviorType));
        }

        /// <summary>
        /// Assign our <see cref="Interaction.behavior"/> based on the selection from the menu.
        /// </summary>
        /// <param name="name">The name of the behavior we're creating</param>
        /// <param name="interaction">The parent to give our <see cref="InteractionBehavior"/></param>
        /// <param name="interactionType">The type of <see cref="InteractionBehavior"/> to use.</param>
        /// <param name="so">Serialized object to apply changes to.</param>
        public static void CreateBehavior(string name, Interaction interaction, System.Type interactionType, SerializedObject so) {
            ConstructorInfo constructor = interactionType.GetConstructors()[0];
            if (constructor != null) {
                so.FindProperty(nameof(Interaction.behavior)).managedReferenceValue = constructor.Invoke(new object[] { interaction });
                so.FindProperty(nameof(Interaction.behaviorType)).stringValue = name;
            }
            so.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            Interaction interaction = (Interaction)target;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(type.displayName);
            if (EditorGUILayout.DropdownButton(new GUIContent(type.stringValue == "" ? "None" : type.stringValue), FocusType.Keyboard)) {
                var enumEditor = new GenericMenu();

                enumEditor.AddItem(new GUIContent("None"), type.stringValue == "", () => {
                    behavior.managedReferenceValue = null;
                    type.stringValue = "";
                    serializedObject.ApplyModifiedProperties();
                });

                var behaviorAssembly = typeof(InteractionBehavior).Assembly;
                var subTypes = behaviorAssembly.GetTypes().Where(t => typeof(InteractionBehavior).IsAssignableFrom(t));
                foreach (var subType in subTypes) {
                    var customAttrs = subType.GetCustomAttributes();
                    if (customAttrs.Count() > 0) {
                        var interactionTypes = customAttrs.Where(a => a is InteractionType);
                        if (interactionTypes.Count() > 0) {
                            InteractionType interactionType = (InteractionType)interactionTypes.First();
                            enumEditor.AddItem(new GUIContent(interactionType.path), type.stringValue == interactionType.path, () => {
                                CreateBehavior(interactionType.path, interaction, subType, serializedObject);
                            });
                        }
                    }
                }
                enumEditor.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            //InteractionBehavior behavior = 
            base.DrawDefaultInspector();
        }
    }
}