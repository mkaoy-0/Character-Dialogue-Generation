using UnityEngine;
using UnityEngine.EventSystems;

public class GraphClickHandler : MonoBehaviour, IPointerClickHandler
{
    public RectTransform rectTransform;
    public RectTransform pointPos;

    public SceneSettingsController sceneSettingsController;

    private float graphPosX = 0, graphPosY = 0;
    public float GraphPosX => graphPosX;
    public float GraphPosY => graphPosY;

    // クリック検知
    public void OnPointerClick(PointerEventData eventData)
    {
        if (sceneSettingsController.editPanel.activeSelf) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            // ここで localPoint.x, localPoint.y が
            // パネル中心を(0,0)とした座標になる！
            //  Debug.Log($"クリック座標: x={localPoint.x}, y={localPoint.y}");

            pointPos.localPosition = new Vector2(localPoint.x, localPoint.y);

            // RectTransform の幅・高さを取得
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            // 中心を0にした状態で、範囲を -1～+1 に正規化
            float normX = localPoint.x / (width / 2f);
            float normY = localPoint.y / (height / 2f);

          //  Debug.Log($"正規化: x={normX}, y={normY}");

            // 任意のスケールに変換（例: x軸 -10～+10, y軸 -5～+5）
            float graphX = normX * 5f;
            float graphY = normY * 5f;

            // 小数点第一位に丸める
            graphPosX = (float)System.Math.Round(graphX, 1);
            graphPosY = (float)System.Math.Round(graphY, 1);

            Debug.Log($"グラフ座標: x={graphPosX}, y={graphPosY}");
        }
    }
}
