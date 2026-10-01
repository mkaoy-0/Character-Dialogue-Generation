using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class RadarNode : MonoBehaviour, IDragHandler
{
    public RadarChart chart;
    public string label;
    public TextMeshProUGUI valueText;

    public int index;
    private int currentStep = 3;
    public float angle; // 固定された角度（ラジアン）

    private int maxStep = 5; // 5段階
    private float stepRadius; // 1ステップの半径

    void Start()
    {
        chart = FindObjectOfType<RadarChart>();
    }

    public void Init(string label, float angleRad, float stepRadius, TextMeshProUGUI valueText)
    {
        this.label = label;
        this.angle = angleRad;
        this.stepRadius = stepRadius;
        this.valueText = valueText;
        // ラベルの固定位置を先に決めておく
        if (valueText != null)
        {
            valueText.text = label; // 初期表示は値なしでもOK
        }

        // ボタンにイベント追加
        var btn = valueText.GetComponentInChildren<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                chart.RemoveNode(this); // 自分を削除
            });
        }

        SetValue(currentStep); // 初期値反映
    }

    public void SetValue(int step)
    {
        currentStep = Mathf.Clamp(step, 0, maxStep);
        Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * stepRadius * currentStep;
        ((RectTransform)transform).localPosition = pos;
    }

    public int GetValue()
    {
        return currentStep;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent, eventData.position, eventData.pressEventCamera, out localPos);

        // このノードが属する方向ベクトル
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // localPos を dir 方向に投影
        float dist = Vector2.Dot(localPos, dir);

        // 0未満なら0にする（反対側に行かないようにする）
        dist = Mathf.Max(0, dist);

        int step = Mathf.RoundToInt(dist / stepRadius);
        SetValue(step);

        chart.UpdateValueLine();
    }
}
