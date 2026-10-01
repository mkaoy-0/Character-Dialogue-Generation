using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotGenerator : MonoBehaviour
{
    public RectTransform graphArea;
    public UILineRenderer lineRenderer;
    public GameObject dotPrefab;

    private List<GameObject> dotInstances = new List<GameObject>();

    public void DrawVerticalGraph(List<float> trustValues)
    {
        // 点の削除（再描画のため）
        foreach (GameObject dot in dotInstances)
        {
            Destroy(dot);
        }
        dotInstances.Clear();

        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        int pointCount = trustValues.Count;
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < pointCount; i++)
        {
            float trust = trustValues[i];

            float x = trust * width;
            float y = height - ((float)i / (pointCount - 1) * height);

            Vector2 point = new Vector2(x, y);
            points.Add(point);

            // ●を生成して配置
            GameObject dot = Instantiate(dotPrefab, graphArea);
            RectTransform dotRect = dot.GetComponent<RectTransform>();
            dotRect.anchoredPosition = point;
            dot.SetActive(true);

            dotInstances.Add(dot);

            var dotScript = dot.GetComponent<DraggableDot>();
            dotScript.graphArea = graphArea;
            dotScript.graphController = FindObjectOfType<GraphController>();
            dotScript.SetIndex(i);
        }

        lineRenderer.SetPoints(points);
    }
}
