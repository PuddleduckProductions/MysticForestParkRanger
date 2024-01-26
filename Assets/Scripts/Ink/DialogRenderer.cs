using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class DialogRenderer : MonoBehaviour
{
    [SerializeField]
    protected TMP_FontAsset symbolsFont;
    protected TMP_FontAsset defaultFont;

    protected RectTransform dialogParent;
    protected TextMeshProUGUI dialog;
    protected Camera worldCamera;

    private Transform targetObject;

    public void Init() {
        worldCamera = Camera.main;
        dialogParent = transform.GetChild(0).GetComponent<RectTransform>();
        dialog = dialogParent.GetComponentInChildren<TextMeshProUGUI>();
        
        defaultFont = dialog.font;
    }

    protected bool TryGetCharacter(string name, out GameObject character) {
        character = GameObject.Find(name);
        return character != null;
    }

    private void Update() {
        if (targetObject != null) { 
            var pos = worldCamera.WorldToScreenPoint(targetObject.position);
            dialogParent.position = new Vector2(Mathf.Clamp(pos.x, 0, Screen.width), Mathf.Clamp(pos.y, 0, Screen.height));
        }
    }

    public void Render(InkManager.DialogLine line) {
        // Our current "zero" coordinates for setting dialogParent's global coordinates:
        dialogParent.localScale = Vector3.one;
        Vector3 position = this.transform.position;
        if (line.character != "" && TryGetCharacter(line.character, out GameObject character)) {
            targetObject = character.transform;
            position = worldCamera.WorldToScreenPoint(targetObject.position);
        } else {
            targetObject = null;
        }
        if (line.tags.Contains("symbols")) {
            dialog.font = symbolsFont;
            dialog.fontSize = 150.0f;
        } else {
            dialog.font = defaultFont;
        }
        dialog.text = line.dialog;
        dialogParent.position = position;
    }


}
