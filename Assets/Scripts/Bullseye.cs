using UnityEngine;

public class Bullseye : MonoBehaviour
{
    [SerializeField]
    private int maxScore = 10000;
    [SerializeField]
    private int minScore = 0;

    public int GetScore(Vector3 pos)
    {
        float distance = Vector3.Distance(transform.position, pos);
        float maxDistance = transform.localScale.x / 2;
        float t = distance / maxDistance;
        return (int)Mathf.Lerp((float)maxScore, (float)minScore, t);
    }

    // Checks to make sure an object properly collided with the bullseye since 
    // the bullseye uses a box collider even though it is a circular shape
    public bool HasCollidedWith(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) < (transform.localScale.x / 2);
    }

}
