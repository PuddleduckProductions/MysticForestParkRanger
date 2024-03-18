using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

public class Settings : MonoBehaviour
{

    [SerializeField] Slider musicVolume;
    [SerializeField] Slider sfxVolume;

    public float sfxVol = 1f;
    public float musVol = 1f;

    public List<GameObject> englishObjects;
    public List<GameObject> japaneseObjects;

    void Awake() {
        var lang = PlayerPrefs.GetString("language");
        switch (lang) {
            case "English":
                GameObject.Find("English").GetComponent<Button>().onClick.Invoke();
                UpdateLanguage(lang);
                break;
            case "Japanese":
                GameObject.Find("Japanese").GetComponent<Button>().onClick.Invoke();
                UpdateLanguage(lang);
                break;
            default:
                UpdateLanguage("English");
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

    private void Start() {
        gameObject.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(sfxVolume.gameObject);
        //EventSystem.current.SetSelectedGameObject(musicVolume.gameObject);
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
        sfxVol = sfxSlider.value / sfxSlider.maxValue;
        AudioManager.Instance.SetSfxVolume(sfxVol);
        var musSlider = GameObject.Find("Music Slider").GetComponent<Slider>();
        musVol = musSlider.value / musSlider.maxValue;
        AudioManager.Instance.SetMusicVolume(musVol);

        PlayerPrefs.SetFloat("volume", (sfxSlider.value / sfxSlider.maxValue));
        PlayerPrefs.Save();
    }

    public void UpdateLanguage(string language) {
        PlayerPrefs.SetString("language", language);

        foreach (var obj in englishObjects) {
            obj.gameObject.SetActive(language == "English");
        }
        foreach (var obj in japaneseObjects) {
            obj.gameObject.SetActive(language == "Japanese");
        }
        PlayerPrefs.Save();
    }
}
