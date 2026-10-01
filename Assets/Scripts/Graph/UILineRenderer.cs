using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    public List<Vector2> points = new List<Vector2>();
    public float lineThickness = 5f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (points == null || points.Count < 2)
            return;

        float halfThickness = lineThickness / 2f;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * halfThickness;

            Vector2 v0 = start - normal;
            Vector2 v1 = start + normal;
            Vector2 v2 = end + normal;
            Vector2 v3 = end - normal;

            int idx = vh.currentVertCount;

            vh.AddVert(v0, color, Vector2.zero);
            vh.AddVert(v1, color, Vector2.zero);
            vh.AddVert(v2, color, Vector2.zero);
            vh.AddVert(v3, color, Vector2.zero);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }
    }

    public void SetPoints(List<Vector2> newPoints)
    {
        points = newPoints;
        SetVerticesDirty();
    }
}
