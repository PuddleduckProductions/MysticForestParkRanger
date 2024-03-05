using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPlant : MonoBehaviour
{
    public float radius = 1f;
    public int trashTillHappy = 3;
    // Temp code for now, we may want to change for later:
    private void FixedUpdate() {
        var colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var c in colliders) {
            if (c.tag == "Trash") {
                if (c.TryGetComponent(out Interaction i)) {
                    i.EndInteraction();
                }
                Destroy(c.gameObject);
                trashTillHappy--;
            }
        }
        if(trashTillHappy<=0){
            this.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
