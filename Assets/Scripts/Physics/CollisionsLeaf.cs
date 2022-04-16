using UnityEngine;

public class CollisionsLeaf : MonoBehaviour
{
    CollisionsRoot root;
    Rigidbody rootRB;
    Collider myCollider;
    const float VELOCITY_THRESHOLD = 1f;

    void Start()
    {
        root = GetComponentInParent<CollisionsRoot>();
        rootRB = GetComponentInParent<Rigidbody>();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision) {
        root.HandleCollision(myCollider, collision, false);
    }

    void OnCollisionStay(Collision collision) {
        // The object is at rest
        if (rootRB.velocity.magnitude <= VELOCITY_THRESHOLD) {
            root.HandleCollision(myCollider, collision, true);
        }
    }
}