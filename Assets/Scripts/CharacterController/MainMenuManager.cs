using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    // [SerializeField] GameObject levelMenu;
    [SerializeField] GameObject credits;

    [SerializeField] GameObject creditsReturnButton;

    [SerializeField] Slider musicVolume;
    [SerializeField] Slider sfxVolume;

    Action goBack;
    Button lastButton;
    InputSystemUIInputModule uiInput;

    void Start()
    {
        uiInput = FindObjectOfType<InputSystemUIInputModule>();
    }

    private void Update()
    {
        if (uiInput.cancel.action.triggered)
        {
            GoBack();
        }
    }

    public void OpenSettings(Button button)
    {
        lastButton = button;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(musicVolume.gameObject);
        goBack = () => { settingsMenu.SetActive(false); };
    }

    public void OpenCredits(Button button)
    {
        lastButton = button;
        mainMenu.SetActive(false);
        credits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(creditsReturnButton);
        goBack = () => { credits.SetActive(false); };
    }

    public void GoBack()
    {
        if (goBack != null)
        {
            goBack();
            goBack = null;
            mainMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(lastButton.gameObject);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public float GetMusicVolume()
    {
        return musicVolume.value;
    }

    public float GetSFXVolume()
    {
        return sfxVolume.value;
    }
}
