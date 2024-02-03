using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utility {
    [Serializable]
    public class SerializedMethod<T> : ISerializationCallbackReceiver {
        [SerializeField, HideInInspector]
        protected string methodName;

        protected Type owningObject;
        protected MethodInfo methodInfo;

        public void SetMethod(MethodInfo info) {
            Assert.IsTrue(typeof(T) == info.ReflectedType, "Method " + info.Name + " does not belong to " + typeof(T).FullName);
            methodInfo = info;
        }

        public void Invoke(T target, object[] parameters) {
            methodInfo.Invoke(target, parameters);
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
