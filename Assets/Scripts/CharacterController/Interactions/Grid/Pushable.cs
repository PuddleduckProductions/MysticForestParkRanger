using Interactions.Behaviors;
using System;
using System.Collections;
using UnityEngine;

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

        Vector3 groundOffset = Vector3.zero;
        Vector3 playerGroundOffset = Vector3.zero;

        Vector3 pushableGetGround(Vector3 inPos) {
            RaycastHit[] hits = Physics.RaycastAll(inPos, Vector3.down);
            foreach (var hit in hits) {
                if (hit.collider.gameObject != interactionObject.gameObject) {
                    return hit.point;
                }
            }
            Debug.LogError("Could not get ground.");
            return inPos;
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

                groundOffset = interactionObject.transform.position - pushableGetGround(interactionObject.transform.position);
                playerGroundOffset = player.transform.position - pushableGetGround(player.transform.position);

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

        bool isPlayerColliding = false;
        void PlayerCollision() {
            if (!pushEnabled) {
                isPlayerColliding = true;
            }
        }

        // Since Coroutines can't be run from non MonoBehaviours.
        protected static IEnumerator PushUpdate(PushableInteraction p, Transform transformToMove, Vector3 toAdd, Vector2Int dirToMove) {
            var targetPos = p.gridObject.transform.position + toAdd;

            // FIXME: This. It's not a great solution for snapping to ground.
            // Should be navmesh.
            var ground = p.pushableGetGround(targetPos + 2 * Vector3.up) + p.groundOffset;
            var groundDist = ground - targetPos;
            targetPos += groundDist;


            var playerTargetPos = p.player.transform.position + toAdd;
            var playerGround = p.pushableGetGround(playerTargetPos) + p.playerGroundOffset;
            var playerGroundDist = playerGround - playerTargetPos;
            playerTargetPos += playerGroundDist;

            var originalPos = transformToMove.position;
            var originalPlayerPos = p.player.transform.position;

            float timer = 0;
            while (timer < 1f) {
                transformToMove.position = Vector3.Lerp(originalPos, targetPos, timer);
                p.player.transform.position = Vector3.Lerp(originalPlayerPos, playerTargetPos, timer);
                timer += Time.fixedDeltaTime * p.pushSpeed;
                yield return new WaitForFixedUpdate();
                var colliders = Physics.OverlapSphere(p.player.transform.position, 1f);
                foreach (var collider in colliders) {
                    var colliderID = collider.gameObject.GetInstanceID();
                    if (!collider.isTrigger && collider.tag != "Ground" && 
                        p.interactionObject.gameObject.GetInstanceID() != colliderID && p.player.gameObject.GetInstanceID() != colliderID) {
                        p.isPushing = false;
                        p.pushEnabled = false;
                        transformToMove.position = originalPos;
                        p.player.transform.position = originalPlayerPos;
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
            if (target is Vector3 t && start is Vector3 s && dirToMove != Vector2Int.zero) {
                interactionObject.StartCoroutine(PushUpdate(this, gridObjectTransform, t - s, dirToMove));
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