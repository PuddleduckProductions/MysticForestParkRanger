using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Cinemachine;
using System.Linq;

namespace Character {
    [CustomEditor(typeof(CameraController_EY))]
    public class CameraController_EYEditor : Editor {
        SerializedProperty camerasProp;
        SerializedProperty cameraZonesProp;
        SerializedProperty playerCameraProp;
        SerializedProperty playerProp;

        public void OnEnable() {
            camerasProp = serializedObject.FindProperty("cameras");
            cameraZonesProp = serializedObject.FindProperty("cameraZones");
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
            var zones = GameObject.FindObjectsOfType<CameraZone>(true);
            i = 0;
            cameraZonesProp.arraySize = 0;
            foreach (var zone in zones) {
                if (i >= cameraZonesProp.arraySize) {
                    cameraZonesProp.arraySize++;
                }
                cameraZonesProp.GetArrayElementAtIndex(i).objectReferenceValue = zone;
                i++;
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