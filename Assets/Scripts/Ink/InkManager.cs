using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class InkManager : MonoBehaviour, ISingleton<InkManager> {
    #region Story Management
    public bool runOnStart = false;

    public static bool storyActive { get; private set; }

    [SerializeField]
    protected TextAsset inkJSONAsset;

    protected Story story;
    #endregion

    InkCommands commands = new InkCommands();

    #region Dialog Parameters
    protected GameObject dialogInstance;
    protected DialogRenderer dialogRenderer;
    public struct DialogLine {
        public string character { get; }
        public string dialog { get; }
        public List<string> tags { get; }
        public DialogLine(string character, string dialog, List<string> tags) {
            this.character = character;
            this.dialog = dialog;
            this.tags = tags;
        }
    }

    static protected Regex lineMatchRegex;
    protected DialogLine LineFromString(string text) {
        const string lineMatch = @"(?:(?<characterName>[\w\W]*):)?(?<dialog>.*)";
        if (lineMatchRegex == null) {
            lineMatchRegex = new Regex(lineMatch);
        }

        Match match = lineMatchRegex.Match(text);
        return new DialogLine(match.Groups["characterName"].Value, match.Groups["dialog"].Value, story.currentTags);
    }
    #endregion

    public UnityEvent dialogEnd = new UnityEvent();

    void Awake() {
        story = new Story(inkJSONAsset.text);
        dialogInstance = transform.GetChild(0).gameObject;
        dialogRenderer = dialogInstance.GetComponent<DialogRenderer>();
        dialogRenderer.Init();
        // In case it's active:
        dialogInstance.SetActive(false);

        ISingleton<InkManager>.Initialize(this);

        if (runOnStart) {
            storyActive = true;
            AdvanceStory();
        }
    }

    #region Flow Control
    public void StartDialog(string name) {
        // Not sure if there's a better way to test this.
        if (!storyActive) {
            story.ChoosePathString(name);
            storyActive = true;
            AdvanceStory();
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
            dialogEnd.Invoke();
        }
    }

    protected void EvaluateStory() {
        string text = story.Continue();
        if (text.Length > 0 && text[0] == '$') {
            commands.Evaluate(text, story.currentTags);
            AdvanceStory();
        } else {
            DialogLine line = LineFromString(text);
            DrawDialog(line);
        }
    }

    private void Update() {
        commands.Update();
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
