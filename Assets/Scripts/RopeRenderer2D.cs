using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeRenderer2D : MonoBehaviour
{
    public Transform startPoint;   // Anchor point
    public Transform endPoint;     // Platform
    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2; // start and end
    }

    void LateUpdate()
    {
        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, endPoint.position);
    }
}
