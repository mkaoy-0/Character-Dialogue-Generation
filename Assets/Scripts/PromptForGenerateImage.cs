using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptForGenerateImage : MonoBehaviour
{
    [SerializeField] private ChatManager chatManager;

    void Start()
    {
      //  chatManager = FindObjectOfType<ChatManager>();
    }

    // 画像生成用のプロンプトを生成
    public void GenerateImagePrompt(CharacterData chara, System.Action<string> onPromptReady)
    {
        string worldSetting = chatManager.worldSettings != null && !string.IsNullOrWhiteSpace(chatManager.worldSettings.text)
            ? chatManager.worldSettings.text : "未定";

        List<ChatGPTClient.MessageModel> messages = new()
        {
            new ChatGPTClient.MessageModel
            {
                role = "user",
                content = $"以下のキャラ設定と世界観をもとに、DALL-Eで使えるキャラの立ち絵イラスト用のプロンプトを英語で作成してください。" +
                           "[キャラ設定]: " + charaPromptText(chara) +
                           "[世界観]: " + worldSetting +
                           "出力は英語のプロンプトのみ。前後に余計な説明をつけないでください。" +
                           "プロンプトには次の情報を書いてください: 性別、髪型、髪の長さ、髪色、瞳の色、服装、靴、表情" +
                           "性格や世界観の説明は不要です。外見に関する情報のみを使用してください。" +
                           "箇条書きではなく、自然な英文形式で記述してください。"
            }
        };

        chatManager.chatGPTClient.GetResponse(messages, (response) =>
        {
            Debug.Log("=== 外見情報 ===");
            // 末尾に立ち絵要素を強調する語句を追加
            response += "\nThe image must show only one full-body character, standing on a white background. The illustration must be in vertical composition. Do not repeat the character. Do not show multiple instances. No back view, no facial close-up, no variations. Only one full-body figure. A Japanese anime-style illustration.";
            Debug.Log(response);
            onPromptReady?.Invoke(response); // プロンプトができたらコールバック実行
        });
    }


    // プロンプト生成
    private string charaPromptText(CharacterData chara)
    {
        List<string> descs = new();
        foreach (var kv in chara.attributes)
        {
            descs.Add($"{kv.Key}:{kv.Value}");
        }

        return $"{chara.characterName}（{string.Join("、", descs)}）";
    }
}
