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
    //[SerializeField] Slider musicVolume;
    //[SerializeField] Slider sfxVolume;

    GameObject menu;
    InputSystemUIInputModule uiInput;
    
    // Start is called before the first frame update
    void Start()
    {
        ((ISingleton<PauseManager>)this).Initialize();
        menu = transform.GetChild(0).gameObject;
        uiInput = GetComponentInParent<InputSystemUIInputModule>();
        //EventSystem.current.SetSelectedGameObject(sfxVolume.gameObject);
        //EventSystem.current.SetSelectedGameObject(musicVolume.gameObject);
    }
    
    private void Update()
    {
        if (uiInput.cancel.action.triggered)
        {
            Resume();
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

    //public bool isAnyVolChanged()
    //{
    //    var sfxSlider = GameObject.Find("SFX Slider").GetComponent<Slider>();
    //    var musSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
    //    if (sfxVol != sfxSlider.value / sfxSlider.maxValue | musVol != musSlider.value / musSlider.maxValue)
    //    {
    //        return true;
    //    }
    //    return false;
    //}


    //public void UpdateVolume()
    //{
    //    var sfxSlider = GameObject.Find("SFX Slider").GetComponent<Slider>();
    //    sfxVol = sfxSlider.value / sfxSlider.maxValue;
    //    AudioManager.Instance.SetSfxVolume(sfxVol);
    //    var musSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
    //    musVol = musSlider.value / musSlider.maxValue;
    //    AudioManager.Instance.SetMusicVolume(musVol);

    //    PlayerPrefs.SetFloat("volume", (sfxSlider.value / sfxSlider.maxValue));
    //    PlayerPrefs.Save();
    //}

    public void SetLanguage(string language = "English"){
        if(language == "Japanese"){
            // Set language to Japanese
        }
        else{
            // Set language to English
        }
    }
}
