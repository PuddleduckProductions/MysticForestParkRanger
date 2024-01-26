using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using static Interaction;

[CustomEditor(typeof(Interaction))]
[CanEditMultipleObjects]
public class InteractionEditor : Editor {
    Dictionary<string, SerializedProperty> properties;

    //SerializedObject behavior;
    private void OnEnable() {
        //properties(interaction.behavior.SerializedProperties());

    }

    public override void OnInspectorGUI() {
        Interaction interaction = (Interaction)target;
        //InteractionBehavior behavior = 
        InteractionType initialType = interaction.type;
        base.DrawDefaultInspector();
        if (interaction.behavior == null || initialType != interaction.type) {
            switch (interaction.type) {
                case InteractionType.Ink:
                    interaction.behavior = new InkInteraction(interaction);
                    break;
                case InteractionType.Pushable:
                    interaction.behavior = new PushableInteraction(interaction);
                    break;
                case InteractionType.Custom:
                    interaction.behavior = new CustomInteraction(interaction);
                    break;
            }
        }
        //interaction.type = (Interaction.InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interaction.type);
        interaction.behavior.OnInspectorGUI();
    }
}

[Serializable]
public abstract class InteractionBehavior {
    // We serialize so we don't lose the parent reference:
    [SerializeField]
    protected Interaction interaction;

    public InteractionBehavior(Interaction parent) {
        interaction = parent;
    }

    public abstract bool isInteracting { get; }
    public abstract void Interact();
    public virtual void Update() { }

    public virtual SerializedProperty[] SerializedProperties(SerializedObject serializedObject) {
        return new SerializedProperty[] { };
    }
    public virtual void OnInspectorGUI() {}

}

[Serializable]
public class InkInteraction : InteractionBehavior {
    public InkInteraction(Interaction parent) : base(parent) {}

    public override bool isInteracting => InkManager.storyActive;
    public override void Interact() {
        InkManager.startDialog.Invoke("interact_" + interaction.name);
        UIController.onInteract.AddListener(InteractAdvance);
        InkManager.dialogEnd.AddListener(EndDialog);
    }

    public void EndDialog() {
        UIController.onInteract.RemoveListener(InteractAdvance);
    }

    public void InteractAdvance(bool pressed) {
        if (pressed && InkManager.storyActive) {
            InkManager.advanceStory.Invoke();
        }
    }
}

[Serializable]
public class PushableInteraction : InteractionBehavior {
    public PushableInteraction(Interaction parent) : base(parent) {}

    public override bool isInteracting => isPushing;

    protected bool isPushing;
    GameObject player;

    Vector3 offset;

    public override void Interact() {
        player = GameObject.FindGameObjectWithTag("Player");
        isPushing = true;
        UIController.onInteract.AddListener(ReleasePush);
        offset = interaction.transform.position - player.transform.position;
    }

    protected void ReleasePush(bool pressed) {
        if (pressed) {
            isPushing = false;
            UIController.onInteract.RemoveListener(ReleasePush);
        }
    }

    public override void Update() {
        interaction.transform.position = player.transform.position + offset;
    }
}

[Serializable]
public class CustomInteraction : InteractionBehavior {
    public CustomInteraction(Interaction parent) : base(parent) { }

    [SerializeField]
    protected UnityEvent onInteract = new UnityEvent();
    [SerializeField]
    protected string interactionFunc;

    public override bool isInteracting => throw new NotImplementedException();

    public override void Interact() {
        throw new NotImplementedException();
    }

    // TODO: Fix.
    public override SerializedProperty[] SerializedProperties(SerializedObject serializedObject) {
        SerializedProperty behavior = serializedObject.FindProperty("behavior");
        return new[] { serializedObject.FindProperty("onInteract") };
    }

    public override void OnInspectorGUI() {
        //EditorGUILayout.PropertyField("Test");
    }
}

public class Interaction : MonoBehaviour
{
    public enum InteractionType { Ink, Pushable, Custom };
    public InteractionType type;
    [HideInInspector, SerializeReference]
    public InteractionBehavior behavior;

    public bool IsInteracting() {
        return behavior.isInteracting;
    }

    public void Interact() {
        behavior.Interact();
    }

    private void Update() {
        if (IsInteracting()) {
            behavior.Update();
        }
    }
}
