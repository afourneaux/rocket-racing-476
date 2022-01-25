using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathManager : MonoBehaviour
{
    private static PathManager Manager;
    private LineRenderer line;

    private void Awake()
    {
        Manager = this;
        line = GetComponent<LineRenderer>();
    }

    public static List<Vector3> GetPath()
    {
        Vector3[] positionArr = new Vector3[Manager.line.positionCount];
        Manager.line.GetPositions(positionArr);
        return new List<Vector3>(positionArr);
    }
}
