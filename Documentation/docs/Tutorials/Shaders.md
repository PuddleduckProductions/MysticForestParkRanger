# Shaders

We have a couple of custom shaders for our park ranger game. Here's the breakdown on how you can manipulate them in the inspector.

## Toon Shader

### What it does:
It's a rendering style designed to make 3D surfaces emulate 2D, flat surfaces. With customary cell shading to go along with it!

### How it works:
After a making a shader graph, we applied it to a Material. This material can now be added to 3D objects in the scene to give the effect. 

**WIP getting it to automatically/more easily apply to every object in the scene**

### Inspector Items Under the Toon Shader Material
**Shadow Intensity:** A slider that dictates how much shadow gets cast by the light. 
A High Intensity
![intense](~assets/images/shaders/shadowIntensityHigh.png)
![lessIntense](~assets/images/shaders/shadowIntensityLow.png)
**Shadow Brightness:** A slider that dictates the brightness of the shadow 
A Zero Brightness Shadow
![brightness](~assets/images/shaders/shadowIntense.png)
**Color:** Changes the color of the whole material, when paired with a texture adds a hue change to the texture as well. 
![color](assets/images/shaders/color.png)
**Main Texture:** Adds a texture to the material 
![texture](assets/images/shaders/texture.png)

## Outline Shader 

### What it does:
Creates an outline around the mesh of the 3D object. Creates a more toony render effect. 

### Shade Smooth vs As it is:
Models shaded smooth work better with the outline. Leaving it just as is, creates weird gaps around the object. Especially the player. 

**Shading Smooth** 
![smooth](assets/images/shaders/shadeSmooth.png)

**Just as is**
![smooth](assets/images/shaders/asIs.png)

### Inspector Items Under the Outline Material:
**Outline Color:** Changes the color of the outline 
**Outline Thickness:** Changes the size of the outline. 
Image applicable to both previous items
![smooth](assets/images/shaders/extremeOutline.png)
**Outline Brightness:** Changes the brightness of the outline
![smooth](![smooth](assets/images/shaders/outlineBrightness)