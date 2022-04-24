using UnityEngine;

public class CollisionsLeaf : MonoBehaviour
{
    CollisionsRoot root;
    Rigidbody rootRB;
    Collider myCollider;
    const float VELOCITY_THRESHOLD = 1f;

    void Awake()
    {
        root = GetComponentInParent<CollisionsRoot>();
        rootRB = GetComponentInParent<Rigidbody>();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        rb.isKinematic = true;
        rb.mass = 0f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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
