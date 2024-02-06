# Interaction System

## Adding Interactions
Here it is, in a nutshell:
1. Create a new game object.
2. Add the "Interaction" component.
3. Select the interaction type you want to use.
4. Fill out any relevant information in the "Behavior" portion of the Inspector. If there's nothing there, it just means that there's nothing you can edit.

![Interaction Component](~/assets/images/interactions/interactionComponent.png)

Each of the interaction types you could use are described under [Interaction.Behaviors](~/api/Interactions.Behaviors.yml). Check the documentation there for the possibilities.

### Bugs
Right now the system isn't fully tested and can have some bugs. Some things to test:
1. If you open Unity Tests (Window->General->Test Runner), do all of the VerifyInteraction tests (under Edit Mode) return success?
2. Are there multiple main cameras in a scene? (There should only be one)

If you find a bug, please report it in [issues](https://github.com/PuddleduckProductions/MysticForestParkRanger/issues) and label it to programming!

### Creating your own interactions
You have two options. Custom interactions, or going into the code and adding your own [InteractionBehavior in Interactions.Behaviors](~/api/Interactions.Behaviors.yml).

#### Custom Interactions
If you want to add a one-off custom interaction, go ahead! You can select CustomInteraction, and then define some custom behavior to use: [Interactions.Behaviors.CustomInteraction](~/api/Interactions.Behaviors.CustomInteraction.yml).

If you want something a more long-term solution (i.e., re-usable behaviors) read on:

#### InteractionBehavior
You add your own scripted InteractionBehavior thusly:

1. Create a subclass of [InteractionBehavior](~/api/Interactions.Behaviors.InteractionBehavior.yml)
2. Go to [Interaction.InteractionType](~/api/Interactions.Interaction.yml#InteractionType) and add your class name there as an enumerator.
3. Your class should now appear in the dropdown.

## Setting up Interactions
If whatever scene you're working in doesn't have the spacebar appearing over interactions, you need to add interactions.

There's a quick prefab for you: the UI prefab in Assets/Scripts/CharacterController. Drag that in along with PlayerObjects, and assuming you have only one Camera tagged "MainCamera", you should be good to go.

![UI prefab](~/assets/images/interactions/interactionUI.png)