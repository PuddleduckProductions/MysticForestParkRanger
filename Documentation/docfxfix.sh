CYAN='\033[0;36m'
NC='\e[0m'
echo -e "${CYAN}Running Docfx fix${NC}"
repl="s/([nN]ame[A-Za-z]*: )''/\1MysticForestParkRanger/g"
SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
sed -i -E "${repl}" $SCRIPT_DIR/api/toc.yml
sed -i -E "${repl}" $SCRIPT_DIR/api/MysticForestParkRanger.yml
echo -e "${CYAN}Finished Docfx fix${NC}"