---
_layout: landing
---

# Mystic Forest Park Ranger
Welcome to the documentation for MysticForestParkRanger!

If you're a programmer, you might want to check the [API](/api).

For tutorials, check the [Docs](docs/introduction.md).

Feel free to contribute to this repository in the Documentation folder on Github. Markdown files can be added in Documentation/docs. Comments that use XML formatting (i.e., ///&gt;summary&lt;) from within the project will be added.

Refer to [Markdown](http://daringfireball.net/projects/markdown/) for how to write markdown files.

## Generating Documentation
Unfortunately, automatic updates are not allowed due to the .gitignore for Unity removing the crucial elements that need to be tracked for automatic generation.

To update this documentation for yourself at any time (if you have [docfx installed](https://github.com/dotnet/docfx?tab=readme-ov-file#getting-started)), you have two routes:

### Windows
Run `build.ps1` in `Documentation/docs`

If you want to preview the docs, you can add `build.ps1 --serve` to run a local server. 

### MacOS/Linux
Run `build.sh` in `Documentation/docs`.

If you want to preview the docs, you can add `build.sh --serve`  to run a local server.

If you experience any bugs, let Tyler know.