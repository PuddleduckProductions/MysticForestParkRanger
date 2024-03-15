using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PauseManager : MonoBehaviour, ISingleton<PauseManager>
{
    public bool isPaused { get; private set; }
    GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        ((ISingleton<PauseManager>)this).Initialize();
        menu = transform.GetChild(0).gameObject;
    }

    public void Pause() {
        isPaused = true;
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    public void Resume() {
        isPaused = false;
        Time.timeScale = 1;
        menu.SetActive(false);
    }
}
