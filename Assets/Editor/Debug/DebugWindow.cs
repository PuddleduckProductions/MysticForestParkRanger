using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
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

    protected delegate void UpdateFunc();
    protected event UpdateFunc onUpdate;

    void Update() {
        onUpdate?.Invoke();
    }

    #region Utility
    protected void Log(string message) {
        consoleList.Add(new Label(message));
    }

    protected string settingPath {
        get { return "DebugWindow/";  }
    }

    bool VerifyPlayMode() {
        if (!EditorApplication.isPlaying) {
            Log("Not in play mode.");
            return false;
        }
        return true;
    }

    GameObject GetPlayer() {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Log("Could not find player.");
        }
        return player;
    }
    #endregion

    #region Teleportation
    TextField teleportObjName;
    Button teleportToClick;
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

        teleportToClick = rootVisualElement.Query<Button>("teleportToClick");
        teleportToClick.RegisterCallback<ClickEvent>(ClickTeleport);
    }

    private void Teleport(ClickEvent evt=null) {
        if (!VerifyPlayMode()) return;
        string name = rootVisualElement.Query<TextField>("teleportObjectName").First().value;
        var obj = GameObject.Find(name);
        if (obj == null) {
            Log("Object not found.");
            return;
        }

        var player = GetPlayer();
        if (player == null) {
            return;
        }

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = obj.transform.position + 5 * Vector3.up;
        player.GetComponent<CharacterController>().enabled = true;
    }

    bool clickOnNextUpdate = false;
    void CleanClickUpdate() {
        onUpdate -= ClickTeleportUpdate;
        teleportToClick.text = "Teleport to Click";
        clickOnNextUpdate = false;
    }

    void ClickTeleportUpdate() {
        if (clickOnNextUpdate) {
            var mainCamera = Camera.main;
            Debug.Log(Input.mousePosition);
            if (mainCamera != null) {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    var player = GetPlayer();
                    if (player != null) {
                        player.GetComponent<CharacterController>().enabled = false;
                        player.transform.position = hit.point;
                        player.GetComponent<CharacterController>().enabled = true;
                    }
                } else {
                    Log("Could not find point.");
                }
            } else {
                Log("Could not find main camera.");
            }
            CleanClickUpdate();
        }

        if (!VerifyPlayMode()) {
            CleanClickUpdate();
        }

        if (Input.GetMouseButtonDown(0)) {
            clickOnNextUpdate = true;
        }
    }

    private void ClickTeleport(ClickEvent evt) {
        if (!VerifyPlayMode()) return;
        onUpdate += ClickTeleportUpdate;
        teleportToClick.text = "Click on screen...";
    }
    #endregion
}
