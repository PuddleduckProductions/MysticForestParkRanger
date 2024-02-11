using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Utility;

namespace Character {
    public class CameraController : MonoBehaviour {
        [SerializeField]
        CinemachineVirtualCamera[] cameras;

        [SerializeField]
        CinemachineVirtualCamera playerCamera;

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
                // TODO: Project theoretical camera here and get if there is an overlap.
                //var collider = cam.GetComponent<CinemachineCollider>();
            }
        }
    }
}
