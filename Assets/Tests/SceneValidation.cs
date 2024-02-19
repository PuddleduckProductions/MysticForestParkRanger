using System.Collections;
using System.Collections.Generic;
using InkTools;
using Interactions.Behaviors;
using Interactions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SceneValidation {
    public class SceneValidation {
        // TODO: This needs to be a play mode test.
        [Test]
        public void ValidateInkInteractions() {
            var interactions = GameObject.FindObjectsOfType<Interaction>();
            foreach (var interaction in interactions) {
                if (interaction.behavior.GetType() == typeof(InkInteraction)) {
                    InkInteraction i = (InkInteraction)interaction.behavior;
                    var manager = GameObject.FindFirstObjectByType<InkManager>();
                    Assert.IsNotNull(manager);
                    Assert.IsTrue(manager.PathExists(i.inkKnot));
                }
            }
        }
        // A Test behaves as an ordinary method
        [Test]
        public void SceneValidationSimplePasses() {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator SceneValidationWithEnumeratorPasses() {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}