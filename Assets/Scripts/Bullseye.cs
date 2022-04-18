using UnityEngine;

public class Bullseye : MonoBehaviour
{
    [SerializeField]
    private int maxScore = 10;
    [SerializeField]
    private int minScore = 0;

    public int GetScore(Vector3 pos)
    {
        float distance = Vector3.Distance(transform.position, pos);
        float maxDistance = transform.lossyScale.x / 2;
        float t = distance / maxDistance;
        float intermediateScore = Mathf.Lerp((float)maxScore, (float)minScore, t);
        return (int)Mathf.Pow(intermediateScore, 2.0f);
    }

    // Checks to make sure an object properly collided with the bullseye since 
    // the bullseye uses a box collider even though it is a circular shape
    public bool HasCollidedWith(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) < (transform.lossyScale.x / 2);
    }

}
