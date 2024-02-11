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
            var pos = CameraHelper.WorldToVcamPoint(mainCamera, cameras[0], player.transform.position);
            foreach (var cam in cameras) {
                Debug.Log($"{cam.name}: {pos} {mainCamera.WorldToScreenPoint(player.transform.position)}");
                // TODO: Project theoretical camera here and get if there is an overlap.
                cam.Priority = -(int)Vector3.Distance(player.transform.position, cam.transform.position);
                //var collider = cam.GetComponent<CinemachineCollider>();
            }
        }
    }
}
