using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utility {
    [Serializable]
    public class SerializedMethod : ISerializationCallbackReceiver {
        /// <summary>
        /// Definition for what sort of functions will be allowed.
        /// Can be used multiple times.
        /// Set in <see cref="Utility.SerializedMethodEditor"/>
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, AllowMultiple=true)]
        public class MethodValidation : Attribute {
            public Type returnType;
            public Type[] parameters;
            /// <summary>
            /// Set what types of methods are supported.
            /// Set in <see cref="Utility.SerializedMethodEditor"/>
            /// </summary>
            /// <param name="returnType">The type this function returns.</param>
            /// <param name="parameters">The parameters this function takes.</param>
            public MethodValidation(Type returnType, Type[] parameters) {
                this.returnType = returnType;
                this.parameters = parameters;
            }
        }

        [SerializeField, HideInInspector]
        protected string methodName;

        [SerializeField]
        protected UnityEngine.Object targetObject;
        protected MethodInfo methodInfo;

        public bool IsNull() {
            return methodInfo == null || methodName == " " || methodName == null || targetObject == null;
        }

        public void SetMethod(MethodInfo info, UnityEngine.Object target) {
            targetObject = target;
            methodInfo = info;
            Debug.Assert(target.GetType() == info.ReflectedType, "Invalid MethodInfo, does not exist in target.");
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
            return methodInfo.Invoke(targetObject, parameters);
        }

        public void OnBeforeSerialize() {
            if (methodInfo != null) {
                methodName = methodInfo.Name;
            }
        }

        public void OnAfterDeserialize() {
            if (methodName != "" && targetObject != null) {
                methodInfo = targetObject.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }
        }


    }
}
