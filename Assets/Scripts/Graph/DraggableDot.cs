using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableDot : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform graphArea;
    public GraphController graphController;

    private int index;

    public void SetIndex(int i) => index = i;

    void Start()
    {
        graphController = FindObjectOfType<GraphController>();
        graphArea = this.transform.parent.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            graphArea, eventData.position, eventData.pressEventCamera, out Vector2 localPos))
            return;

        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        // X位置を制限（0-width）
        float clampedX = Mathf.Clamp(localPos.x, 0, width);

        // 信頼値（0-1に正規化）
        float newTrust = clampedX / width;

        // ドットの位置を更新（Yは固定）
        float y = height - ((float)index / (graphController.TrustValueCount - 1) * height);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(clampedX, y);

        // 値の更新（再描画しないように注意）
        graphController.UpdateSingleTrustValue(index, newTrust);
    }

    // 「ドラッグ終了時」に呼ばれる
    public void OnEndDrag(PointerEventData eventData)
    {
        float currentTrust = graphController.GetTrustValue(index);
        Debug.Log($"Dot {index} drag ended. Trust value: {currentTrust:F2}");
    }

}
