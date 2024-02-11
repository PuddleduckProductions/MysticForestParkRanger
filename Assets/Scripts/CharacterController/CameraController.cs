using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Character {
    public class CameraController : MonoBehaviour {
        [SerializeField]
        CinemachineVirtualCamera[] cameras;

        [SerializeField]
        CinemachineVirtualCamera playerCamera;

        [SerializeField]
        GameObject player;
        

        private void Awake() {
            //nextCam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update() {
            foreach (var cam in cameras) {
                // TODO: Project theoretical camera here and get if there is an overlap.
                cam.Priority = -(int)Vector3.Distance(player.transform.position, cam.transform.position);
                //var collider = cam.GetComponent<CinemachineCollider>();
            }
        }
    }
}
