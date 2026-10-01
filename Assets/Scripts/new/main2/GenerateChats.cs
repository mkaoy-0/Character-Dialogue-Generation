using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateChats : MonoBehaviour
{
    public GPTManager gpt;
    public CharaDatabase db;
    public CharaEditor editor;
  //  public UIManager_SceneSb uiManeger;
 //   public GraphClickHandler graph;
 //   public SceneSettingsController sceneSettings;
    public TextMeshProUGUI chara1Name, chara2Name;
    public TMP_InputField if_worldSetting;
    public TMP_InputField if_situation;
 //   public Button generateButton;

//    private bool isRunning = false; // 自動生成中かどうかのフラグ
 //   private Coroutine autoCoroutine; // 実行中のコルーチンを保持

//    private int targetCount = 3;
//    [SerializeField] private int generatedCount = 0;

    public CreatePatternUI createPatternUI;
    private string[] _patternArray = new string[3];
    //   public string[] ParrentArray => _patternArray;

    public UIManager_SceneSb uiManager;

    public GeneratePersonality generatePersonality;

    public Button generateChatButton;

    public GameObject loadingText;

    public GameObject sbClickManual;

    public AbleGenerateChats ableGenerateChats;

    public CSVSavingManager csvSavingManager;


    public void GeneratePattern()
    {
        string worldSetting = if_worldSetting.text;
        string systemContent = $"You are an assistant who creates characters for stories. All output must be in Japanese.";

        string userContent = $"キャラ2人の会話を生成する。" + 
            $"シーン「{if_situation.text}」での会話のパターン（雰囲気）を3つ考えてください。3つは被らず、キャラの反応が全く違うものになるようにすること。" + "\n" +
            "[例：明るい、冷静、怒る、など]" +"\n" + 
            "各パターンはひとことで表すこと。" + "\n" +
           @"出力形式:
{
  ""pattern1"": ""パターン1"",
  ""pattern2"": ""パターン2"",
  ""pattern3"": ""パターン3"",
}

- 出力は有効なJSONオブジェクト1つのみ
-JSON以外の文章、説明、コードブロック記号(```json や ```) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。";

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            Debug.Log("受け取った返答: " + result);
            PatternSet data = GPTResponseParser.ParseResponse<PatternSet>(result);
            // Debug.Log($"パターン：{data.pattern1} | {data.pattern2} | {data.pattern3}");
            _patternArray[0] = data.pattern1;
            _patternArray[1] = data.pattern2;
            _patternArray[2] = data.pattern3;

            createPatternUI.ActivatePatternUI(_patternArray);

          //  GenerateChatsPaterns();
        });
    }


    public void GenerateChatsPaterns()
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

        string worldSetting = string.IsNullOrEmpty(if_worldSetting.text)
                ? ""
              : $"世界観：{if_worldSetting.text}";

        string systemContent = "You are an assistant that generates creative dialogue scenes. All output must be in Japanese.出力はJSON形式。"
                               +  $"世界観:{worldSetting}";

        string chara1RCText = editor.CharaRadarChartText(chara1);
        string chara2RCText = editor.CharaRadarChartText(chara2);

        string chara1Personality = generatePersonality.GetPersonalityList(chara1.name);
        string chara2Personality = generatePersonality.GetPersonalityList(chara2.name);

        string chartDes = "BigFiveの値。5に近いほど値が高い。値は性格などに大きな影響を与える。";
        string cD = "";
        if (!generatePersonality.PersonalDictIsEmpty(chara1.name) && !generatePersonality.PersonalDictIsEmpty(chara2.name))
        {
            cD = $"(【{chara1.name}|{chara1RCText}】【{chara2.name}|{chara2RCText}】({chartDes}))";
        }else
        {
            cD = "";
        }


        string _sceneSituation = $"3つの会話は全て{if_situation.text}という場面にすること。場面は変えず、雰囲気や内容をそれぞれ全く異なるものにすること。";
        string sceneSit = null;
        if (!string.IsNullOrEmpty(if_situation.text)) sceneSit = _sceneSituation;

        


        /*/
        string userContent = $"以下のJSON形式で{if_situation.text}という場面の会話セットを1つ出力してください。" +
@"
[
  {
    ""scene"": ""パターン1"",
    ""dialogue"": [
      ""キャラ名: セリフ1"",
      ""キャラ名: セリフ2"",
      ...
    ]
  },
  {
    ""scene"": ""パターン2"",
    ""dialogue"": [
      ""キャラ名: セリフ1"",
      ""キャラ名: セリフ2"",
      ...
    ]
  },
  {
    ""scene"": ""パターン3"",
    ""dialogue"": [
      ""キャラ名: セリフ1"",
      ""キャラ名: セリフ2"",
      ...
    ]
  },
]

- 出力は必ず上記の配列のみ。
-上記のJSON以外の文章、説明、コードブロック記号(```json や ```、scenes:[]など) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
" +
"条件：\n" +
$"- sceneは順に{string.Join(" , ", _patternArray)}にすること。" + "\n" +
"- 登場人物は次の2人\n" +
$" ・1人目「{chara1.name}」 | {editor.CharaStatusText(chara1)}\n" +
$" ・2人目「{chara2.name}」 | {editor.CharaStatusText(chara2)}\n" +
@"
- キャラの設定を厳密に守り、会話内容を生成してください。
- キャラの性別、年齢は絶対に変更しないこと。
- 性格はユニークなものを自由に想像してください。ただし一貫させ、そのキャラの性格が表れるような会話内容にすること。
- 口調や一人称、二人称は統一すること。" + 
$"- **必ず{if_situation.text}という内容で、各sceneに沿った会話・発言にし、他のsceneとは全く異なる内容や雰囲気にすること。**" + 
@"- 無理に仲良くしたり、協力したり、好意的に終わらせようとすることは禁止。
- dialogueは4～8ターン
";
        /**/

        string userContent = $"以下のJSON形式で会話セットを1つ出力してください。" +
