using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditChats : MonoBehaviour
{
    public GPTManager gpt;
    public CharaDatabase db;
    public CharaEditor editor;

    public GenerateChats generateChats;

    public TextMeshProUGUI chara1Name, chara2Name;

    public UIManager_SceneSb uiManager;

    private string nowSceneName;

    [SerializeField] private int[] reGenerateCount = new int[3];
    [SerializeField] private int[] currentDialogueIndex = new int[3];

    public GameObject changeButton;

    public void ReGenerateChats()
    {
        if (string.IsNullOrEmpty(chara1Name.text) || string.IsNullOrEmpty(chara2Name.text))
        {
            Debug.Log("名前がない");
            return;
        }

        CharaData chara1 = db.GetCharaByName(chara1Name.text);
        CharaData chara2 = db.GetCharaByName(chara2Name.text);

        if (chara1 == null || chara2 == null)
        {
            Debug.Log("キャラデータが見つからない");
            return;
        }

        if (uiManager == null)
        {
            Debug.LogError("uiManager がアサインされていません！");
            return;
        }

        string _scene = uiManager.CurrentSceneName;
        if (string.IsNullOrEmpty(_scene))
        {
            Debug.LogError("CurrentSceneName が空です！");
            return;
        }

        SceneData selectedScene = SceneRepository.GetScene(_scene);
        if (selectedScene == null)
        {
            Debug.LogError($"SceneRepository に {_scene} が見つかりません！");
            return;
        }
        if (selectedScene.sceneDescription == null)
        {
            Debug.LogError($"{_scene} の sceneDescription が null です！");
        }

        string worldSetting = string.IsNullOrEmpty(generateChats.if_worldSetting.text)
                ? ""
              : $"世界観：{generateChats.if_worldSetting.text}";

        string systemContent = "You are an assistant that generates creative dialogue scenes. All output must be in Japanese.出力はJSON形式。"
                               + $"世界観:{worldSetting}";
        string userContent = $"以下のJSON形式で会話セットを1つ出力してください。" +
@"
{
  ""dialogue"": [
    ""キャラ名: セリフ1"",
    ""キャラ名: セリフ2"",
    ...
]


- 出力は必ず上記の配列のみ。
-上記のJSON以外の文章、説明、コードブロック記号(```json や ```、scenes:[]など) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
" +
"条件：\n" +
"- 登場人物は次の2人\n" +
$" ・1人目「{chara1.name}」 | {editor.CharaStatusText(chara1)}\n" +
$" ・2人目「{chara2.name}」 | {editor.CharaStatusText(chara2)}\n" +
$"- {_scene}({selectedScene.sceneDescription})という場面に沿った会話にすること。\n" + 
@"
- キャラの設定を厳密に守り、会話内容を生成してください。
- キャラの性別、年齢は絶対に変更しないこと。
- 性格はユニークなものを自由に想像してください。ただし一貫させ、そのキャラの性格が表れるような会話内容にすること。
- 口調や一人称、二人称は統一すること。" +
@"- 無理に仲良くしたり、協力したり、好意的に終わらせようとすることは禁止。
- dialogueは4～8ターン"
;

        Debug.Log("【会話】入力プロンプト: " + userContent);

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            reGenerateCount[uiManager.CurrentSceneNum]++;

            Debug.Log("受け取った返答: " + result);
            // JSONをDialogueSetに変換
            DialogueSet data = GPTResponseParser.ParseResponse<DialogueSet>(result);

            if (data != null)
            {
                data.scene = $"{_scene}{reGenerateCount[uiManager.CurrentSceneNum]}";
                Debug.Log($"{data.scene} | base={_scene}, index={reGenerateCount[uiManager.CurrentSceneNum]}");
               
                data.sceneDescription = selectedScene.sceneDescription;
                SceneRepository.AddScene(data);
                //
                uiManager.OnSceneButtonClick(data.scene);
                nowSceneName = data.scene;
                uiManager.CurrentSceneName = nowSceneName;
            }

            if (!changeButton.activeSelf) changeButton.SetActive(true);
        });
    }


    public void ChangeDialogue()
    {
        string baseScene = uiManager.CurrentSceneName;
        int sceneNum = uiManager.CurrentSceneNum;
        int max = reGenerateCount[sceneNum];

        currentDialogueIndex[sceneNum]--;
        if (currentDialogueIndex[sceneNum] < 0)
            currentDialogueIndex[sceneNum] = max;

        // 末尾の数字（例：1, 2, 3など）を除去して「街の公園」に戻す
        baseScene = System.Text.RegularExpressions.Regex.Replace(baseScene, @"\d+$", "");

        // シーン名の組み立て
        string sceneName = (currentDialogueIndex[sceneNum] == 0)
            ? baseScene
            : $"{baseScene}{currentDialogueIndex[sceneNum]}";

        SceneData selectedScene = SceneRepository.GetScene(sceneName);
        if (selectedScene == null)
        {
            Debug.LogWarning($"Scene '{sceneName}' が見つかりません。");
            Debug.Log($"base={baseScene} | index={currentDialogueIndex[sceneNum]}");
            return;
        }

        uiManager.OnSceneButtonClick(selectedScene.scene);
        nowSceneName = selectedScene.scene;
        uiManager.CurrentSceneName = nowSceneName;

        Debug.Log($"会話を切り替えました: {sceneName}");
    }

}
