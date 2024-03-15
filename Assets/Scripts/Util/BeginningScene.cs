using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BeginningScene : MonoBehaviour
{
    // Start is called before the first frame update
    public string level_1;
    void OnEnable()
    {
        SceneManager.LoadScene(level_1);
        // Fix any pausing issues:
        Time.timeScale = 1;
    }
}
