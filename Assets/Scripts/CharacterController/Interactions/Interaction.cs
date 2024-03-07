using System;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using InkTools;
using System.Reflection;
using FMOD.Studio;
using FMODUnity;

namespace Interactions {
    namespace Behaviors {
        /// <summary>
        /// Attribute for InteractionBehavior to add it to <see cref="Interactions.InteractionEditor"/>
        /// </summary>
        public class InteractionType : Attribute {
            public string path;
            /// <summary>
            /// Attribute for the editor to see and show up on the dropdown.
            /// </summary>
            /// <param name="path">The path to show as on the dropdown.</param>
            public InteractionType(string path) {
                this.path = path;
            }
        }

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
            [SerializeField, HideInInspector]
            protected Interaction interactionObject;

            public InteractionBehavior(Interaction parent) {
                interactionObject = parent;
            }

            /// <summary>
            /// Function to call when the object is interacted with (i.e., Space is pressed)
            /// Called by <see cref="InteractionManager"/>.
            /// This will still be called even while <see cref="Update"/> returns true, as long as this object is being
            /// interacted with and space is being pressed.
            /// </summary>
            public abstract void Interact();

            /// <summary>
            /// Function to call when the object is interacted with another interaction.
            /// This will still be called even while <see cref="Update"/> returns true, as long as this object is being
            /// interacted with and space is being pressed on another object.
            /// </summary>
            /// <param name="other">The other interaction</param>
            public virtual void Interact(Interaction other) { }

            /// <summary>
            /// Called during <see cref="Interaction.Start"/>.
            /// </summary>
            public virtual void GameObjectStart() { }

            /// <summary>
            /// Are we currently having an interaction happen? What do we need to do to update it?
            /// If this returns true, this supresses all other interactions in the scene.
            /// </summary>
            public virtual bool Update() { return false; }

            /// <summary>
            /// What <see cref="InteractionManager"/> calls when we stop interacting for whatever reason.
            /// Use this to clean up. You can trigger this yourself when you return false in <see cref="Update"/>,
            /// but this may be called by other interactions who want to do interactions with your object in <see cref="InteractionManager.StopCurrentInteraction"/>
            /// </summary>
            public virtual void EndInteraction() { }

            /// <summary>
            /// Should we allow interactions currently?
            /// Default setting is as long as you're not holdign something else.
            /// </summary>
            /// <param name="other">The other object that wants to interact with us. Can be null.</param>
            /// <returns></returns>
            public virtual bool CanInteract(Interaction other=null) { return other == null;  }
        }

        /// <summary>
        /// For displaying dialog in Ink. By default, loads `interact_Name` knot in Ink. Can be changed.
        /// Tests check to see if the selected knot exists. If you get an error on tests regarding an invalid knot, this is why.
        /// </summary>
        [Serializable, InteractionType("Misc/Ink")]
        public class InkInteraction : InteractionBehavior {

            //FMOD character voices
            //Set these in the inspector
            [SerializeField]
            public EventReference characterVoiceRef;
            [SerializeField]
            [Range(0, 3)]
            public int emotionType;


            /// <summary>
            /// Knot to start on interaction.
            /// </summary>
            public string inkKnot;
            public InkInteraction(Interaction parent) : base(parent) {
                inkKnot = $"interact_{parent.name}";
            }

            bool setup = false;

            /// <summary>
            /// Call interact_<see cref="UnityEngine.Object.name"/> in Ink.
            /// </summary>
            public override void Interact() {
                if (!setup) {
                    ISingleton<InkManager>.Instance.StartDialog(inkKnot);
                    
                    //FMOD
                    AudioManager.Instance.PlayOneShotWithParameters("characterVoice", characterVoiceRef, "emotionType", (float)emotionType);

                    setup = true;
                } else if (InkManager.storyActive) {
                    ISingleton<InkManager>.Instance.AdvanceStory();
                    //FMOD
                    AudioManager.Instance.PlayOneShotWithParameters("characterVoice", characterVoiceRef, "emotionType", (float)emotionType);
                }
            }

            public override bool Update() {
                return InkManager.storyActive;
            }
        }

