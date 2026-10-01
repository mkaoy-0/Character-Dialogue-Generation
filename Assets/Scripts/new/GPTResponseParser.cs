using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;  // NuGetパッケージ or Unity Package Managerで導入

// GPTが返すJSONを受け取るクラス
[Serializable]
/*/
public class DialogueSet
{
    public string scene;
    public List<string> dialogue;
}
/**/
public class DialogueSet
{
    public string scene;
    public string sceneDescription;
    public List<string> dialogue;
}

// 会話を「話者」と「セリフ」に分けるためのクラス
[Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

// キャラ説明文と外見情報に分けるためのクラス
[Serializable]
public class CharaDescriptionSet
{
    public string personalityDescription;
    public string appearanceDescription;
}


public static class GPTResponseParser
{
    /// <summary>
    /// JSON文字列を DialogueSet に変換
    /// </summary>
    /// 
    /*/
    public static DialogueSet ParseResponse(string jsonResponse)
    {
        jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "");
        jsonResponse = jsonResponse.Replace("”", "\"");
        jsonResponse = jsonResponse.Trim();
        try
        {
            DialogueSet data = JsonConvert.DeserializeObject<DialogueSet>(jsonResponse);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("JSONパース失敗: " + e.Message);
            return null;
        }
    }
    /**/
    public static T ParseResponse<T>(string jsonResponse) where T : class
    {
        // 余計な記号や全角文字を除去
        jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "");
        jsonResponse = jsonResponse.Replace("”", "\"").Replace("“", "\"");
        jsonResponse = jsonResponse.Trim();

        try
        {
            T data = JsonConvert.DeserializeObject<T>(jsonResponse);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"JSONパース失敗: {e.Message}\n入力: {jsonResponse}");
            return null;
        }
    }

    /// <summary>
    /// dialogue を「話者: セリフ」形式から分割してリスト化
    /// </summary>
    public static List<DialogueLine> ParseDialogue(List<string> dialogueStrings)
    {
        List<DialogueLine> lines = new();

        foreach (string line in dialogueStrings)
        {
            string[] parts = line.Split(new char[] { ':' }, 2);

            if (parts.Length == 2)
            {
                lines.Add(new DialogueLine
                {
                    speaker = parts[0].Trim(),
                    text = parts[1].Trim()
                });
            }
            else
            {
                // ":" がない場合はセリフだけ
                lines.Add(new DialogueLine
                {
                    speaker = "？？？",
                    text = line
                });
            }
        }

        return lines;
    }
}

public class SceneData
{
    public string scene;                   // シーン説明
    //
    public string sceneDescription;
    /**/
    public List<DialogueLine> dialogues;   // 会話リスト
}

public static class SceneRepository
{
    private static List<SceneData> scenes = new();

    // シーン追加
    public static void AddScene(DialogueSet dialogueSet)
    {
        // dialogue を行ごとに分割して保存
        List<DialogueLine> lines = GPTResponseParser.ParseDialogue(dialogueSet.dialogue);

        scenes.Add(new SceneData
        {
            scene = dialogueSet.scene,
            //
            sceneDescription = dialogueSet.sceneDescription,
            /**/
            dialogues = lines
        });
    }

    // シーン検索
    public static SceneData GetScene(string sceneDescription)
    {
        return scenes.Find(s => s.scene == sceneDescription);
    }

    // 保存してあるシーン一覧を取る
    public static List<SceneData> GetAllScenes()
    {
        return scenes;
    }
}

