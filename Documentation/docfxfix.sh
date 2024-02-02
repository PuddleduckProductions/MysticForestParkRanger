repl="s/([nN]ame[A-Za-z]*: )''/\1MysticForestParkRanger/g"
sed -i -E "${repl}" api/toc.yml
sed -i -E "${repl}" api/MysticForestParkRanger.yml