using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsLeaf : MonoBehaviour
{
    CollisionsRoot root;

    void Start()
    {
        root = GetComponentInParent<CollisionsRoot>();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision) {
        root.HandleCollision(gameObject, collision);
    }
}
