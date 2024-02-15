using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Utility;
using System.Linq;

namespace Character {
    /// <summary>
    /// Looks at all active Cinemachine Cameras in the scene and sets priority.
    /// Cameras are updated in the inspector, press the "Update Cameras List" button when you add a new camera.
    /// </summary>
    public class CameraController_EY : MonoBehaviour {
        /// <summary>
        /// List of virtual cameras in the scene to switch between.
        /// TODO: Needs to work with loading.
        /// </summary>
        [SerializeField]
        CinemachineVirtualCamera[] cameras;
        [SerializeField]
        CameraZone[] cameraZones;

        /// <summary>
        /// The player camera, in case no other cameras are found.
        /// </summary>
        [SerializeField]
        CinemachineVirtualCamera playerCamera;
        [SerializeField]
        CinemachineVirtualCamera zoneCamera;

        /// <summary>
        /// The current camera we're using for render info.
        /// </summary>
        Camera mainCamera;

        [SerializeField]
        GameObject player;
        

        private void Awake() {
            mainCamera = Camera.main;
            //nextCam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update() {
            foreach (var cam in cameras) {
                var pos = CameraHelper.WorldToScreenPointVcam(mainCamera, cam, player.transform.position);
                // TODO: System for when the player gets obscured by objects
                if (pos.x > 0 && pos.x < mainCamera.pixelWidth && pos.y > 0 && pos.y < mainCamera.pixelHeight) {
                    cam.Priority = 20 - (int)Mathf.Log(Vector3.Distance(player.transform.position, cam.transform.position));
                } else {
                    cam.Priority = 0;
                }
                if(zoneCamera==cam){
                    cam.Priority=20;
                }
                // TODO: Project theoretical camera here and get if there is an overlap.
                //var collider = cam.GetComponent<CinemachineCollider>();
            }
        }
        
        private void OnTriggerStay(Collider other) {
            var zone = other.GetComponent<CameraZone>();
            // int ind = -1;
            
            if(zone){
                zoneCamera = zone.zoneCamera;
            }
        }
    }
}
