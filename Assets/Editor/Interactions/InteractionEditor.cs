using UnityEditor;

namespace Interactions {
    using Behaviors;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
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
        public static void CreateBehavior(Interaction interaction, SerializedProperty behavior, SerializedObject so) {
            var behaviorAssembly = typeof(InteractionBehavior).Assembly;
            var subTypes = behaviorAssembly.GetTypes().Where(t => typeof(InteractionBehavior).IsAssignableFrom(t));

            ConstructorInfo constructor = null;
            foreach (var subType in subTypes) {
                if (subType.Name == interaction.type.ToString()) {
                    constructor = subType.GetConstructors()[0];
                    break;
                }
            }
            if (constructor != null) {
                behavior.managedReferenceValue = constructor.Invoke(new object[] { interaction });
            }
            so.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            Interaction interaction = (Interaction)target;
            //InteractionBehavior behavior = 
            InteractionType initialType = interaction.type;
            base.DrawDefaultInspector();
            if (interaction.behavior == null || initialType != interaction.type) {
                CreateBehavior(interaction, behavior, serializedObject);
            }
        }
    }
}