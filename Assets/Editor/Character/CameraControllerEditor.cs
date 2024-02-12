using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Cinemachine;
using System.Linq;

namespace Character {
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor {
        SerializedProperty camerasProp;
        SerializedProperty playerCameraProp;
        SerializedProperty playerProp;

        public void OnEnable() {
            camerasProp = serializedObject.FindProperty("cameras");
            playerCameraProp = serializedObject.FindProperty("playerCamera");
            playerProp = serializedObject.FindProperty("player");
            UpdateCams();
        }

        public override void SaveChanges() {
            base.SaveChanges();
            UpdateCams();
        }

        protected void UpdateCams() {
            Debug.Log("[CameraController] Updating Cameras...");
            serializedObject.Update();
            var cams = GameObject.FindObjectsOfType<CinemachineVirtualCamera>(true);
            int i = 0;
            camerasProp.arraySize = 0;
            foreach (var cam in cams) {
                if (cam.Name != "PlayerCamera") {
                    if (i >= camerasProp.arraySize) {
                        camerasProp.arraySize++;
                    }
                    camerasProp.GetArrayElementAtIndex(i).objectReferenceValue = cam;
                    i++;
                } else {
                    playerCameraProp.objectReferenceValue = cam;
                }
            }
            playerProp.objectReferenceValue = GameObject.FindGameObjectWithTag("Player");
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Update Cameras List")) {
                UpdateCams();
            }
        }


    }
}