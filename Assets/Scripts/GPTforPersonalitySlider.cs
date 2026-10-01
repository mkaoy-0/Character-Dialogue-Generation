using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTforPersonalitySlider : MonoBehaviour
{
    private ChatManager chatManager;
    private StatusSlider statusSlider;


    private void Awake()
    {
        chatManager = FindObjectOfType<ChatManager>();
        statusSlider = FindObjectOfType<StatusSlider>();
    }

    // 性格に基づき、スライダーの値設定
    // キャラ作成ボタンから呼び出し
    public void AnalyzePersonality(CharacterData target)
    {
        string personality = target.GetAttribute("設定", "");
        Debug.Log("==================== 設定 ===================");
        Debug.Log(personality);

        if (string.IsNullOrWhiteSpace(personality)) return;

        string prompt = $@"以下のキャラクターの性格に基づき、各性格軸について -50～50 の数値で評価してください。
評価する軸は以下の7つです：
1. 理性感情軸（-50 = 冷静に判断する、50 = 感情で動く）  
2. 内外向軸（-50 = 一人を好む、50 = 人と関わりたがる）  
3. 厳格柔軟軸（-50 = 厳しいルールを好む、50 = 自由さを重視）  
4. 静騒軸（-50 = 非常に落ち着いている、50 = 非常にテンション高め）  
5. 協調独立軸（-50 = 周囲に合わせる、50 = 我が道を行く）  
6. 慎重衝動軸（-50 = 計画を重視、50 = 直感的に行動）  
7. 信疑軸（-50 = 他人を信じやすい、50 = 警戒心が強い）  

それぞれの評価は「軸名: 数値」という形式で、1行ずつ出力してください。
例：
理性感情軸: 30  
内外向軸: -10  
...

キャラ性格：{personality}";

        List<ChatGPTClient.MessageModel> messages = new()
        {
            new ChatGPTClient.MessageModel { role = "user", content = prompt }
        };

        chatManager.chatGPTClient.GetResponse(messages, (response) =>
        {
            statusSlider.ApplyFromGPT(response);
            statusSlider.SaveFromSliders(target);
        });
    }

    // スライダーの基づくキャラの性格
    public string GeneratePersonalityBySlider(CharacterData characterData)
    {
        Dictionary<string, float> sliderValues = characterData.personalityScores;
        // スライダーの情報をテキスト化
        string sliderSummary = GenerateSliderSummary(sliderValues);

        return sliderSummary;
    }


    private string GenerateSliderSummary(Dictionary<string, float> scores)
    {
        Dictionary<string, (string low, string high)> axisHints = new()
        {
            { "理性感情軸", ("冷静に判断", "感情で動く") },
            { "内外向軸", ("一人を好む", "人と関わりたがる") },
            { "厳格柔軟軸", ("厳格", "柔軟") },
            { "静騒軸", ("落ち着き", "テンション高") },
            { "協調独立軸", ("周囲に合わせる", "自分の道を行く") },
            { "慎重衝動軸", ("計画を重視", "直感的に行動") },
            { "信疑軸", ("信じやすい", "警戒心強い") },
        };

        List<string> lines = new();
        foreach (var kv in scores)
        {
            if (axisHints.TryGetValue(kv.Key, out var hint))
            {
                lines.Add($"・{kv.Key}（低：{hint.low}／高：{hint.high}）: {kv.Value:F0}");
            }
            else
            {
                lines.Add($"・{kv.Key}: {kv.Value:F0}");
            }
        }

        Debug.Log(string.Join("\n", lines));
        return "性格は以下のスライダーの値に基づきます:\n各スライダー軸は -50～50 の値をとり、以下の傾向を表します:\n" + string.Join("\n", lines);
    }

}
