using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName; // 名前だけは別管理
    public Dictionary<string, string> attributes = new(); // 名前以外の設定
    public Dictionary<string, float> personalityScores = new(); // 性格スライダー値の保存先（"理性感情軸" => 数値）
    public Texture2D imgTexture = null; // 画像

    // =============================-
    public CharacterData(string name, Dictionary<string, string> attr)
    {
        characterName = name;
        attributes = attr;
    }
    public string GetAttribute(string key, string defaultValue = "不明")
    {
        return attributes.ContainsKey(key) ? attributes[key] : defaultValue;
    }

    // =============================-
    // 性格スライダー
    public float GetScore(string key, float defaultValue = 0f)
    {
        return personalityScores.ContainsKey(key) ? personalityScores[key] : defaultValue;
    }
    public void SetScore(string key, float value)
    {
        personalityScores[key] = value;
    }

    // =============================-
    // 画像
    public Texture2D GetImgTex()
    {
        return imgTexture;
    }
    public void SetImgTex(Texture2D tex)
    {
        imgTexture = tex;
    }
}

[System.Serializable]
public class RelationshipData
{
    public string character1;
    public string character2;
    public float c1Toc2Emotion; // -1 to 1
    public float c2Toc1Emotion; // -1 to 1

    public RelationshipData(string chara1, string chara2)
    {
        character1 = chara1;
        character2 = chara2;
        c1Toc2Emotion = 0f;
        c2Toc1Emotion = 0f;
    }
}

