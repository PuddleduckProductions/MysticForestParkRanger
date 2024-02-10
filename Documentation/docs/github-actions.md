# Github Actions

## Automatic Builds
Located in `.github/workflows/main.yml`. Uses [GameCI](https://game.ci/).

Right now only Windows builds are supported. Other OSes may be supported later, but right now this is meant to be a system to automatically run tests and generate a usable build for whatever the latest version of the game is.

If you're in a bind and need the latest build in a hurry, please use this system!


### To Download:

Go to the Unity Build and Test action: [![Unity Build and Test](https://github.com/PuddleduckProductions/MysticForestParkRanger/actions/workflows/main.yml/badge.svg?branch=master)](https://github.com/PuddleduckProductions/MysticForestParkRanger/actions/workflows/main.yml)

If it shows passing, go ahead and click. If the status checks show failing, that means that the latest build will most likely not work. Talk to a programmer! 

Then, you'll want to click on the latest run in the list.

![Open latest run](~/assets/images/actions/latestWorkflow.png)

Scroll down to the bottom, and download the `Build` artifact.

![Download the latest build](~/assets/images/actions/downloadBuild.png)

## Automatic Documentation
Located in `.github/workflows/documentation.yml`. Uses DocFx to build what you're reading right now.

Will only run `docfx build Documentation/docfx.json` to avoid overwritting the `api` folder.

Run `Documentation/build.ps1` (if on Windows) or `Documentation/build.sh` to regenerate the `api` folder.