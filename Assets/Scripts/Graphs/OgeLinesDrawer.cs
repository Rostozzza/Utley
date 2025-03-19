using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OgeLinesDrawer : MonoBehaviour
{
    [SerializeField] private bool show;
    private List<Vector3> pointsTo;
    void OnDrawGizmos()
    {
        if (show)
        {
            pointsTo = GetComponent<OgePointLogic>().GetConnectedPoints().ConvertAll(x => x.transform.position);
            if (pointsTo.Count == 0) return;

            Gizmos.color = Color.yellow;
            foreach (var point in pointsTo)
            {
                Gizmos.DrawLine(transform.position, point);
                DrawArrow(point, transform.position, true);
            }
        }
    }

    private void DrawArrow(Vector3 pointFrom, Vector3 pointTo, bool drawArrowOnRib = false)
    {
        Vector3 vectorToSource = (pointTo - pointFrom).normalized;

        float radianDeviationFromSource = Mathf.Atan2(vectorToSource.y, vectorToSource.x);
        
        Vector3 topDeviation = new(Mathf.Cos(radianDeviationFromSource + Mathf.PI / 4), Mathf.Sin(radianDeviationFromSource + Mathf.PI / 4));
        Vector3 bottomDeviation = new(Mathf.Cos(radianDeviationFromSource - Mathf.PI / 4), Mathf.Sin(radianDeviationFromSource - Mathf.PI / 4));

        Gizmos.DrawLine(pointFrom, pointFrom + topDeviation * 0.025f);
        Gizmos.DrawLine(pointFrom, pointFrom + bottomDeviation * 0.025f);
        
        if (drawArrowOnRib) // don't try to steal, it's not reference (works badly);
        {
            pointFrom = (pointTo + pointFrom) / 2;
            Gizmos.DrawLine(pointFrom, pointFrom + topDeviation * 0.025f);
            Gizmos.DrawLine(pointFrom, pointFrom + bottomDeviation * 0.025f);
        }
    }
}
