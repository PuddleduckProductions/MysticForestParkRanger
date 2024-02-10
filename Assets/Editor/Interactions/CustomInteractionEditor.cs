using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Interactions {
    using Behaviors;
    [CustomEditor(typeof(CustomInteraction))]
    [CanEditMultipleObjects]
    public class CustomInteractionEditor : Editor {
        //SerializedProperty onUpdate;
        public void OnEnable() {
            //onUpdate = serializedObject.FindProperty("onUpdate");
        }
        public override void OnInspectorGUI() {
            base.DrawDefaultInspector();
            //EditorGUILayout.ObjectField();
        }
    }
}
