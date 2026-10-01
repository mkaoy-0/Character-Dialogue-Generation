using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateCharaData : MonoBehaviour
{
    /// <summary>
    /// キャラ設定画面を閉じるボタンを押したときの挙動
    /// </summary>
    public CharaEditor editor;
    public RadarChart chart;
    public CharaStatusTextManager charaStatusManager;
    public GenerateCharaDescription generateDescription; 

    private CreateNewChara _createNewChara;
    public CreateNewChara createNewChara { set { _createNewChara = value; } }

    public ImageVisualizeManager imgVisManager;

    public GeneratePersonality generatePersonality;
    public CreateIF_Personality createPersonalityUI;

    public GameObject mainCanvas;
    public GameObject editCanvas;

    public void UpdateChara()
    {
        CanvasActiveController();

        if (string.IsNullOrEmpty(charaStatusManager.nameField.text)) return;

        // 更新
        bool statusChanged = editor.IsStatusChanged(charaStatusManager);
        bool chartChanged = editor.IsChartChanged(chart);

        if (statusChanged)
        {
            Debug.Log("設定更新");
            editor.UpdateCharaSettings(charaStatusManager);
        }
        if (chartChanged)
        {
            Debug.Log("性格値更新");
            editor.UpdateStatsFromChart(chart);
        }

        if (statusChanged || chartChanged)
        {
            Debug.Log("説明リクエスト実行");
          //  generateDescription.GenerateCharaDesctiptionByGPT(editor.editingChara);
            editor.PrintEditingChara();
        }

        // キャラ作成ボタンの名前表示更新
        if (_createNewChara != null)
        {
            _createNewChara.charaDataSet.SetActive(true);
            _createNewChara.charaDataSet.GetComponentInChildren<TextMeshProUGUI>().text = charaStatusManager.nameField.text;
            if (imgVisManager != null)
            {
                imgVisManager.data = CharaDatabase.Instance.GetCharaByName(charaStatusManager.nameField.text);
                imgVisManager.raw = _createNewChara.charaDataSet.GetComponentInChildren<RawImage>();
            }

        }

        createPersonalityUI.ReloadPersonalityUI(editor.editingChara.name);
        generatePersonality.RCTextVisualizer(editor.editingChara, _createNewChara.rcTextBg);

    }

    public void CanvasActiveController()
    {
        mainCanvas.SetActive(true);
        editCanvas.SetActive(false);
    }

}
