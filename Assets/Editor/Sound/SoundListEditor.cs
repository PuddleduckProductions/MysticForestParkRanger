using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SoundList))]
public class SoundListEditor : Editor
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    public override VisualElement CreateInspectorGUI() {
    var root = m_VisualTreeAsset.Instantiate();
        return root;
    }

}
