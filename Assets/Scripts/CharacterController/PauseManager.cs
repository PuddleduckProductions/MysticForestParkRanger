using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using Utility;

public class PauseManager : MonoBehaviour, ISingleton<PauseManager>
{
    public bool isPaused { get; private set; }
    [SerializeField] Slider musicVolume;
    [SerializeField] Slider sfxVolume;
    
    GameObject menu;
    InputSystemUIInputModule uiInput;
    
    // Start is called before the first frame update
    void Start()
    {
        ((ISingleton<PauseManager>)this).Initialize();
        menu = transform.GetChild(0).gameObject;
        uiInput = GetComponentInParent<InputSystemUIInputModule>();
        EventSystem.current.SetSelectedGameObject(sfxVolume.gameObject);
    }
    
    private void Update()
    {
        if (uiInput.cancel.action.triggered)
        {
            Resume();
        }
        if(isPaused){
            // Set muisc volume to musicVolume.normalizedValue;
            // Set SFX volume to sfxVolume.normalizedValue;
            UpdateVolume();
        }
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
    
    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateVolume()
    {
        var slider = GameObject.Find("SFX Slider").GetComponent<Slider>();
        var volume = slider.value / slider.maxValue;
        AudioManager.Instance.volume = volume;
        AudioManager.Instance.setVolume(volume);
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }

    public void SetLanguage(string language = "English"){
        if(language == "Japanese"){
            // Set language to Japanese
        }
        else{
            // Set language to English
        }
    }
}
