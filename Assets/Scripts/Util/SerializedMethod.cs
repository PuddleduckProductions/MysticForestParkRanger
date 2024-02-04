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

        protected Type owningObject;
        protected MethodInfo methodInfo;

        public bool IsNull() {
            return methodInfo == null;
        }

        public void SetMethod(MethodInfo info) {
            methodInfo = info;
        }

        public object Invoke(UnityEngine.Object target, object[] parameters) {
            Debug.Assert(target.GetType() == methodInfo.ReflectedType, "Method " + methodInfo.Name + " does not belong to " + target.GetType().FullName);
            return methodInfo.Invoke(target, parameters);
        }

        public void OnBeforeSerialize() {
            if (methodInfo != null) {
                methodName = methodInfo.Name;
                owningObject = methodInfo.ReflectedType;
            }
        }

        public void OnAfterDeserialize() {
            if (methodName != "" && owningObject != null) {
                methodInfo = owningObject.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            }
        }


    }
}
