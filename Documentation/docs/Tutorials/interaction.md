# Interaction System

## Adding Interactions
Here it is, in a nutshell:
1. Create a new game object.
2. Add the "Interaction" component.
3. Select the interaction type you want to use.
4. Fill out any relevant information in the "Behavior" portion of the Inspector. If there's nothing there, it just means that there's nothing you can edit.

![Interaction Component](/assets/images/interactions/interactionComponent.png)

Each of the interaction types you could use are described under [Interaction.Behaviors](~/api/Interactions.Behaviors.yml). Check the documentation there for the possibilities.

### Creating your own interactions
You have two options. Custom interactions, or going into the code and adding your own [InteractionBehavior in Interactions.Behaviors](~/api/Interactions.Behaviors.yml).

#### Custom Interactions
WIP. Not finished. Will be sometime this Sprint!

#### InteractionBehavior
You add your own scripted InteractionBehavior thusly:

1. Create a subclass of [InteractionBehavior](~/api/Interactions.Behaviors.InteractionBehavior.yml)
2. Go to [InteractionEditor](~/api/Interactions.InteractionEditor.yml) and under `CreateBehavior`, add your custom type.

## Setting up Interactions
If whatever scene you're working in doesn't have the spacebar appearing over interactions, you need to add interactions.

There's a quick prefab for you: the UI prefab in Assets/Scripts/CharacterController. Drag that in along with PlayerObjects, and assuming you have a 

![UI prefab](/assets/images/interactions/interactionUI.png)