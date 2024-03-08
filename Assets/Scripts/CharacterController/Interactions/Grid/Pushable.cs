using Interactions.Behaviors;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;

namespace Interactions.Behaviors {
    /// <summary>
    /// Push an object around.
    /// </summary>
    [Serializable, InteractionType("Grid/Pushable")]
    public class PushableInteraction : InteractionBehavior {
        [SerializeField, HideInInspector]
        GridObject gridObject;
        public PushableInteraction(Interaction parent) : base(parent) {
            if (parent.TryGetComponent(out GridObject o)) {
                gridObject = o;
            } else {
                gridObject = parent.gameObject.AddComponent<GridObject>();
            }
        }
        /// <summary>
        /// Whether player is pushing. 
        /// </summary>
        protected bool isPushing;
        /// <summary>
        /// Our reference to the player.
        /// </summary>
        GameObject player;
        /// <summary>
        /// Our reference to the player's character controller script.
        /// </summary>
        Character.characterController controller;

        /// <summary>
        /// makes sure player is intentionally moving (mostly for controllers). 
        /// </summary>
        public float moveThreshold = 0.01f;

        /// <summary>
        /// How fast the object should be pushed.
        /// </summary>
        public float pushSpeed = 5f;

        /// <summary>
        /// Whether or not the player is currently allowed to push
        /// </summary>
        protected bool pushEnabled = true;

        /// <summary>
        /// FMOD event reference
        /// </summary
        //public EventInstance dragSound;
        [SerializeField]
        public EventReference dragSoundRef;
        [SerializeField]
        [Range(0, 2)]
        public int materialType;

        Vector3 groundOffset = Vector3.zero;
        Vector3 playerGroundOffset = Vector3.zero;

        float pushableGetGround(Vector3 inPos) {
            if(NavMesh.SamplePosition(inPos, out NavMeshHit hit, 10.0f, NavMesh.AllAreas)) {
                return hit.position.y;
            }

            Debug.LogError("Could not get ground.");
            return inPos.y;
        }

        /// <summary>
        /// Set ourselves to push, and hook into the interaction system to get when space is pressed again (to <see cref="ReleasePush(bool)"/>
        /// </summary>
        public override void Interact() {
            if (gridObject == null) {
                Debug.LogError($"Not GridObject found on interaction {interactionObject.name}. Try pressing CTRL+ALT+R with this object selected to fix.");
                return;
            }
            if (!isPushing) {
                player = GameObject.FindGameObjectWithTag("Player");
                controller = player.GetComponent<Character.characterController>();
                isPushing = true;
                pushEnabled = true;

                controller.moveEnabled = false;

                groundOffset = interactionObject.transform.position - Vector3.up * pushableGetGround(interactionObject.transform.position);
                playerGroundOffset = player.transform.position - Vector3.up * pushableGetGround(player.transform.position);

            } else if (pushEnabled) { // Are we in the process of moving? Don't allow releasing push.
                                      // Force InteractionManager to call EndInteraction.
                isPushing = false;
            }
        }

        public override bool CanInteract(Interaction other = null) {
            return base.CanInteract(other) && gridObject != null;
        }

        public override void EndInteraction() {
            controller.moveEnabled = true;
        }

        void ResetPushing(Vector3 originalPos, Vector3 originalPlayerPos, Transform transformToMove) {
            isPushing = false;
            pushEnabled = false;
            transformToMove.position = originalPos;
            player.transform.position = originalPlayerPos;
        }

        // Since Coroutines can't be run from non MonoBehaviours.
        protected static IEnumerator PushUpdate(PushableInteraction p, Transform transformToMove, Vector3 toAdd, Vector2Int dirToMove) {
            p.player.GetComponent<PlayerAnimator>().UpdatePush(dirToMove);

            var targetPos = p.gridObject.transform.position + toAdd;
            targetPos = new Vector3(targetPos.x, p.pushableGetGround(targetPos), targetPos.z);


            var playerTargetPos = p.player.transform.position + toAdd;
            playerTargetPos = new Vector3(playerTargetPos.x, p.pushableGetGround(playerTargetPos), playerTargetPos.z);

            var originalPos = transformToMove.position;
            var originalPlayerPos = p.player.transform.position;

            float timer = 0;
            while (timer < 1f) {
                transformToMove.position = Vector3.Lerp(originalPos, targetPos, timer);
                p.player.transform.position = Vector3.Lerp(originalPlayerPos, playerTargetPos, timer);
                timer += Time.fixedDeltaTime * p.pushSpeed;
                yield return new WaitForFixedUpdate();
                var colliders = Physics.OverlapSphere(p.player.transform.position, 1f);
                var interactionId = p.interactionObject.gameObject.GetInstanceID();
                foreach (var collider in colliders) {
                    var colliderID = collider.gameObject.GetInstanceID();
                    var parentId = collider.transform.parent.gameObject.GetInstanceID();
                    if (!collider.isTrigger && collider.tag != "Navmesh" && 
                        interactionId != colliderID && interactionId != parentId && p.player.gameObject.GetInstanceID() != colliderID) {
                        p.ResetPushing(originalPos, originalPlayerPos, transformToMove);
                        yield break;
                    }
                }
            }
            transformToMove.position = targetPos;
            p.player.transform.position = playerTargetPos;

            // Wait for the next update to roll around before resetting our pushing ability.
            yield return new WaitForEndOfFrame();
            // Finish by updating the actual grid position:
            p.gridObject.Move(dirToMove);

            p.player.GetComponent<PlayerAnimator>().UpdatePush(Vector2Int.zero);
            if (p.gridObject == null) {
                p.isPushing = false;
            }
            p.pushEnabled = true;
        }

        protected void Push(Vector3 dir) {
            pushEnabled = false;

            // Because gridObject may get destroyed while we're moving it: 
            Transform gridObjectTransform = gridObject.transform;

            dir = gridObject.GetGridDirectionFromWorld(dir);

            var dirToMove = new Vector2Int(0, 0);
            var x = Mathf.Abs(dir.x);
            var z = Mathf.Abs(dir.z);
            if (x > z) {
                dirToMove.x = Mathf.RoundToInt(dir.x);
            } else {
                dirToMove.y = Mathf.RoundToInt(dir.z);
            }

            var target = gridObject.GetSomeAdjacent(dirToMove);
            var start = gridObject.GetSomeAdjacent(Vector2Int.zero);
            if (target is Vector3 t && start is Vector3 s && dirToMove != Vector2Int.zero && gridObject.MoveIsValid(dirToMove)) {
                interactionObject.StartCoroutine(PushUpdate(this, gridObjectTransform, t - s, dirToMove));
                //FMOD
                AudioManager.Instance.PlayOneShotWithParameters("dragSound", dragSoundRef, "materialType", (float)materialType);
            } else {
                pushEnabled = true;
            }
        }

        /// <summary>
        /// Update the pushed object to move with us.
        /// </summary>
        public override bool Update() {
            if (pushEnabled && controller.intendedMove.magnitude > moveThreshold) {
                Push(controller.intendedMove);
            }
            return isPushing;
        }
    }
}