using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsSub : MonoBehaviour
{
    HashSet<int> activeCollisions;

    void Start() {
        activeCollisions = new HashSet<int>();

        bool isLeaf = true;

        // Propagate the CollisionsSub script to all child transforms containing a collider
        foreach(Transform child in transform) {
            if (child.GetComponent<Collider>() != null) {
                isLeaf = false;
                child.gameObject.AddComponent<CollisionsSub>();
            }
        }

        // If there are no child components, make this GameObject a CollisionsLeaf and destroy this sub component
        if (isLeaf) {
            gameObject.AddComponent<CollisionsLeaf>();
            Destroy(this);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (activeCollisions.Count == 0) {
            EnableChildColliders();
        }
        activeCollisions.Add(collider.GetInstanceID());
    }

    void OnTriggerExit(Collider collider) {
        activeCollisions.Remove(collider.GetInstanceID());
        if (activeCollisions.Count == 0) {
            DisableChildColliders();
        }
    }

    public void EnableChildColliders() {
        foreach (Transform child in transform) {
            CollisionsSub colSub = child.GetComponent<CollisionsSub>();
            Collider col = child.GetComponent<Collider>();
            if (colSub != null) {
                colSub.EnableChildColliders();
            }
            if (col != null) {
                col.enabled = true;
            }
        }
    }

    public void DisableChildColliders() {
        foreach (Transform child in transform) {
            CollisionsSub colSub = child.GetComponent<CollisionsSub>();
            Collider col = child.GetComponent<Collider>();
            if (colSub != null) {
                colSub.DisableChildColliders();
            }
            if (col != null) {
                col.enabled = false;
            }
        }
    }
}
