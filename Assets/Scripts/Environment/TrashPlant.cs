using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TrashPlant : MonoBehaviour
{
    public float radius = 1f;
    public int trashTillHappy = 3;

    //FMOD
    [SerializeField]
    public EventReference retractSound;

    // Temp code for now, we may want to change for later:
    private void FixedUpdate() {
        var colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var c in colliders) {
            if (c.tag == "Trash") {
                if (c.TryGetComponent(out Interaction i)) {
                    i.EndInteraction();
                }
                if (c.TryGetComponent(out GridObject g)) {
                    g.RemoveFromGridFull();
                }
                Destroy(c.gameObject);
                trashTillHappy--;
            }
        }
        if(trashTillHappy<=0){
            AudioManager.Instance.PlayOneShot("vinesRetract", retractSound, GetComponent<Transform>());
            this.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
