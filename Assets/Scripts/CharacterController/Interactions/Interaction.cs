using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(Interaction))]
[CanEditMultipleObjects]
public class InteractionEditor : Editor {
    SerializedProperty customInteractionProp;

    private void OnEnable() {
        customInteractionProp = serializedObject.FindProperty("customInteraction");
    }

    public override void OnInspectorGUI() {
        Interaction interaction = (Interaction)target;
        base.DrawDefaultInspector();
        //interaction.type = (Interaction.InteractionType)EditorGUILayout.EnumPopup("Interaction Type", interaction.type);
        if (interaction.type == Interaction.InteractionType.Custom) {
            EditorGUILayout.PropertyField(customInteractionProp, new GUIContent("onInteract"));
        }
    }
}

public class Interaction : MonoBehaviour
{
    public enum InteractionType { Ink, Pushable, Custom };
    public InteractionType type;
    [HideInInspector, SerializeField]
    protected UnityEvent onInteract = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        switch (type) {
            case InteractionType.Ink:
                onInteract.AddListener(Ink);
            break;
            case InteractionType.Pushable:
                onInteract.AddListener(Push);
            break;
        }
    }

    public void Interact() {
        onInteract.Invoke();
    }

    protected void Push() {

    }

    protected void Ink() {
        InkManager.startDialog.Invoke("interact_" + name);
    }
}
