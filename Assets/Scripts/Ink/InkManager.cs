using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.Text.RegularExpressions;

public class InkManager : MonoBehaviour
{
    [SerializeField]
    protected TextAsset inkJSONAsset;

    protected Story story;
    bool storyActive = false;

    #region Dialog Parameters
    [SerializeField]
    protected GameObject dialog;
    protected GameObject dialogInstance;
    protected DialogRenderer dialogRenderer;
    public struct DialogLine {
        public string character { get; }
        public string dialog { get; }
        public DialogLine(string character, string dialog) {
            this.character = character;
            this.dialog = dialog;
        }
    }

    static protected Regex lineMatchRegex;
    static protected DialogLine LineFromString(string text) {
        const string lineMatch = @"(?:(?<characterName>[\w\W]*):)?(?<dialog>.*)";
        if (lineMatchRegex == null) {
            lineMatchRegex = new Regex(lineMatch);
        }

        Match match = lineMatchRegex.Match(text);
        return new DialogLine(match.Groups["characterName"].Value, match.Groups["dialog"].Value);
    }
    #endregion


    void Awake() {
        story = new Story(inkJSONAsset.text);
        dialogInstance = GameObject.Instantiate(dialog, transform);
        dialogRenderer = dialogInstance.GetComponent<DialogRenderer>();
        dialogRenderer.Init();
        dialogInstance.SetActive(false);
    }

    #region Flow Control
    public void StartDialog(string name) {
        // Not sure if there's a better way to test this.
        if (!storyActive) {
            story.ChoosePathString(name);
            EvaluateStory();
            storyActive = true;
        } else {
            Debug.LogWarning("Dialog already in progress.");
        }
    }

    public void AdvanceStory() {
        if (story.canContinue) {
            EvaluateStory();
        } else {
            storyActive = false;
            dialogInstance.SetActive(false);
        }
    }

    protected void EvaluateStory()
    {
        string text = story.Continue();
        Debug.Log(text);
        DialogLine line = LineFromString(text);
        DrawDialog(line);
    }
    #endregion

    #region Dialog Rendering
    protected bool TryFindCharacter(string name, out GameObject character)
    {
        character = GameObject.Find(name);
        if (character == null) {
            Debug.LogWarning("Could not find Character " + name);
        }
        return character == null;
    }

    protected void DrawDialog(DialogLine line) {
        dialogRenderer.Render(line);
        dialogInstance.SetActive(true);
    }
    #endregion
}
