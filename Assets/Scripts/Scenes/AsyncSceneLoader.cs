using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour
{
    public string sceneToLoad;
    public Vector3 offset;
    public Vector3 rotation;

    private IEnumerator SceneLoading() {
        var loader = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        yield return loader;
        var scene = SceneManager.GetSceneByName(sceneToLoad);
        SceneManager.SetActiveScene(scene);
        var objs = scene.GetRootGameObjects();
        var parent = new GameObject("TransformParent");
        foreach (var obj in objs ) {
            if (obj.name == "PlayerObjects" || obj.name == "UI") {
                obj.SetActive(false);
            }
            obj.transform.SetParent(parent.transform);
        }
        parent.transform.position += offset;
        parent.transform.rotation = Quaternion.Euler(rotation);
    }
    private void OnTriggerEnter(Collider other) {
        StartCoroutine(SceneLoading());
    }
}