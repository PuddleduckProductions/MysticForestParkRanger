using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Interactions {
    using Behaviors;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using Utility;

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
                EditorGUI.indentLevel = 0;

                position = new Rect(position.x, position.y + 20, position.width, 30);
                baseHeight += 30;
                var onInteract = property.FindPropertyRelative("onInteract");
                EditorGUI.SelectableLabel(position, onInteract.tooltip);

                position = new Rect(position.x, position.y + 30, position.width, 100);
                baseHeight += 100;
                EditorGUI.PropertyField(position, onInteract);

                var targetObject = property.FindPropertyRelative("targetObject");

                position = new Rect(position.x, position.y + 100, position.width, 20);
                baseHeight += 20;

                var onUpdate = property.FindPropertyRelative("onUpdate");
                EditorGUI.LabelField(position, new GUIContent("OnUpdate()", onUpdate.tooltip));
                EditorGUI.indentLevel += 1;

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

                        List<GUIContent> methods = new List<GUIContent>();;
                        var objectMethods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Where(CustomInteraction.ValidateUpdateFunc);

                        methods.Add(new GUIContent("No Function"));

                        foreach (var method in objectMethods) {
                            var paramString = "";
                            foreach (var param in method.GetParameters()) {
                                paramString += param.ToString();
                            }
                            methods.Add(new GUIContent($"{method.ReflectedType.Name} {method.ReturnType} {method.Name}({paramString})"));
                        }

                        baseHeight += 20;

                        EditorGUI.BeginChangeCheck();

                        selectedFunc = EditorGUI.Popup(position, new GUIContent("Method", "Method to call"), selectedFunc, methods.ToArray());

                        if (EditorGUI.EndChangeCheck()) {
                            if (selectedFunc > 0) {
                                MethodInfo info = objectMethods.ElementAt(selectedFunc - 1);

                                SerializedMethod.SetMethod(onUpdate, info.Name, obj);

                                //var methodStore = (SerializedMethod) onUpdate.managedReferenceValue;

                                //methodStore.SetMethod(info, obj);
                            }
                        }
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
