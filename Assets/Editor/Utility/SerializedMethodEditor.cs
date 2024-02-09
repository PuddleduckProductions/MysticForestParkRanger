using Interactions.Behaviors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Utility {
    [CustomPropertyDrawer(typeof(SerializedMethod))]
    public class SerializedMethodEditor : PropertyDrawer {
        bool showFoldout = true;

        float baseHeight;

        // TODO: Pretty sure this is busted on serialization update.
        int selectedFunc = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return baseHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            baseHeight = 20f;
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

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

                    List<GUIContent> methods = new List<GUIContent>(); ;
                    var objectMethods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(CustomInteraction.ValidateUpdateFunc);

                    methods.Add(new GUIContent("No Function"));

                    int i = 1;
                    foreach (var method in objectMethods) {
                        var paramString = "";
                        foreach (var param in method.GetParameters()) {
                            paramString += param.ToString();
                        }
                        if (method.Name == onUpdate.FindPropertyRelative("methodName").stringValue) {
                            selectedFunc = i;
                        }
                        i++;
                        methods.Add(new GUIContent($"{method.ReflectedType.Name} - {method.Name}({paramString})"));
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

            //EditorGUILayout.LabelField("Test");

            //targetObject.objectReferenceValue = EditorGUILayout.ObjectField("Target Object", targetObject.objectReferenceValue, typeof(UnityEngine.Object), true);
        }
    }
}