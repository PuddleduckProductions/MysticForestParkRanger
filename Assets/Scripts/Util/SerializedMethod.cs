using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utility {
    [Serializable]
    public class SerializedMethod : ISerializationCallbackReceiver {
        [SerializeField, HideInInspector]
        protected string methodName;

        [SerializeField, HideInInspector]
        protected UnityEngine.Object owningObject;
        protected MethodInfo methodInfo;

        public bool IsNull() {
            return methodInfo == null;
        }

        public void SetMethod(MethodInfo info, UnityEngine.Object target) {
            owningObject = target;
            methodInfo = info;
            Debug.Assert(target.GetType() == info.ReflectedType, "Invalid MethodInfo, does not exist in target.");
        }

        public static void SetMethod(UnityEditor.SerializedProperty prop, string methodName, UnityEngine.Object target) {
            prop.FindPropertyRelative("methodName").stringValue = methodName;
            prop.FindPropertyRelative("owningObject").objectReferenceValue = target;
        }

        public ParameterInfo[] parameters {
            get {
                if (!IsNull()) {
                    return methodInfo.GetParameters();
                } else {
                    return new ParameterInfo[0];
                }
            }
        }

        public object Invoke(object[] parameters) {
            var infoParams = methodInfo.GetParameters();
            Debug.Assert(parameters.Length == infoParams.Length, 
                "Invalid Number of Parameters passed through. Expected: " + infoParams.Length + ". Got: " + parameters.Length);
            for (int i = 0; i < parameters.Length; i++) {
                Debug.Assert(parameters[i].GetType() == infoParams[i].GetType(), 
                    "Invalid parameter at position " + i + ". Expected type " + infoParams[i].GetType() + ". Got type " + parameters[i].GetType());
            }
            return methodInfo.Invoke(owningObject, parameters);
        }

        public void OnBeforeSerialize() {
            if (methodInfo != null) {
                methodName = methodInfo.Name;
            }
        }

        public void OnAfterDeserialize() {
            if (methodName != "" && owningObject != null) {
                methodInfo = owningObject.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }
        }


    }
}
