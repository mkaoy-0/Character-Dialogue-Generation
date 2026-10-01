using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GenerateChatScene : MonoBehaviour
{
    public GPTManager gpt;
    public CharaDatabase db;
    public CharaEditor editor;
    public UIManager_SceneSb uiManeger;
    public GraphClickHandler graph;
    public SceneSettingsController sceneSettings;
    public TextMeshProUGUI chara1Name, chara2Name;
    public TMP_InputField if_worldSetting;
    public Button generateButton;

    private int targetCount = 10;
    [SerializeField] private int generatedCount = 0;

    private bool isRunning = false; // 自動生成中かどうかのフラグ
    private Coroutine autoCoroutine; // 実行中のコルーチンを保持


    public int TargerCount => targetCount;
    public int GeneratedCount { get { return generatedCount; } set { generatedCount = value; } }


    public void StartAutoGenerate(int count)
    {
        if (isRunning) return; // 二重起動防止
        generateButton.interactable = false;
        targetCount = count;
        generatedCount = 0;
        isRunning = true;
        autoCoroutine = StartCoroutine(AutoGenerateRoutine());
    }

    // 自動生成停止（リセットボタンから呼ぶ）
    public void StopAutoGenerate()
    {
        if (!isRunning) return;
        isRunning = false;

        if (autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
            autoCoroutine = null;
        }

        generateButton.interactable = true;
        Debug.Log("自動生成を中断しました");
    }

    private IEnumerator AutoGenerateRoutine()
    {
        while (generatedCount < targetCount && isRunning)
        {
            bool done = false;

            GenerateChat((data) =>
            {
                if (data != null)
                {
                    if (!isRunning) return; // 停止後にコールバックが来ても無視
                    uiManeger.CreateSceneSb(data.scene);
                    SceneRepository.AddScene(data);
                }
                generatedCount++;
                done = true;
            });

            // GPTの返答を待つ
            yield return new WaitUntil(() => done || !isRunning);

            if (!isRunning) yield break; // フラグが落ちたら即終了
        }

        isRunning = false;
        generateButton.interactable = true;
        Debug.Log($"自動生成完了: {generatedCount}件");
    }

    private void GenerateChat(System.Action<DialogueSet> onComplete)
    {
        if (string.IsNullOrEmpty(chara1Name.text) || string.IsNullOrEmpty(chara2Name.text))
        {
            Debug.Log("名前がない");
            onComplete(null);
            return;
        }

        CharaData chara1 = db.GetCharaByName(chara1Name.text);
        CharaData chara2 = db.GetCharaByName(chara2Name.text);

        if (chara1 == null || chara2 == null)
        {
            Debug.Log("キャラデータが見つからない");
            onComplete(null);

            return;
        }

        ///
        string worldSetting = if_worldSetting.text;

        string sceneTexts = string.Join(" , ", SceneRepository.GetAllScenes().Select(s => s.scene));

        string systemContent = "You are an assistant that generates creative dialogue scenes. All output must be in Japanese.出力はJSON形式。" +
                               $"世界観:{worldSetting}";

        string chara1RCText = editor.CharaRadarChartText(chara1);
        string chara2RCText = editor.CharaRadarChartText(chara2);

        string sceneSettingsText = $"-(ラベル1){sceneSettings.label[0].text}/(ラベル2){sceneSettings.label[1].text}軸：{graph.GraphPosX}" + "\n" +
                                    "* 値が-5に近い場合: 落ち着いている、淡々としている、感情表現控えめ（世間話・相談など）\n" + 
                                    "* 値が5に近い場合: 感情が高ぶった声、テンション高い（大声で喜ぶ・怒鳴るなど）\n" + 
                                   $"-(ラベル3){sceneSettings.label[2].text}/(ラベル4){sceneSettings.label[3].text}軸：{graph.GraphPosY}" + "\n" +
                                    "* 値が-5に近い場合: 不満・怒り・落ち込み・愚痴・喧嘩などを含む\n" + 
                                    "* 値が5に近い場合: 前向き・明るい・元気・希望的な言葉\n" + 
                                 //  $"(-10に近いほどラベル1,3、10に近いほどラベル2,4に近い。)\n" +
                                  // "- 静か：落ち着いている、冷静。　激しい：感情が強く出ている。テンションが高い。\n" +
                                  // "- ネガティブ：後ろ向き、怒りや落ち込みを含むやりとり。　ポジティブ：前向き、明るい、希望的なやりとり。\n" +
                                   "指定された軸の値は、会話全体の雰囲気・展開、言葉遣いや感情の強さに強く影響を与える。セリフごとにこの数値の強さを必ず反映すること。**必ずこの数値を強く反映した会話を生成してください。**)\n";

        //
        string userContent = @"
以下のJSON形式で会話セットを1つ出力してください。

{
  ""scene"": ""シーン説明"",
  ""dialogue"": [
    ""キャラ名: セリフ1"",
    ""キャラ名: セリフ2"",
    ...
  ]
}

- 出力は必ず有効なJSONオブジェクト1つのみ。
-JSON以外の文章、説明、コードブロック記号(```json や ```) を出力しないこと。
-JSON内の文字列は必ずダブルクォートで囲むこと。
- フォーマットを絶対に崩さないでください。
" + 
"条件：\n" +
$"次のシーン設定軸の値に応じたシーンおよび会話内容にすること：\n{sceneSettingsText}" + 
"シーン名は一言で書き、色々な場面・展開を生成すること。\n" +
//"(例: 喧嘩している場面、楽しい会話場面、どこかに出かける、など。これらの例はそのまま使わず参考にとどめておくこと。)" + 
$"次に示す既存のシーン名と絶対に被らないようにすること：既存シーン[{sceneTexts}]\n" +
"登場人物は次の2人\n" +
$"・1人目「{chara1.name}」 | {chara1.description} \n[性別]: {chara1.charaStatus["性別"]}\n[年齢]: {chara1.charaStatus["年齢"]}\n[口調]: {chara1.charaStatus["口調"]}\n[性格]: \n{chara1RCText}\n" +
$"・2人目「{chara2.name}」 | {chara2.description} \n[性別]: {chara2.charaStatus["性別"]}\n[年齢]: {chara2.charaStatus["年齢"]}\n[口調]: {chara2.charaStatus["口調"]}\n[性格]: \n{chara2RCText}\n" +
"(性格は、各項目を0～5の数値で表したもの。値が大きいほど、その項目の強さが高いことを意味する) \n\n" +
@"
キャラの設定を厳密に守り、会話内容を生成してください。
**口調の文は絶対にコピーせず**、語尾や文体のみを参考にすること。
無理に仲良くしたり、協力したり、好意的に終わらせようとすることは禁止。会話の最後は**必ずシーン設定軸の値に沿ったトーンで締めること(ネガティブなら暗い、険悪など)。**キャラの性別、年齢、性格、一人称、二人称は絶対に変更しないこと。
**シーン設定軸の値に沿った会話内容・雰囲気・発言にすることを最優先にしてください。**
- dialogueは6～10ターン
";
        // ただし、キャラクター同士の関係や性格設定よりも、シーン設定軸の値を優先して会話の雰囲気を調整してください。
        // キャラ説明に矛盾しない発言のみ許可。
        // ストーリー性よりもキャラクター同士の関係性にフォーカスした内容にしてください。   

        /*/
        ,
        ""value"": {
            ""軸1ラベル - 軸2ラベル"": ""数値"",
             ""軸3ラベル - 軸4ラベル"": ""数値""
        }

        - 「value」はAIが生成した会話を見て、実際にどの程度軸の値が反映されているかを自己評価した結果を数値で書くこと。
        - 数値は-5～5で表すこと。-5に近いほど「ラベル1,3」側、5に近いほど「ラベル2,4」側に寄っていることを意味する。
        - 各軸ラベルの部分は与えたシーン設定軸のラベルに変えること。

        /**/

        /**/

        Debug.Log("【会話】入力プロンプト: " + userContent);

        gpt.RequestForGPT(systemContent, userContent, (result) =>
        {
            if (!isRunning) return; // 停止後は処理しない

            Debug.Log("受け取った返答: " + result);
            // JSONをDialogueSetに変換
            DialogueSet data = GPTResponseParser.ParseResponse<DialogueSet>(result);

            /*/
            if (data != null)
            {
                // シーン説明を表示
                uiManeger.CreateSceneSb(data.scene);
                // シーンを保存
                SceneRepository.AddScene(data);
                Debug.Log("シーン：" + data.scene);
            }
            /**/

            onComplete(data);

        });

    }
}
