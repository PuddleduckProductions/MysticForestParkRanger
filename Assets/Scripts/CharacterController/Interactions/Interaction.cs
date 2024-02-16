using System;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using InkTools;
using System.Reflection;
using Interactions.Behaviors;
using Ink.Parsed;
using System.Collections.Generic;
using static Cell;

namespace Interactions {
    namespace Behaviors {
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
        [Serializable]
        public class InkInteraction : InteractionBehavior {
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
                    setup = true;
                } else if (InkManager.storyActive) {
                    ISingleton<InkManager>.Instance.AdvanceStory();
                }
            }

            public override bool Update() {
                return InkManager.storyActive;
            }
        }

        /// <summary>
        /// Push an object around.
        /// </summary>
        [Serializable]
        public class PushableInteraction : InteractionBehavior {
            public PushableInteraction(Interaction parent) : base(parent) { }
            Interaction parent;
            /// <summary>
            /// Whether player is pushing. 
            /// </summary>
            protected bool isPushing;
            /// <summary>
            /// Whether player is in pushing/pulling "mode". Active until space is pressed.
            /// </summary>
            protected bool inPushMode;
            /// <summary>
            /// Our reference to the player.
            /// </summary>
            GameObject player;
            /// <summary>
            /// Our reference to the player's character controller script.
            /// </summary>
            Character.characterController controller;

            public Cell parentCell;
            /// <summary>
            /// Stored offset between the player and pushed object.
            /// </summary>
            Vector3 offset;
            /// <summary>
            /// makes sure player is intentionally moving (mostly for controllers). 
            /// </summary>

            public float moveThreshold = 0.01f;
            /// <summary>
            /// Weight of the Interactable Object, used with the CharacterController's
            /// pushForce and movementSpeed to determine speed.
            /// </summary>
            public float weight = 1f;

            /// <summary>
            /// distance between every push start and ending position.
            /// </summary>
            public float cellLength = 10f;

            /// <summary>
            /// Time to wait inbetween pushes.
            /// </summary>
            public float pushCoolDown = 1f;
            float lastPushTime = -Mathf.Infinity;
            private float startTime;
            private float pushTime;
            private Vector3 playerTargetPosition;            
            private Vector3 playerStartPosition;

            /// <summary>
            /// Set ourselves to push, and hook into the interaction system to get when space is pressed again (to <see cref="ReleasePush(bool)"/>
            /// </summary>
            public override void Interact() {
                if (!inPushMode) {
                    player = GameObject.FindGameObjectWithTag("Player");
                    controller = player.GetComponent<Character.characterController>();
                    if (parentCell == null){
                        inPushMode = false;
                        Debug.LogWarning("Make sure your item is in a Grid (create a grid game object and attach the grid group component to it!)");
                        return; //needs to be in grid in order to be pushable
                    } 
                    inPushMode = true;

                    // TODO: Should be recursive.
                    if (interactionObject.TryGetComponent<Collider>(out Collider c)) {
                        c.enabled = false;
                    }

                } else {
                    // Force InteractionManager to call EndInteraction.
                    inPushMode = false;
                }

            }

            public override void EndInteraction() {
                //SnapToGrid(interactionObject.transform);
                if (interactionObject.TryGetComponent<Collider>(out Collider c)) {
                    c.enabled = true;
                }
                controller.moveEnabled = true;
            }
            /// <summary>
            /// Snap the object's position to the center of the nearest grid point
            /// (currently unused)
            /// </summary>
            private void SnapToGrid(Transform objTransform)
            {
                Vector3 position = objTransform.position;
                //position.x = Mathf.Round(position.x / gridSize) * gridSize;
                //position.z = Mathf.Round(position.z / gridSize) * gridSize;
                objTransform.position = position;
            }   
            //checks to see if we can start changing values to COMMENCE pushing movement
            public void activatePush(){
                Vector2 direction = controller.input.normalized;
                //here is where check push would occur to make sure the cell you're going towards is valid 
                GridGroup grid = parentCell.parent;
                Cell newCell = grid.cellInDirection(parentCell, direction);
                if(newCell == null) return;
                
                grid.moveObjFromAToB(parentCell, newCell);
                parentCell = newCell;
                initPushMovement();

            }
            //sets values related to pushing movement
            //IF CHAR CONTROLLER CONTROLS PUSHING MOVEMENT LOGIC HERE WILL BE MOVED THERE
            public void initPushMovement(){ //once checks have been made, inits values for push
                isPushing = true; // we're now pushing!
                controller.animator.SetBool("walking", isPushing); //FOR FUTURE CHANGE TO PUSHING OR HANDLE MOVEMENT IN CHAR CONTROLLER
                Vector3 objTargetPos = parentCell.pos;
                offset = interactionObject.transform.position - player.transform.position;
                playerStartPosition = player.transform.position;
                playerTargetPosition = new Vector3(objTargetPos.x - offset.x, playerStartPosition.y, objTargetPos.z - offset.z);
                
                pushTime = cellLength / (controller.movementSpeed * (controller.pushForce / weight)); //for lerping
                startTime = Time.time;
            }
            //updates obj & player positions!
            //IF CHAR CONTROLLER CONTROLS PUSHING MOVEMENT LOGIC HERE WILL BE MOVED THERE
            public void updatePush(){
                controller.moveEnabled = false;
                float elapsedTime = Time.time - startTime;

                if (elapsedTime < pushTime){
                    player.transform.position = Vector3.Lerp(playerStartPosition, playerTargetPosition, elapsedTime / pushTime);
                    interactionObject.transform.position = player.transform.position + offset;
                }
                else
                {
                    // ensure player & obj are at target pos when done
                    player.transform.position = playerTargetPosition;
                    interactionObject.transform.position = player.transform.position + offset;
                    
                    isPushing = false; // stop pushing once movement is complete
                    controller.animator.SetBool("walking", isPushing);

                    controller.moveEnabled = true;
                }
            }

            /// <summary>
            /// Update the pushed object to move with us.
            /// </summary>
            public override bool Update() {
                if(inPushMode && isPushing){
                    controller.moveEnabled = false;
                    updatePush();
                }
                if(inPushMode && !isPushing){
                    controller.moveEnabled = false;
                    if (controller.input.magnitude> moveThreshold || controller.input.magnitude < -moveThreshold) {
                        if(Time.time - lastPushTime >= pushCoolDown) activatePush();
                    }
                }
                return inPushMode;
            }
        }

        /// <summary>
        /// Custom interaction setup. Define your own interaction behavior through other scripts.
        /// Called once.
        /// TODO: Updates.
        /// </summary>
        [Serializable]
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
        
        /// <summary>
        /// An object that can be picked up and placed within the world.
        /// </summary>
        [Serializable]
        public class PickAndPutInteraction : InteractionBehavior {
            public PickAndPutInteraction(Interaction parent) : base(parent) { }
            protected bool isPicking = false;

            GameObject player;
            Collider playerCollider;
            Collider c;
            Bounds colliderBounds;

            public override void Interact() {
                if (!isPicking) {
                    player = GameObject.FindGameObjectWithTag("Player");
                    isPicking = true;
                    interactionObject.interactionEnabled = false;
                    c = interactionObject.GetComponent<Collider>();
                    playerCollider = player.GetComponent<Collider>();
                    colliderBounds = c.bounds;
                    c.enabled = false;
                } else {
                    // Will force InteractionManager to call EndInteraction.
                    isPicking = false;
                }
            }

            public override void EndInteraction() {
                interactionObject.transform.position = player.transform.position + player.transform.forward * (0.5f + 
                    Mathf.Max(
                    Vector3.Dot(Vector3.forward, colliderBounds.extents), 
                    playerCollider.bounds.size.z
                    ));
                // Doesn't work rn because of player.
                if (Physics.Raycast(interactionObject.transform.position, Vector3.down, out RaycastHit hit)) {
                    interactionObject.transform.position += new Vector3(0, (hit.point.y - interactionObject.transform.position.y) + colliderBounds.size.y/2);
                }
                interactionObject.interactionEnabled = true;
                c.enabled = true;
            }


            public override bool Update() {
                interactionObject.transform.position = player.transform.position + new Vector3(0, playerCollider.bounds.size.y);
                interactionObject.transform.rotation = player.transform.rotation;
                return isPicking;
            }
        }

        /// <summary>
        /// An object where a <see cref="PickAndPutInteraction"/> can be placed.
        /// TODO: Not finished, needs to be choosy.
        /// </summary>
        [Serializable]
        public class PutTrigger : InteractionBehavior {
            public PutTrigger(Interaction parent) : base(parent) { }

            /// <summary>
            /// Calls with the object that just interacted with this trigger. Use this to define custom place behavior.
            /// </summary>
            [Tooltip("Calls with the object that just interacted with this trigger. Use this to define custom place behavior.")]
            public UnityEvent<GameObject> onChained = new UnityEvent<GameObject>();

            /// <summary>
            /// A list of interactions with tags that are allowed to interact with this placement.
            /// </summary>
            [Tooltip("A list of interactions with tags that are allowed to interact with this placement.")]
            public List<string> allowedTags = new List<string>();

            public override void Interact() {}

            public override void Interact(Interaction other) {
                onChained.Invoke(other.gameObject);
                // FIXME: Probably a more elegant solution based on what we want later on.
                // This is fine for now.
                ISingleton<InteractionManager>.Instance.StopCurrentInteraction();
                GameObject.Destroy(other.gameObject);
            }

            public override bool CanInteract(Interaction other = null) {
                if (other == null) return false;
                foreach (var tag in allowedTags) {
                    if (tag == other.tag) {
                        return true;
                    }
                }
                return false;
            }
        }
    } 
    //Require a Collider for the Boxcast system to reference
    [RequireComponent(typeof(Collider))]

    [HelpURL("https://puddleduckproductions.github.io/MysticForestParkRanger/docs/Tutorials/interaction.html")]
    public class Interaction : MonoBehaviour {
        public enum InteractionType { InkInteraction, PushableInteraction, PickAndPutInteraction, PutTrigger, CustomInteraction, ShowImageInteraction, WaterPipe };
        /// <summary>
        /// Should we allow interaction with this object?
        /// If this is set to false while <see cref="IsInteracting"/> is true,
        /// this will allow control over <see cref="InteractionManager.interactionButton"/>
        /// </summary>
        public bool interactionEnabled = true;
        public InteractionType type;
        [SerializeReference]
        public Behaviors.InteractionBehavior behavior;

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