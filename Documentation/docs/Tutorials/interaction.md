# Interaction System

## Adding Interactions
Here it is, in a nutshell:
1. Create a new game object.
2. Add the "Interaction" component.

### Creating your own interactions
You have two options. Custom interactions, or going into the code and adding your own [InteractionBehavior](~/api/Interactions.Behaviors.yml).

#### Custom Interactions
WIP. Not finished. Will be sometime this Sprint!

#### InteractionBehavior
You add your own scripted InteractionBehavior thusly:

1. Create a subclass of [InteractionBehavior](~/api/Interactions.Behaviors.InteractionBehavior.yml)
2. Go to [InteractionEditor](~/api/Interactions.InteractionEditor.yml) and under `CreateBehavior`, add your custom type.

## Setting up Interactions
If whatever scene you're working in doesn't have the spacebar appearing over interactions, you need to add interactions.

TODO: This.