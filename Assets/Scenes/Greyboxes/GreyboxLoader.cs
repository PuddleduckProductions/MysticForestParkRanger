using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GreyboxLoader : MonoBehaviour
{
    public void LoadLevel(string name) {
        SceneManager.LoadScene(name);
        // Fix any pausing issues:
        Time.timeScale = 1;
    }

    public void Quit() {
        Application.Quit();
    }
}
