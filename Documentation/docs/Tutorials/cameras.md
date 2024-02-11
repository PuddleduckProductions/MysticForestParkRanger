# Adding Dynamic Cameras
Want to have a camera that will be automatically switched to? Here's all you have to do:

1. Make sure you're using the [PlayArea](../Assets/scenes.md#PlayArea) scene template.

2. Add a cinemachine virtual camera anywhere in the scene.

> [!TIP]
> To frame the shot, you can view in edit mode and then select "GameObject->Cinemachine->Virtual Camera".

![Create Camera](~/assets/images/cameras/create-camera.png)

3. Go to the Player object and click the "Update Cameras List" button.

![Update Cameras Button](~/assets/images/cameras/update-cameras.png)

View your results!

> [!Video https://puddleduckproductions.github.io/MysticForestParkRanger/assets/videos/cameras.mp4]

Feel free to mess around with the virtual camera settings to get the best shot! You should also view the [CameraController](~/api/Character.CameraController.yml#Update) source to view the priority system. It's weighted based on whether or not the player is in shot, and then by distance. If no suitable camera is detected, the system defaults to the PlayerCamera.