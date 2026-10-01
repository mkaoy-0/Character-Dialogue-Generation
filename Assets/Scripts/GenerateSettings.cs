using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenerateSettings : MonoBehaviour
{
    public ChatGPTClient chatGPTClient;


    public TMP_InputField nameInputField;
    public TMP_InputField personalityInputField;
    public TMP_InputField genderInputField;
    public TMP_InputField oldInputField;
    public TMP_InputField worldSettingInputField; // ← 追加！

    // あなたは創作アシスタントです。出力は日本語にしてください
    string systemPrompt = "You are a creative assistant.Always respond in Japanese.";

    // テンプレート
    List<ChatGPTClient.MessageModel> CreateMessageList(string userPrompt)
    {
        return new List<ChatGPTClient.MessageModel>
        {
            new ChatGPTClient.MessageModel { role = "system", content = systemPrompt },
            new ChatGPTClient.MessageModel { role = "user", content = userPrompt }
        };
    }

    // 世界観
    public void GenerateWorldSetting()
    {
        // 物語の世界観を完全に自由に考えてください。ジャンル、時代設定、文明の発展度、社会構造、文化、主要な特徴的な要素などを含めて、物語の舞台となる世界全体の概要を200-250文字程度で簡潔にまとめてください。箇条書きで書いて。
        string userPrompt = "Please generate a fully original fictional world setting. You are free to choose the genre, historical period, level of civilization, society structure, culture, and any unique features. The setting should be around 200 - 250 Japanese characters.Write it down in bullet points.";
        List<ChatGPTClient.MessageModel> messages = CreateMessageList(userPrompt);

        chatGPTClient.GetResponse(messages, (response) =>
        {
            Debug.Log("=== 世界観設定 ===");
            Debug.Log(response);
            worldSettingInputField.text = response;
            // ここに会話のUI表示処理など追加してOK
        });
    }

    // 性格
    public void GeneratePersonality()
    {
        string userPrompt = $"{worldSettingInputField}という世界観の物語に登場するキャラの性格を考えてください。名前や性別など性格以外の情報は一切不要です。出力は４～５行の箇条書きにしてください。";
        List<ChatGPTClient.MessageModel> messages = CreateMessageList(userPrompt);

        chatGPTClient.GetResponse(messages, (response) =>
        {
            Debug.Log("=== キャラ性格 ===");
            Debug.Log(response);
            personalityInputField.text = response;
            // ここに会話のUI表示処理など追加してOK
        });
    }

    // 名前
    public void GenerateName()
    {
        string gender = genderInputField != null && !string.IsNullOrWhiteSpace(genderInputField.text)
            ? genderInputField.text : "undecided"; // デフォルトは"未定"

        string userPrompt = $"{worldSettingInputField}という世界観の物語に登場するキャラの名前を1つ考えてください。世界観に合った名前にしてください（和風なら日本人名、洋風ならカタカナ名など）。性別は{gender}です。出力は名前一語のみにしてください（例：名前：○○」のような記述は禁止）。「」の記載も禁止。";
        List<ChatGPTClient.MessageModel> messages = CreateMessageList(userPrompt);

        chatGPTClient.GetResponse(messages, (response) =>
        {
            Debug.Log("=== 名前 ===");
            Debug.Log(response);
            nameInputField.text = response;
            // ここに会話のUI表示処理など追加してOK
        });
    }

}