        /// <summary>
        /// Custom interaction setup. Define your own interaction behavior through other scripts.
        /// Called once.
        /// TODO: Updates.
        /// </summary>
        [Serializable, InteractionType("Misc/Custom")]
        public class CustomInteraction : InteractionBehavior {

            public CustomInteraction(Interaction parent) : base(parent) { }

            /// <summary>
            /// Functions to call when space is pressed on this object.
            /// Called when space is pressed for the first time, as well as
            /// every time <see cref="onUpdate"/> returns true and space is pressed.
            /// </summary>
            [SerializeField]
            [Tooltip("Functions to call when space is pressed\non this object. Called when space is pressed for " +
                " the first time, as well as every time onUpdate returns true and space is pressed.")]
            protected UnityEvent onInteract = new UnityEvent();

            /// <summary>
            /// Set in <see cref="Interactions.CustomInteractionEditor"/>
            /// Called every frame.
            /// Should return a boolean as to whether or not the object is still being interacted with.
            /// While returning true, the object will still be interacted with.
            /// Can take <see cref="Interaction"/> as an optional argument.
            /// Validated in <see cref="ValidateUpdateFunc(MethodInfo)"/>
            /// </summary>
            [SerializeField, Tooltip("Called every frame. " +
                "Should return a boolean as to whether or not the object is still being interacted with. " +
                "While returning true, the object will still be interacted with. Can take Interaction as an optional argument.")]
            [SerializedMethod.MethodValidation(typeof(bool), new Type[] { }),
                SerializedMethod.MethodValidation(typeof(bool), new Type[] { typeof(Interaction) })]
            public SerializedMethod onUpdate = new SerializedMethod();

            public override void Interact() {
                onInteract.Invoke();
            }

            /// <summary>
            /// Used in <see cref="Interactions.CustomInteractionEditor"/> to validate functions.
            /// </summary>
            /// <param name="func">The function to validate.</param>
            /// <returns>Whether or not <see cref="Update"/> will be able to call this function.</returns>
            public static bool ValidateUpdateFunc(MethodInfo func) {
                return func.ReturnParameter.ParameterType == typeof(bool) &&
                    (func.GetParameters().Length == 0 || 
                    func.GetParameters().Length == 1 && func.GetParameters()[0].ParameterType == typeof(Interaction));
            }

            protected bool CallUpdate() {
                if (!onUpdate.IsNull()) {
                    var parameters = onUpdate.parameters;
                    if (parameters.Length == 0) {
                        return (bool)onUpdate.Invoke(new object[0]);
                    } else {
                        return (bool)onUpdate.Invoke(new object[] { this.interactionObject });
                    }
                }
                return false;
            }

            public override bool Update() {
                return CallUpdate();
            }
        }
    }

    //Require a Collider for the Boxcast system to reference
    [RequireComponent(typeof(Collider))]

    [HelpURL("https://puddleduckproductions.github.io/MysticForestParkRanger/docs/Tutorials/interaction.html")]
    public class Interaction : MonoBehaviour {
        /// <summary>
        /// Should we allow interaction with this object?
        /// If this is set to false while <see cref="IsInteracting"/> is true,
        /// this will allow control over <see cref="InteractionManager.interactionButton"/>
        /// </summary>
        public bool interactionEnabled = true;
        [SerializeReference]
        public Behaviors.InteractionBehavior behavior;

        [HideInInspector]
        public string behaviorType;

        private void Start() {
            if (HasInteractionBehavior()) {
                behavior.GameObjectStart();
            }
        }

        public bool HasInteractionBehavior() {
            return behavior != null;
        }

        public void Interact() {
            behavior.Interact();
        }

        public void Interact(Interaction other) {
            behavior.Interact(other);
        }

        public void EndInteraction() {
            behavior.EndInteraction();
        }

        public void OnDestroy() {
            behavior = null;
        }

        public bool CanInteract(Interaction other=null) {
            return behavior.CanInteract(other);
        }

        /// <summary>
        /// Is this interaction still being used?
        /// </summary>
        /// <returns>Whether or not to keep using this interaction</returns>
        public bool InteractionUpdate() {
            if (HasInteractionBehavior()) {
                return behavior.Update();
            }
            return false;
        }
    }
}