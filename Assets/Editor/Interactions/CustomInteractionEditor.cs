using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Interactions {
    using Behaviors;
    using Codice.CM.SEIDInfo;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(CustomInteraction))]
    public class CustomInteractionDrawer : PropertyDrawer {

        bool showFoldout = true;

        float baseHeight;

        int selectedFunc = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return baseHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            baseHeight = 20f;
            showFoldout = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, position.width, baseHeight), showFoldout, "Custom Behavior");
            if (showFoldout) {
                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                //EditorGUI.indentLevel = 0;


                var targetObject = property.FindPropertyRelative("targetObject");

                //EditorGUI.BeginProperty(position, new GUIContent("OnInteract"), property.FindPropertyRelative("onInteract"));

                position = new Rect(position.x, position.y + 20, position.width, 20);
                baseHeight += 20;

                EditorGUI.ObjectField(position, targetObject);
                var obj = targetObject.objectReferenceValue;
                if (obj != null) {
                    if (EditorUtility.IsPersistent(obj)) {
                        Debug.LogError("TargetObject must be located within the scene.");
                        targetObject.objectReferenceValue = null;
                    } else {
                        position = new Rect(position.x, position.y + 20, position.width, 20);
                        var prefix = EditorGUI.PrefixLabel(position, new GUIContent("Method"));

                        List<string> methods = new List<string>();;
                        var objectMethods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Where(method => method.ReturnParameter.ParameterType == typeof(bool) && 
                            method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(CustomInteraction));
                        foreach (var method in objectMethods) {
                            methods.Add(method.Name);
                        }

                        baseHeight += 20;

                        EditorGUI.Popup(prefix, selectedFunc, methods.ToArray());
                    }
                }

                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndFoldoutHeaderGroup();

            //EditorGUILayout.LabelField("Test");

            //targetObject.objectReferenceValue = EditorGUILayout.ObjectField("Target Object", targetObject.objectReferenceValue, typeof(UnityEngine.Object), true);
        }
    }
}
