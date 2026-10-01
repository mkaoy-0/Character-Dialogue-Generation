using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour
{
    [SerializeField] private UILineRenderer lineRenderer;
    [SerializeField] private RectTransform graphArea;
    [SerializeField] private DotGenerator dotGenerator;

    private List<float> trustValues = new List<float>();

    public int TrustValueCount => trustValues.Count;


    public void DrawGraph()
    {
        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        int pointCount = trustValues.Count;
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < pointCount; i++)
        {
            float x = trustValues[i] * width;
            float y = height - ((float)i / (pointCount - 1) * height);

            points.Add(new Vector2(x, y));
        }

        lineRenderer.SetPoints(points);
    }

    /*/
    public void UpdateTrustValue(int index, float newValue)
    {
        trustValues[index] = newValue;
        DrawGraph();
        dotGenerator.DrawVerticalGraph(trustValues);
    }
    /**/

    // ‚±‚ÌŠÖ”‚ÍÄ•`‰æ‚µ‚È‚¢I
    public void UpdateSingleTrustValue(int index, float newValue)
    {
        trustValues[index] = newValue;
        lineRenderer.SetPoints(CalculatePoints(trustValues)); // Ü‚êü‚¾‚¯XV
    }

    private List<Vector2> CalculatePoints(List<float> trustValues)
    {
        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < trustValues.Count; i++)
        {
            float x = trustValues[i] * width;
            float y = height - ((float)i / (trustValues.Count - 1) * height);
            points.Add(new Vector2(x, y));
        }
        return points;
    }

    public float GetTrustValue(int index)
    {
        return trustValues[index];
    }


    void Start()
    {
        trustValues = new List<float> { 0.2f, 0.5f, 0.4f };
        DrawGraph();
        dotGenerator.DrawVerticalGraph(trustValues);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            trustValues.Add(Random.Range(0.0f, 1.0f));
            DrawGraph();
            dotGenerator.DrawVerticalGraph(trustValues);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (trustValues.Count > 2)
            {
                trustValues.RemoveAt(trustValues.Count - 1);

                DrawGraph();
                dotGenerator.DrawVerticalGraph(trustValues);
            }
        }
    }

}
