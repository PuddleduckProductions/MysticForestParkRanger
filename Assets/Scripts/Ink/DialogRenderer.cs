using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DialogRenderer : MonoBehaviour
{
    protected RectTransform dialogParent;
    protected TextMeshProUGUI dialog;
    protected Camera worldCamera;

    public void Init() {
        worldCamera = Camera.main;
        dialogParent = transform.GetChild(0).GetComponent<RectTransform>();
        dialog = dialogParent.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected bool TryGetCharacter(string name, out GameObject character) {
        character = GameObject.Find(name);
        return character != null;
    }

    public void Render(InkManager.DialogLine line) {
        // Our current "zero" coordinates for setting dialogParent's global coordinates:
        dialogParent.localScale = Vector3.one;
        Vector3 position = this.transform.position;
        if (line.character != "" && TryGetCharacter(line.character, out GameObject character)) {
            position = worldCamera.WorldToScreenPoint(character.transform.position);
        }
        Debug.Log(position);
        dialog.text = line.dialog;
        dialogParent.position = position;
    }


}
