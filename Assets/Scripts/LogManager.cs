using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConversationTurn
{
    public string speaker;
    public string line;
    public Dictionary<string, float> sliderValues;
    public int turnIndex;
}

public class ConversationLog
{
    public List<ConversationTurn> turns = new();
    public string worldSetting;
    public CharacterData charA;
    public CharacterData charB;
}

public class LogManager : MonoBehaviour
{

    /// <summary>
    /// 会話の表示処理
    /// </summary>

    [Header("ログのプレハブ")]
    public GameObject logLinePrefab;
    public Transform logParent;

    [Header("参照")]
    public ChatManager_nextTurn turnManager;

    [Header("セリフ・画像表示場所")]
    public TextMeshProUGUI nameArea;
    public TextMeshProUGUI dialogueArea;
   // public Transform charaImgParent;
    [SerializeField] private List<RawImage> charaImg;
    public List<RawImage> CharaImg => charaImg;

    private List<string> logLines = new();
    private List<GameObject> logLineObjects = new(); // 各行のUIプレハブ保持


    // 会話ログを追加・UIを更新
    public void AddLine(string res)
    {
        string[] str = res.Split('：', ':');
        string line = str[0] + "：\n" + str[1];

        logLines.Add(line);

        GameObject obj = Instantiate(logLinePrefab, logParent);
        logLineObjects.Add(obj); // ★ UIオブジェクト保存

        TMP_Text logText = obj.transform.GetChild(0).GetComponent<TMP_Text>();
        Button retryButton = obj.transform.GetChild(1).GetChild(0).GetComponent<Button>();

        logText.text = line;

        RectTransform contentRect = logParent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentRect.sizeDelta.y + 250f);

        int index = logLines.Count - 1; // ログのインデックス（何ターン目のセリフか）を取得
        retryButton.onClick.RemoveAllListeners(); // 古いリスナーを消す
        // 再生ボタンを押したときに、そのインデックスの行を再生成
        retryButton.onClick.AddListener(() => RegenerateLine(index)); 
    }

    // 特定の行を再生成（後続も一括修正したければ、ここでトリガー）
    public void RegenerateLine(int index)
    {
        Debug.Log($"再生成リクエスト：{index}行目");

        turnManager.RegenerateTurnAt(index, logLines, (newLine, updatedList) =>
        {
            logLines = updatedList;

            // ★ UIテキストだけ書き換え
            TMP_Text logText = logLineObjects[index].transform.GetChild(0).GetComponent<TMP_Text>();
            logText.text = newLine;

            // もしセリフに応じてボタンやスタイル変更があればここで処理
        });
    }

    public List<string> GetLogLines() => new(logLines);


    // テキストエリアにセリフを表示
    public void ShowDialogue(string res)
    {
        string[] str = res.Split('：', ':');

        nameArea.text = str[0];
        dialogueArea.text = str[1];
    }

}

