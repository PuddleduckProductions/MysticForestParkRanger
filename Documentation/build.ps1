docfx metadata $PSScriptRoot/docfx.json
bash $PSScriptRoot/docfxfix.sh
docfx build $PSScriptRoot/docfx.json $args[0]