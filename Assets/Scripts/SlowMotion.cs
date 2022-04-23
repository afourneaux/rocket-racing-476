using UnityEngine;

// Script responsible of adding a slow motion effect when the player collides with something.
public class SlowMotion : MonoBehaviour
{
    // the minimum time between the end of the last slow motion effect and the start of the next one.
    [SerializeField]
    private float slowMotionCooldown = 3.0f;
    private float currSlowMotionCooldownTimer;

    [SerializeField]
    private float gameTimeDurationOfEffect = 0.1f;
    [SerializeField]
    private float timeScaleOfSlowMotionEffect = 0.5f;

    private float timeScaleDurationTimer;
    private float currSlowMotionEffectTimer;
    private Camera cam;
    private CameraController camController;

    private bool inSlowMotion = false;
    private Vector3 prevCamPos;
    private Quaternion prevCamRotation;


    [SerializeField]
    private float cameraOffsetDistance = 5.0f;
    private float previousTimeScale;

    private void Start()
    {
        CollisionsRoot rootCollider = GetComponent<CollisionsRoot>();
        camController = GetComponent<CameraController>();
        if (camController == null || rootCollider == null)
        {
            Destroy(this); // remove this component if it is not attached on the player
            return;
        }

        cam = Camera.main;
        rootCollider.onCollision += OnCollision;
        currSlowMotionCooldownTimer = -CountdownController.Instance.getCountdownTime();

        if (timeScaleOfSlowMotionEffect <= 0.0f)
        {
            Debug.LogError("Invalid time scale provided for the slow motion effect. The time scale must be greater than 0");
            Destroy(this);
            return;
        }
        timeScaleDurationTimer = gameTimeDurationOfEffect * timeScaleOfSlowMotionEffect;
    }

    private void Update()
    {
        if (inSlowMotion)
        {
            if (SlowMotionManager.SlowMotionIsEnabled())
            {
                if (currSlowMotionEffectTimer < timeScaleDurationTimer)
                {
                    currSlowMotionEffectTimer += Time.deltaTime;
                }
                else
                {
                    StopSlowMotion();
                }
            }
            else
            {
                StopSlowMotion();
            }
        }
        else
        {
            if (currSlowMotionCooldownTimer < slowMotionCooldown)
            {
                currSlowMotionCooldownTimer += Time.deltaTime;
            }
        }
    }

    // Event function passed to the root collider to be called when a collision occurs 
    private void OnCollision(Collision collision)
    {
        if (SlowMotionManager.SlowMotionIsEnabled())
        {
            ApplySlowMotionEffect(collision.contacts[0].point, collision.contacts[0].normal);
        }
    }

    private void ApplySlowMotionEffect(Vector3 collisionPoint, Vector3 normal)
    {
        if (inSlowMotion || currSlowMotionCooldownTimer < slowMotionCooldown)
        {
            return;
        }

        inSlowMotion = true;
        camController.enabled = false;
        currSlowMotionEffectTimer = 0.0f;

        // Store previous cam position and rotation
        prevCamPos = cam.transform.position;
        prevCamRotation = cam.transform.rotation;

        // Move camera to look at collision
        Vector3 forward = Vector3.Cross(normal, Vector3.up);
        Vector3 forwardWithoutY = new Vector3(forward.x, 0.0f, forward.z);

        cam.transform.position = collisionPoint + -forwardWithoutY.normalized * cameraOffsetDistance;
        cam.transform.LookAt(collisionPoint, normal);
        cam.transform.eulerAngles = new Vector3(0.0f, cam.transform.eulerAngles.y, 0.0f);

        // Slow down time
        previousTimeScale = Time.timeScale;
        Time.timeScale = timeScaleOfSlowMotionEffect;
    }

    private void StopSlowMotion()
    {
        if (!inSlowMotion)
        {
            return;
        }

        inSlowMotion = false;
        camController.enabled = true;
        currSlowMotionCooldownTimer = 0.0f;

        // Restore cam position and transform
        cam.transform.position = prevCamPos;
        cam.transform.rotation = prevCamRotation;

        Time.timeScale = previousTimeScale;
    }


}
