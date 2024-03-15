using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class Settings : MonoBehaviour
{

    public float sfxVol = 1f;
    public float musVol = 1f;

    void Awake() {
        var lang = PlayerPrefs.GetString("language");
        switch (lang) {
            case "English":
                GameObject.Find("English").GetComponent<Button>().onClick.Invoke();
            break;
            case "Japanese":
                GameObject.Find("Japanese").GetComponent<Button>().onClick.Invoke();
            break;
        }
        var volume = 1f;

        if (PlayerPrefs.HasKey("volume")) {
            volume = PlayerPrefs.GetFloat("volume");
        }
        var slider = GameObject.Find("SFX Slider").GetComponent<Slider>();
        slider.value = volume * slider.maxValue;
        UpdateVolume();
    }

    public void Update()
    {
        if (isAnyVolChanged())
        {
            UpdateVolume();
        }
    }

    public bool isAnyVolChanged()
    {
        var sfxSlider = GameObject.Find("SFX Slider").GetComponent<Slider>();
        var musSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
        if (sfxVol != sfxSlider.value / sfxSlider.maxValue | musVol != musSlider.value / musSlider.maxValue)
        {
            return true;
        }
        return false;
    }


    public void UpdateVolume()
    {
        var sfxSlider = GameObject.Find("SFX Slider").GetComponent<Slider>();
        AudioManager.Instance.SetSfxVolume(sfxSlider.value / sfxSlider.maxValue);
        var musSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
        AudioManager.Instance.SetMusicVolume(musSlider.value / musSlider.maxValue);

        PlayerPrefs.SetFloat("volume", (sfxSlider.value / sfxSlider.maxValue));
        PlayerPrefs.Save();

    }

    public void UpdateLanguage(string language) {
        PlayerPrefs.SetString("language", language);
        PlayerPrefs.Save();
    }
}
