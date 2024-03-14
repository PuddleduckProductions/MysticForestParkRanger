using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class Settings : MonoBehaviour
{
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
        var volume = 0.5f;

        if (PlayerPrefs.HasKey("volume")) {
            volume = PlayerPrefs.GetFloat("volume");
        }
        var slider = GameObject.Find("SFX Slider").GetComponent<Slider>();
        slider.value = volume * slider.maxValue;
        UpdateVolume();
    }

    public void UpdateVolume() {
        var slider = GameObject.Find("SFX Slider").GetComponent<Slider>();
        var volume =  slider.value/slider.maxValue;
        AudioManager.Instance.volume = volume;
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }

    public void UpdateLanguage(string language) {
        PlayerPrefs.SetString("language", language);
        PlayerPrefs.Save();
    }
}
