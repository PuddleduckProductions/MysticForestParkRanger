using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Interactions {
    using Behaviors;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(CustomInteraction))]
    public class CustomInteractionDrawer : PropertyDrawer {

        bool showFoldout = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 100;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            showFoldout = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, 20), showFoldout, "Custom Behavior");
            if (showFoldout) {
                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                //EditorGUI.indentLevel = 0;


                var targetObject = property.FindPropertyRelative("targetObject");

                //EditorGUI.BeginProperty(position, new GUIContent("OnInteract"), property.FindPropertyRelative("onInteract"));

                position = new Rect(position.x, position.y + 20, position.width, 20);

                EditorGUI.ObjectField(position, targetObject);
                var obj = targetObject.objectReferenceValue;
                if (obj != null) {
                    Debug.Assert(!EditorUtility.IsPersistent(obj), "TargetObject must be located within the scene.");
                    targetObject.objectReferenceValue = null;
                }

                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndFoldoutHeaderGroup();

            //EditorGUILayout.LabelField("Test");

            //targetObject.objectReferenceValue = EditorGUILayout.ObjectField("Target Object", targetObject.objectReferenceValue, typeof(UnityEngine.Object), true);
        }
    }
}
