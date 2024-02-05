docfx metadata $PSScriptRoot/docfx.json
$bashroot=$PSScriptRoot -replace '\\', '/'
# And for handy bash conversion on Windows:
$bashroot=$bashroot -replace 'C:/', '/mnt/c/'
bash $bashroot/docfxfix.sh
docfx build $PSScriptRoot/docfx.json $args[0]