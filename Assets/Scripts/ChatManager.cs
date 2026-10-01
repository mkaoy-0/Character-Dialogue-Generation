using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class FieldEntry
{
    public string key;              // 例: "name", "gender"
    public TMP_InputField field;   // 実際のUIの入力欄
}

public class ChatManager : MonoBehaviour
{

    [Header("ChatGPT Client")]
    public ChatGPTClient chatGPTClient;

    [Header("世界観InputField")]
    public TMP_InputField worldSettings;

    public CreateChara createChara;
    private RelationShipSliderManager rsManager;
    private GPTforPersonalitySlider pSlider;

    private void Awake()
    {
        rsManager = FindObjectOfType<RelationShipSliderManager>();
        pSlider = FindObjectOfType<GPTforPersonalitySlider>();
    }

    // 会話処理
    public void StartConversation()
    {

        if (createChara.characters.Count < 2)
        {
            return;
        }

        CharacterData char1 = createChara.characters[0].GetData();
        CharacterData char2 = createChara.characters[1].GetData();

        string worldSetting = worldSettings != null && !string.IsNullOrWhiteSpace(worldSettings.text)
            ? worldSettings.text : "未定";

        List<ChatGPTClient.MessageModel> messages = new()
        {
            new ChatGPTClient.MessageModel
            {
                role = "system",
                content = $"あなたはキャラクター創作支援AIです。ユーザーの入力に基づき、キャラ設定や会話を創作的に補完してください。一人称・二人称、口調は指定されたものを必ず守ってください。\n"+
                          $"世界観:{worldSetting}"

            },
            new ChatGPTClient.MessageModel
            {
                role = "user",
                content = $"以下は{charaPromptText(char1)}と{charaPromptText(char2)}【{rsManager.GetEmotionPrompt(char1, char2)}】の会話です。" +
                           "キャラの設定を厳密に守り、会話内容を生成してください。ストーリー性よりもキャラクター同士の関係性にフォーカスした内容にしてください。"+
                           "サンプルボイスは口調や一人称・二人称の傾向を把握するための参考で、これをそのまま使わず、話し方の特徴のみを反映してください。"+
                           "キャラクターの口調や態度、感情表現、発言の方向性はスライダーの値によって大きく変化します。\n"+
                           "次のルールに厳密にしたがってください：\n"+
                           "最大で10ターン程度。出力は「キャラ名：セリフ」の形式で、キャラ名とセリフのみで構成してください。\n"+
                           "禁止事項：\n"+
                           "無理に仲良くしたり、協力したり、好意的に終わらせようとすること。キャラの性格、一人称、二人称の変更"
            }
        };

        chatGPTClient.GetResponse(messages, (response) =>
        {
            Debug.Log("=== 会話ログ ===");
            Debug.Log(response);
            // TODO: ここでUI表示に渡して表示
        });

        Debug.Log("==== スライダーの値 ====");
        Debug.Log($"{rsManager.GetEmotionPrompt(char1, char2)}");
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
}


