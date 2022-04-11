using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsLeaf : MonoBehaviour
{
    CollisionsRoot root;
    Collider myCollider;

    void Start()
    {
        root = GetComponentInParent<CollisionsRoot>();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision) {
        root.HandleCollision(myCollider, collision);
    }
}
