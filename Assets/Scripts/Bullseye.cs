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
}
