# Introduction

## Doc Structure
You'll see some items on the left. Here's a description of each:

### Tutorials
Any relevant tutorials to a subject you may be trying to learn about. There will be subcategories based on the subject.

### Scenes
A list of all the scenes contained in the build and a description of what they're there for.

## Writing your own Docs
DocFX has some documentation on this subject. Write your articles in Documentation/docs as .md files. Then to add them to the table of contents in Documentation/docs: https://dotnet.github.io/docfx/docs/table-of-contents.html.

Assets are exposed in the Documentation/assets folder, so place any items you need in there, then access them with `/assets/path/to/asset.mp4`.

For instance, you can check the source for this .md file to see the relative path to the site icon:

![The site icon](/assets/images/icon.png)

Which is `/assets/images/icon.png`.

If you want to test your changes locally, you'll need docfx installed: https://dotnet.github.io/docfx/index.html
(You should already have the .NET SDK if you're running Unity)

Then you can run `docfx build Documentation/docfx.json --serve` to preview your changes.