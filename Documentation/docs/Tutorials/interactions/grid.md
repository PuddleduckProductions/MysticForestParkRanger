# The Grid System

For anything involving a grid (right now just pushing and pulling), you need two things.

## One: a Grid Group component.
![Grid System](~/assets/images/interactions/grid.png)

You configure this like you would any grid, and it will update you to show you how your grid is configured.

- RED means that the space is occupied.
- BLACK means that the space is empty.
- BLUE represents the grid origin (i.e.: 0,0).

There are yellow squares at the center of each empty space (apart from the origin, since I don't want to change the colors there). If you click on one, it will change the square to RED to indicate that the space is now blocked. Click it again to change it back to an empty space.

ALSO: There is a dimension for y-values, but it remains unused. X and Z are the only world coordinates used right now, and Y should interpret based on collisions in the future.

Y-axis may be added later? Not sure yet.

## Two: A child game object with a Pushable Interaction 
This should be a child of the GridGroup component.

![Grid Object](~/assets/images/interactions/interaction.md)

If you have a pushable interaction component that doesn't also have the `GridObject` component, press CTRL+ALT+R with the object selected. It should refresh and add the component.

If you then re-select the Grid Group object, you should see how your object fits within the grid. Feel free to adjust the offset, dimensions, size, and spacing of the grid until it looks right.

You should be able to go up and push things now. Pushable interactions without the GridGroup component as their direct parent will not function.