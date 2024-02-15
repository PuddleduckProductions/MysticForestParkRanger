using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Interactions.Behaviors {
    public class ShowImageInteraction : InteractionBehavior {
        public ShowImageInteraction(Interaction parent) : base(parent) { }

        public Sprite imageToShow;
        bool isShowing = false;
        public override void Interact() {
            if (!isShowing) {
                GameObject.FindGameObjectWithTag("Player").GetComponent<characterController>().moveEnabled = false;
                isShowing = true;
                ISingleton<ShowImage>.Instance.Show(imageToShow);
            } else {
                isShowing = false;
            }
        }

        public override bool Update() {
            return isShowing;
        }

        public override void EndInteraction() {
            GameObject.FindGameObjectWithTag("Player").GetComponent<characterController>().moveEnabled = true;
            ISingleton<ShowImage>.Instance.Hide();
        }
    }
}
public class ShowImage : MonoBehaviour, ISingleton<ShowImage>
{
    Image img;
    // Start is called before the first frame update
    void Start()
    {
        (this as ISingleton<ShowImage>).Initialize();
        img = GetComponent<Image>();
    }

    public void Show(Sprite sprite) {
        img.sprite = sprite;
        img.enabled = true;
        img.SetNativeSize();
    }

    public void Hide() {
        img.enabled = false;
    }
}
