# Unity Merge Tools
If you're having trouble merging Unity scenes, there's a few steps you can take.

Biggest help is  this tutorial:

https://stackoverflow.com/questions/63005558/setting-up-unity-smart-merge-with-git-and-meld-for-unity-2019-3

`.gitattributes` is automatically set up for you. You'll need to edit `.gitconfig`. Here's mine, for reference:

```
[merge "unityyamlmerge"]
	driver='C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.16f1\\Editor\\Data\\Tools\\UnityYAMLMerge.exe' merge -p \"$BASE\" \"$REMOTE\" \"$LOCAL\" \"$MERGED\"
	name=Unity SmartMerge
[diff]
    tool = meld
[difftool "meld"]
    path = C:/Program Files/Meld/meld.exe
[merge]
    tool = meld
[mergetool "meld"]
    path = C:/Program Files/Meld/meld.exe
    prompt = false
```

And here's my `mergespecfile.txt`:
```
unity use "%programs%\Meld\Meld.exe" "%l" "%r" "%b" "%d"
prefab use "%programs%\Meld\Meld.exe" "%l" "%r" "%b" "%d"
* use "%programs%\Meld\meld.exe" "%l" "%r" "%b" "%d"
```

UnityYamlMerge just needs a merge tool to fall back on in `mergespecfile.txt`. It'll automatically merge files that it can in Github Desktop, but it'll throw an error unless you set up a tool like Meld. In fact, Github Desktop won't even open Meld for you during a merge. You can do that with:

`git mergetool`.

If you abort a merge however, just make sure to discard all the leftover files from that aborted merge.