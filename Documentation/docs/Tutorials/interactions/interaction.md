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
2. Try selecting the interactions that are giving you errors, and press CTRL+ALT+R (or Puddleduck->Interactions->Refresh Selected Interactions). That should refresh any data that hasn't been updated. This will clear any changes you have made though, so you will have to input that data again.
3. Are there multiple main cameras in a scene? (There should only be one)

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

There's a quick prefab for you, the `PlayArea` scene. Hit `CTRL+N` and select it to get started.

You should then see something like this:
![UI prefab](~/assets/images/interactions/interactionUI.png)

### Put Trigger
This will allow any interaction to interact with it, so long as it's being held by the player (i.e., there's an interaction whose Update function is being called and returning true). It will only show as valid if the currently held interaction has a tag from the `Allowed Tags` list. When space is pressed with a current interaction on this one, the other interaction is destroyed, and `onChained` is called. You can set `onChained` to do whatever you want, and it takes a `GameObject` as a parameter.

Want some more functionality? Maybe being able to let you use something with a put trigger without destroying what you're holding? Let me know. I'll move it higher up on my priorities.