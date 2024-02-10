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
        float baseHeight;

        int selectedFunc = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return baseHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            label.text += "()";
            EditorGUI.BeginProperty(position, label, property);
            baseHeight = 20f;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            position = new Rect(position.x, position.y, position.width, 20f);
            EditorGUI.LabelField(position, label);
            // Don't make child fields be indented
            //var indent = EditorGUI.indentLevel;
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

                    List<GUIContent> methods = new List<GUIContent>();

                    var attrs = fieldInfo.GetCustomAttributes(typeof(SerializedMethod.MethodValidation));
                    var objectMethods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(info => {
                            foreach (var attr in attrs) {
                                if (attr is SerializedMethod.MethodValidation s) {
                                    if (s.returnType != info.ReturnType) {
                                        return false;
                                    }
                                    var parameters = info.GetParameters();
                                    if (parameters.Length != s.parameters.Length) {
                                        return false;
                                    }

                                    for (int i = 0; i < parameters.Length; i++) {
                                        if (parameters[i].ParameterType != s.parameters[i]) {
                                            return false;
                                        }
                                    }
                                    return true;
                                }
                            }
                            Debug.LogError("SerializedMethod.MethodValidation attribute not found for " + property.name);
                            return false;
                        });

                    methods.Add(new GUIContent("No Function"));

                    int i = 1;
                    foreach (var method in objectMethods) {
                        var paramString = "";
                        foreach (var param in method.GetParameters()) {
                            paramString += param.ToString();
                        }

                        if (method.Name == property.FindPropertyRelative("methodName").stringValue) {
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

                            SetMethod(property, info.Name, obj);

                            //var methodStore = (SerializedMethod) onUpdate.managedReferenceValue;

                            //methodStore.SetMethod(info, obj);
                        } else {
                            SetMethod(property, null, obj);
                        }
                    }
                }
            }
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            //EditorGUILayout.LabelField("Test");

            //targetObject.objectReferenceValue = EditorGUILayout.ObjectField("Target Object", targetObject.objectReferenceValue, typeof(UnityEngine.Object), true);
        }


        public void SetMethod(UnityEditor.SerializedProperty prop, string methodName, UnityEngine.Object target) {
            var methodProp = prop.FindPropertyRelative("methodName");
            if (methodName == null) {
                // Stupid workaround since I cant null the stringValue property this way.
                methodProp.stringValue = " ";
            } else {
                methodProp.stringValue = methodName;
            }
            prop.FindPropertyRelative("targetObject").objectReferenceValue = target;
        }
    }
}