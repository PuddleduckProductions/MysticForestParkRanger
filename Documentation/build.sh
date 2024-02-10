SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
docfx metadata $SCRIPT_DIR/docfx.json
bash $SCRIPT_DIR/docfxfix.sh
docfx build $SCRIPT_DIR/docfx.json $1