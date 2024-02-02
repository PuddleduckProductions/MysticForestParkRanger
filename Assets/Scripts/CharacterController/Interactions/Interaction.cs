using System;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Interactions {
    /// <summary>
    /// A serialized class meant to control different interaction behaviors when space is pressed on one.
    /// This is to avoid having to attach multiple monobehaviors for anything with one interaction.
    /// If you want to add your own, you can either use Custom (in progress), or create a subclass of InteractionBehavior.
    /// </summary>
    [Serializable]
    public abstract class InteractionBehavior {
        /// <summary>
        /// Reference to the MonoBehaviour <see cref="Interaction"/> for accessing things like position.
        /// It's a serialized field so we don't lose the reference to the parent when the scene starts.
        /// </summary>
        [SerializeField]
        protected Interaction interactionObject;

        public InteractionBehavior(Interaction parent) {
            interactionObject = parent;
        }

        public abstract bool isInteracting { get; }
        public abstract void Interact();
        public virtual void Update() { }

        /*public virtual SerializedProperty[] SerializedProperties(SerializedObject serializedObject) {
            return new SerializedProperty[] { };
        }*/
        public virtual void OnInspectorGUI() { }

    }

    [Serializable]
    public class InkInteraction : InteractionBehavior {
        public InkInteraction(Interaction parent) : base(parent) { }

        public override bool isInteracting => InkManager.storyActive;
        public override void Interact() {
            ISingleton<InkManager>.Instance.StartDialog("interact_" + interactionObject.name);
            ISingleton<UIController>.Instance.onInteract.AddListener(InteractAdvance);
            ISingleton<InkManager>.Instance.dialogEnd.AddListener(EndDialog);
        }

        public void EndDialog() {
            ISingleton<UIController>.Instance.onInteract.RemoveListener(InteractAdvance);
        }

        public void InteractAdvance(bool pressed) {
            if (pressed && InkManager.storyActive) {
                ISingleton<InkManager>.Instance.AdvanceStory();
            }
        }
    }

    [Serializable]
    public class PushableInteraction : InteractionBehavior {
        public PushableInteraction(Interaction parent) : base(parent) { }

        public override bool isInteracting => isPushing;

        protected bool isPushing;
        GameObject player;

        Vector3 offset;

        public override void Interact() {
            player = GameObject.FindGameObjectWithTag("Player");
            isPushing = true;
            ISingleton<UIController>.Instance.onInteract.AddListener(ReleasePush);
            offset = interactionObject.transform.position - player.transform.position;
        }

        protected void ReleasePush(bool pressed) {
            if (pressed) {
                isPushing = false;
                ISingleton<UIController>.Instance.onInteract.RemoveListener(ReleasePush);
            }
        }

        public override void Update() {
            interactionObject.transform.position = player.transform.position + offset;
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
        /*public override SerializedProperty[] SerializedProperties(SerializedObject serializedObject) {
            SerializedProperty behavior = serializedObject.FindProperty("behavior");
            return new[] { serializedObject.FindProperty("onInteract") };
        }*/

        public override void OnInspectorGUI() {
            //EditorGUILayout.PropertyField("Test");
        }
    }

    public class Interaction : MonoBehaviour {
        public enum InteractionType { Ink, Pushable, Custom };
        public InteractionType type;
        [SerializeReference]
        public InteractionBehavior behavior;

        public bool HasInteractionBehavior() {
            return behavior != null;
        }

        public bool IsInteracting() {
            return behavior.isInteracting;
        }

        public void Interact() {
            behavior.Interact();
        }

        private void Update() {
            if (behavior != null && IsInteracting()) {
                behavior.Update();
            }
        }
    }
}