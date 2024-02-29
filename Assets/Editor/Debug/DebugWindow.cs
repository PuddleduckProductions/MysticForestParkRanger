using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    ScrollView consoleList;

    [MenuItem("Puddleduck/Debug Window ^&w")]
    public static void ShowExample()
    {
        DebugWindow wnd = GetWindow<DebugWindow>();
        wnd.titleContent = new GUIContent("Debug Window");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement fromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(fromUXML);

        consoleList = root.Query<ScrollView>("consoleList");
        TeleportSetup();
    }

    protected void Log(string message) {
        consoleList.Add(new Label(message));
    }

    protected string settingPath {
        get { return "DebugWindow/";  }
    }

    TextField teleportObjName;
    private void TeleportSetup() {
        rootVisualElement.Query<Button>("teleport").First().RegisterCallback<ClickEvent>(Teleport);
        teleportObjName = rootVisualElement.Query<TextField>("teleportObjectName");
        teleportObjName.value = EditorPrefs.GetString($"{settingPath}teleportObjName");
        teleportObjName.RegisterCallback<KeyDownEvent>((evt) => {
            EditorPrefs.SetString($"{settingPath}teleportObjName", teleportObjName.value);
            if (evt.keyCode == KeyCode.Return) {
                evt.StopPropagation();
                evt.PreventDefault();
                Teleport();
            }
        }, TrickleDown.TrickleDown);
    }

    private void Teleport(ClickEvent evt=null) {
        if (!EditorApplication.isPlaying) {
            Log("Not in play mode.");
            return;
        }
        string name = rootVisualElement.Query<TextField>("teleportObjectName").First().value;
        var obj = GameObject.Find(name);
        if (obj == null) {
            Log("Object not found.");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Log("Player not found.");
            return;
        }

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = obj.transform.position + 5 * Vector3.up;
        player.GetComponent<CharacterController>().enabled = true;
    }
}