@"
[
  {
    ""scene"": ""シーン名"",
    ""sceneDescription"": ""説明"",
    ""dialogue"": [
      ""キャラ名: セリフ1"",
      ""キャラ名: セリフ2"",
      ...
    ]
  },
  {
    ""scene"": ""シーン名"",
    ""sceneDescription"": ""説明"",
    ""dialogue"": [
      ""キャラ名: セリフ1"",
      ""キャラ名: セリフ2"",
      ...
    ]
  },
  {
    ""scene"": ""シーン名"",
    ""sceneDescription"": ""説明"",
    ""dialogue"": [
      ""キャラ名: セリフ1"",
      ""キャラ名: セリフ2"",
      ...
    ]
  },
]

- 出力は必ず上記の配列のみ。
-上記のJSON以外の文章、説明、コードブロック記号(```json や ```、scenes:[]など) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
" +
"条件：\n" +
"- 登場人物は次の2人\n" +
$" ・1人目「{chara1.name}」 | {editor.CharaStatusText(chara1)}\n【{chara1Personality}】" +
$" ・2人目「{chara2.name}」 | {editor.CharaStatusText(chara2)}\n【{chara2Personality}】" +
$" {cD}" + "\n" +
$"キャラ名には、{chara1.name}か{chara2.name}をそのまま記入してください。\n" + 
@"
- キャラの設定を厳密に守り、会話内容を生成してください。
- キャラの性別、年齢は絶対に変更しないこと。" +
// $"- 各々の性格：【{chara1Personality}】【{chara2Personality}】\n" + 
// $" {cD}" + "\n" +
// @"- 性格の指定がない場合は3つの会話セットで全く異なるものにし、ユニークなものを自由に想像してください。ただし一貫させ、そのキャラの性格が表れるような会話内容にすること。
@"- 口調や一人称、二人称は統一すること。
- そのキャラの性格が表れるような会話内容にすること。" + "\n" +
$"- シーン設定：{sceneSit}" + "\n" +
@"- シーン設定の指定がない場合、**3つの会話セットは様々な場面・展開にし、それぞれ全く異なる内容や雰囲気にすること。**
- scene はdialogue をひとことで表した内容にすること。他sceneと被るものは禁止。
- sceneDescription は会話の場面の簡単な説明を書いてください。
- 無理に仲良くしたり、協力したり、好意的に終わらせようとすることは禁止。
- dialogueは4～8ターン"
;

        Debug.Log("【会話】入力プロンプト: " + userContent);
        generateChatButton.interactable = false;

        // 
        // CSV書き出し
        csvSavingManager.WriteCsv("会話生成ボタンを押した");
        csvSavingManager.WriteCsv($"{chara1.name},{editor.CharaStatusText(chara1)}");
        csvSavingManager.WriteCsv($"{chara2.name},{editor.CharaStatusText(chara2)}");
        if (!string.IsNullOrEmpty(if_worldSetting.text)) csvSavingManager.WriteCsv($"世界観,{if_worldSetting.text}");
        if (!string.IsNullOrEmpty(if_situation.text)) csvSavingManager.WriteCsv($"シーン設定,{if_situation.text}");
        csvSavingManager.WriteCsv("以下、生成した会話");
        /**/

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            if (loadingText != null) loadingText.SetActive(false);

            Debug.Log("受け取った返答: " + result);
            // JSONをDialogueSetに変換
            List<DialogueSet> data = GPTResponseParser.ParseResponse<List<DialogueSet>>(result);


            if (data != null)
            {
                int _i = 0;
                foreach (var d in data)
                {
                    SceneRepository.AddScene(d);
                    //            
                    _patternArray[_i] = d.scene;
                    _i++;
                    /**/

                    //
                    // csv
                    csvSavingManager.WriteCsv("シーン名");
                    csvSavingManager.WriteCsv($"{d.scene},{d.sceneDescription}");
                    SceneData selected = SceneRepository.GetScene(d.scene);
                    foreach (var line in selected.dialogues)
                    {
                        csvSavingManager.WriteCsv($"{line.speaker},{line.text}");
                    }
                    /**/
    }
                //
                createPatternUI.ActivatePatternUI(_patternArray); 
                /**/
                for (int i = 0; i < _patternArray.Length; i++)
                {
                    int index = i;

                    createPatternUI.patternUI[index].interactable = true;
                    createPatternUI.patternUI[index].onClick.AddListener(() =>
                    {
                        uiManager.PanelVisualizer();
                        uiManager.OnSceneButtonClick(_patternArray[index]);
                    });
                }

                // マニュアル提示
                if (sbClickManual != null && !ableGenerateChats.GCBtnIsClicked)
                {
                    sbClickManual.SetActive(true);
                }
                /**/
            }
            ableGenerateChats.GCBtnClick();
            generateChatButton.interactable = true;
        });
    }

    /**/
}



[Serializable]
public class PatternSet
{
    public string pattern1;
    public string pattern2;
    public string pattern3;
}