using System.Collections;
using InkTools;
using Interactions.Behaviors;
using Interactions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace SceneValidation {
    [TestFixture]
    [TestFixtureSource(typeof(PlayScenesProvider))]
    public class SceneValidation {
        protected string scenePath;

        public SceneValidation(string scenePath) {
            this.scenePath = scenePath;
        }

        [OneTimeSetUp]
        public void SetUp() {
            SceneManager.LoadScene(this.scenePath);
        }

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
    }

    public class PlayScenesProvider : IEnumerable {
        public IEnumerator GetEnumerator() {
            foreach (var scene in EditorBuildSettings.scenes) {
                if (!scene.enabled || scene.path == null) {
                    continue;
                }

                yield return scene.path;
            }
        }
    }
}