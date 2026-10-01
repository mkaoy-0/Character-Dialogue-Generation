using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadarChart : MonoBehaviour
{
    public RectTransform chartArea;
    public GameObject nodePrefab;
    public Button addButton;
    public TMP_InputField ifLabel;

    public UILineRenderer backgroundLine; // 最大値の多角形
    public UILineRenderer[] backgroundStepLines;
    public UILineRenderer axisLines;      // 放射軸
    public UILineRenderer valueLine;      // 現在値

    private List<RectTransform> nodes = new List<RectTransform>();
    public List<RectTransform> Nodes => nodes;
    private List<float> angles = new List<float>();

    public string[] firstLabel = {"外向性", "開放性", "誠実性", "協調性", "神経症傾向"};
    public TextMeshProUGUI labelPrefab;         // 頂点ラベル用のPrefab

    private int maxStep = 5;
    private float maxRadius;


    // =======================================================================

    void Start()
    {
        if (addButton != null)
        {
            addButton.onClick.AddListener(() => AddNode(ifLabel.text));
        }
    }

    public void ChartInitialize()
    {
        ClearAllNodes(); // 一気に消す

        maxRadius = chartArea.rect.width * 0.4f;
        float stepRadius = maxRadius / maxStep;

        for (int i = 0; i < 5; i++) AddNode(firstLabel[i]);
    }

    // CharaData を読み込んでUIに反映
    public void LoadChara(CharaData data)
    {
        ClearAllNodes(); // 一気に消す

        // 辞書の中身を使ってノード復元
        foreach (var pair in data.radarCharStatus)
        {
            AddNode(pair.Key);
            var node = nodes[nodes.Count - 1].GetComponent<RadarNode>();
            node.SetValue(pair.Value);
        }

        UpdateValueLine();

    }

    public void AddNode(string itemLabel)
    {
        int count = nodes.Count + 1;
        float stepRadius = maxRadius / maxStep;

        float angle = 2 * Mathf.PI * (count - 1) / count;
        angles.Add(angle);

        // ラベル生成
        TextMeshProUGUI labelText = Instantiate(labelPrefab, chartArea);

        GameObject nodeObj = Instantiate(nodePrefab, chartArea);
        RectTransform rt = nodeObj.GetComponent<RectTransform>();
        nodes.Add(rt);

        var radarNode = nodeObj.GetComponent<RadarNode>();
        radarNode.chart = this;
     //   radarNode.index = nodes.Count - 1;
        radarNode.Init(itemLabel, angle, stepRadius, labelText);
        radarNode.SetValue(maxStep / 2);


        // 初期位置（最大値に置く）
        Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * maxRadius;
        rt.localPosition = pos;

        RecalculateAngles();
        UpdateBackgroundAndAxes();
        UpdateBackgroundSteps();
        UpdateValueLine();

        ifLabel.gameObject.SetActive(false);
        ifLabel.text = "";
    }

    // 背景ポリゴン & 軸線更新
    public void UpdateBackgroundAndAxes()
    {
        List<Vector2> bgPoints = new List<Vector2>();
        List<Vector2> axisPoints = new List<Vector2>();

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector2 outer = new Vector2(Mathf.Cos(angles[i]), Mathf.Sin(angles[i])) * maxRadius;
            bgPoints.Add(outer);

            // 軸線（中心→外周）
            axisPoints.Add(Vector2.zero);
            axisPoints.Add(outer);
        }

        // 閉じる
        if (bgPoints.Count > 2) bgPoints.Add(bgPoints[0]);

        backgroundLine.SetPoints(bgPoints);
        axisLines.SetPoints(axisPoints);
    }
    public void UpdateBackgroundSteps()
    {
        int count = nodes.Count;
        if (count == 0) return; 

        for (int step = 1; step <= maxStep; step++)
        {
            float radius = maxRadius * step / maxStep;
            List<Vector2> pts = new List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                float angle = 2 * Mathf.PI * i / count;
                Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                pts.Add(pos);
            }
            pts.Add(pts[0]); // 閉じる

            // ここで stepごとのUILineRendererに渡す
            backgroundStepLines[step - 1].SetPoints(pts);
        }
    }


    // ノード値の線更新
    public void UpdateValueLine()
    {
        List<Vector2> valuePoints = new List<Vector2>();
        foreach (var node in nodes)
        {
            valuePoints.Add(node.localPosition);
        }
        if (nodes.Count > 2) valuePoints.Add(nodes[0].localPosition);

        valueLine.SetPoints(valuePoints);
    }

    public void RecalculateAngles()
    {
        float stepRadius = maxRadius / maxStep;
        int count = nodes.Count;
        angles.Clear();

        for (int i = 0; i < count; i++)
        {
            float angle = 2 * Mathf.PI * i / count;
            var radarNode = nodes[i].GetComponent<RadarNode>();
            radarNode.angle = angle;
            angles.Add(angle);

            // ノードがあるなら位置を更新
            if (i < nodes.Count)
            {
                float radius = nodes[i].localPosition.magnitude; // 今の半径を維持
                Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                nodes[i].localPosition = pos;

                // ノードのRadarNodeにも更新
                //  var radarNode = nodes[i].GetComponent<RadarNode>();
                //  radarNode.Init(label, angle, maxRadius / maxStep, labelText);

                // ノードの位置も再計算
                radarNode.SetValue(radarNode.GetValue());
            }

            // ラベルの位置も更新
            if (radarNode.valueText != null)
            {
                float labelOffset = 30f;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                radarNode.valueText.rectTransform.anchoredPosition = dir * (maxRadius / maxStep * maxStep + labelOffset);
            }

        }
    }

    // 値取得
    public Dictionary<string, int> GetNodeValuesDict()
    {
        var dict = new Dictionary<string, int>();
        foreach (var nodeObj in nodes)
        {
            var node = nodeObj.GetComponent<RadarNode>();
            dict[node.label] = node.GetValue();
        }
        return dict;
    }

    // 削除
    public void RemoveNode(RadarNode node)
    {
        if (nodes.Contains(node.GetComponent<RectTransform>()))
            nodes.Remove(node.GetComponent<RectTransform>());

        // UIラベル削除
        if (node.valueText != null)
            Destroy(node.valueText.gameObject);

        // ノード本体削除
        Destroy(node.gameObject);

        RecalculateAngles();
        UpdateBackgroundAndAxes();
        UpdateBackgroundSteps();
        UpdateValueLine();
    }

    // 全消去
    public void ClearAllNodes()
    {
        foreach (var node in nodes)
        {
            var radarNode = node.GetComponent<RadarNode>();
            if (radarNode.valueText != null)
                Destroy(radarNode.valueText.gameObject);
            Destroy(node.gameObject);
        }
        nodes.Clear();
        angles.Clear();

        UpdateBackgroundAndAxes();
        UpdateBackgroundSteps();
        UpdateValueLine();
    }


    // ノードの値を自動で設定する関数
    public void AutoSetNodeValues(int[] values)
    {
        // values の数がノード数と合っているかチェック
        if (values.Length != nodes.Count)
        {
            Debug.LogError("AutoSetNodeValues: 配列の長さがノード数と一致していません");
            return;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            var radarNode = nodes[i].GetComponent<RadarNode>();
            radarNode.SetValue(values[i]);  // 自動で値設定 & ノード移動
        }

        // 線を再描画
        UpdateValueLine();
    }


}
