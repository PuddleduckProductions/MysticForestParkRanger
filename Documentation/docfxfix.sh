CYAN='\033[0;36m'
echo -e "${CYAN}Running Docfx fix"
repl="s/([nN]ame[A-Za-z]*: )''/\1MysticForestParkRanger/g"
SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
sed -i -E "${repl}" $SCRIPT_DIR/api/toc.yml
sed -i -E "${repl}" $SCRIPT_DIR/api/MysticForestParkRanger.yml