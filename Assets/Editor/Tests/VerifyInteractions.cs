using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class VerifyInteractions
{

    [SetUp]
    public void Setup() {
        EditorSceneManager.OpenScene("Assets/Scenes/Prototype Zero/PrototypeZero.unity");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [Test]
    public void ValidateInteractions()
    {
        var interactions = GameObject.FindObjectsOfType<Interaction>();
        foreach (var interaction in interactions) {
            Assert.IsTrue(interaction.HasInteractionBehavior());
        }
    }

    [TearDown]
    public void Teardown() {
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }
}
