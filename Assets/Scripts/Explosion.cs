using UnityEngine;

// Simple Script responsible of destroying the explosion effect after a given amount of time
public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 2.0f;
    private float currLifeTime = 0.0f;

    private void Update()
    {
        currLifeTime += Time.deltaTime;

        if (currLifeTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
