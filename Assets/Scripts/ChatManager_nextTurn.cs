using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatManager_nextTurn : MonoBehaviour
{
    [Header("世界観InputField")]
    public TMP_InputField worldSettings;

    private CreateChara createChara;
    private RelationShipSliderManager rsManager;
    private GPTforPersonalitySlider pSlider;
    private ChatManager chatManager;
    private LogManager logManager;

    int currentTurn = 0;
    const int maxTurns = 10;
    bool isChar1Turn = true; // true = char1のターン、false = char2のターン

    private List<string> previousTurns = new();

    void Start()
    {
        chatManager = FindObjectOfType<ChatManager>();
        worldSettings = chatManager.worldSettings;
        rsManager = FindObjectOfType<RelationShipSliderManager>();
        pSlider = FindObjectOfType<GPTforPersonalitySlider>();
        createChara = FindObjectOfType<CreateChara>();
        logManager = FindObjectOfType<LogManager>();
    }


    // プロンプト生成
    private string charaPromptText(CharacterData chara)
    {
        List<string> descs = new();
        foreach (var kv in chara.attributes)
        {
            descs.Add($"{kv.Key}:{kv.Value}");
        }

        string sliderSummary = pSlider.GeneratePersonalityBySlider(chara);

        return $"{chara.characterName}（{string.Join("、", descs)} | {sliderSummary}）";
    }

    public void GenerateNextTurn(CharacterData char1, CharacterData char2)
    {
        if (currentTurn >= maxTurns) return;

        string speakerName = isChar1Turn ? char1.characterName : char2.characterName;
        string responderName = isChar1Turn ? char2.characterName : char1.characterName;

        string worldSetting = worldSettings != null && !string.IsNullOrWhiteSpace(worldSettings.text)
            ? worldSettings.text : "未定";

        string userPrompt = $@"キャラA: {charaPromptText(char1)} | キャラB: {charaPromptText(char2)}
                         {rsManager.GetEmotionPrompt(char1, char2)}

                         前のターンまでのセリフ:
                         {string.Join("\n", previousTurns)}

                         次に話すのは「{speakerName}」。このキャラが直前の流れを踏まえて発言するとしたら、次の形式で出力してください：
                         {speakerName}：～～～～

                         ※ 出力はセリフのみ、前後に余計な説明をつけないでください。";

        string systemPrompt = $@"あなたは創作支援AIです。指定されたキャラの設定をもとに会話を生成してください。
                                 一人称・二人称、口調は指定されたものを必ず守ってください。
                                 キャラクターの口調や態度、感情表現、発言の方向性はスライダーの値によって大きく変化します。
                                 出力はセリフのみ、前後に余計な説明をつけないでください。
                                 世界観: {worldSetting}";

        List<ChatGPTClient.MessageModel> messages = new()
        {
            new ChatGPTClient.MessageModel { role = "system", content = systemPrompt },
            new ChatGPTClient.MessageModel { role = "user", content = userPrompt }
        };

        chatManager.chatGPTClient.GetResponse(messages, (response) =>
        {
            previousTurns.Add(response.Trim());
            currentTurn++;
            isChar1Turn = !isChar1Turn;

            // UIに表示 or 次ターン生成ボタンをアクティブにする
            Debug.Log(response);
            logManager.AddLine(response);
            logManager.ShowDialogue(response);
        });
    }


    // 任意の行を再生成
    public void RegenerateTurnAt(int index, List<string> currentLogs, System.Action<string, List<string>> onDone)
    {
        // キャラ情報取得
        var char1 = createChara.characters[0].GetData();
        var char2 = createChara.characters[1].GetData();

        bool isChar1 = (index % 2 == 0);
        string speakerName = isChar1 ? char1.characterName : char2.characterName;

        // 前ターンまでを渡す
        string pastLines = string.Join("\n", currentLogs.GetRange(0, index));

        string worldSetting = worldSettings != null && !string.IsNullOrWhiteSpace(worldSettings.text)
            ? worldSettings.text : "未定";

        string userPrompt = $@"キャラA: {charaPromptText(char1)} | キャラB: {charaPromptText(char2)}
                         {rsManager.GetEmotionPrompt(char1, char2)}

                         前のターンまでのセリフ:
                         {pastLines}

                         次に話すのは「{speakerName}」。このキャラが直前の流れを踏まえて発言するとしたら、次の形式で出力してください：
                         {speakerName}：～～～～

                         ※ 出力はセリフのみ、前後に余計な説明をつけないでください。";

        string systemPrompt = $@"あなたは創作支援AIです。指定されたキャラの設定をもとに会話を生成してください。
                                 一人称・二人称、口調は指定されたものを必ず守ってください。
                                 キャラクターの口調や態度、感情表現、発言の方向性はスライダーの値によって大きく変化します。
                                 出力はセリフのみ、前後に余計な説明をつけないでください。
                                 世界観: {worldSetting}";

        List<ChatGPTClient.MessageModel> messages = new()
        {
            new ChatGPTClient.MessageModel { role = "system", content = systemPrompt },
            new ChatGPTClient.MessageModel { role = "user", content = userPrompt }
        };

        chatManager.chatGPTClient.GetResponse(messages, (response) =>
        {
            Debug.Log(response);
            currentLogs[index] = response.Trim();

            // 再生成後に必要なら後続ターンを削除・再生成も可
            // currentLogs.RemoveRange(index + 1, currentLogs.Count - (index + 1));

            onDone(response.Trim(), currentLogs);
        });
    }


}
