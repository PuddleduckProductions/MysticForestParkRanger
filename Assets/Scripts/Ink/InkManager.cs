using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkManager : MonoBehaviour
{
    [SerializeField]
    protected TextAsset inkJSONAsset;

    protected Story story;

    protected Camera worldCamera;

    [SerializeField]
    protected GameObject dialog;
    // Start is called before the first frame update
    void Awake()
    {
        story = new Story(inkJSONAsset.text);
        worldCamera = Camera.main;
    }

    public void StartDialog(string name)
    {
        story.ChoosePathString(name);
        DrawDialog();
    }

    protected void DrawDialog()
    {
        
    }
}
