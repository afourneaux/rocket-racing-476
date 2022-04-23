using UnityEngine;

// Class which contains data common to both AI and Player racers. This data will be unique to each vehicle
[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float maxVelocity = 20.0f;
    [SerializeField]
    private float maxForce = 10.0f;
    [SerializeField]
    private float rotationSpeed = 1.5f;
    [SerializeField]
    private float width = 2.0f;
    [SerializeField]
    private float height = 2.0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    public float GetMaxAcceleration() { return maxAcceleration; }
    public float GetMaxVelocity() { return maxVelocity; }
    public float GetMaxForce() { return maxForce; }
    public float GetRotationSpeed() { return rotationSpeed; }
    public float GetWidth() { return width; }
    public float GetHeight() { return height; }

    private void OnDestroy()
    {
        if (rb != null)
        {
            RacerManager.RemoveRacer(rb);
        }
    }
}
