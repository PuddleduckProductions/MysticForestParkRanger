using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public static class CameraHelper {
        public static Matrix4x4 WorldToCameraMatrixFromVcam(Camera camera, CinemachineVirtualCamera vCam) {
            // All we really need to do is translate z to -1 (with the OpenGL standard)
            // Per: https://docs.unity3d.com/ScriptReference/Camera-worldToCameraMatrix.html
            Matrix4x4 transform = Matrix4x4.TRS(Vector3.zero,
                Quaternion.identity, new Vector3(1, 1, -1));
            return transform * vCam.transform.worldToLocalMatrix;
        }

        public static Vector2 WorldToVcamPoint(Camera camera, CinemachineVirtualCamera vCam, Vector3 point) {
            var worldToClip = camera.projectionMatrix * WorldToCameraMatrixFromVcam(camera, vCam);

            Vector3 clip = worldToClip.MultiplyPoint(point);
            Rect viewport = camera.pixelRect;
            return new Vector2(viewport.x + (1.0f + clip.x) * viewport.width * 0.5f,
                viewport.y + (1.0f + clip.y) * viewport.height * 0.5f);
        }
    }
}
