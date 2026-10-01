using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationShipSliderManager : MonoBehaviour
{
    /// <summary>
    /// 関係値スライダー
    /// </summary>
    /// 

    public Slider slider1to2;
    public Slider slider2to1;

    private string character1;
    private string character2;
    private CreateChara createChara;

    // 関係値スライダー
    private Dictionary<(string, string), RelationshipData> relationshipMap = new();

    public void Setup(string a, string b, CreateChara manager)
    {
        character1 = a;
        character2 = b;
        createChara = manager;

        slider1to2.onValueChanged.AddListener(OnSliderChanged);
        slider2to1.onValueChanged.AddListener(OnSliderChanged);

        // 初期値を関係に即時反映
        OnSliderChanged(0f);
    }

    private void OnSliderChanged(float _)
    {
        // Debug.Log($"スライダー変更: {character1}→{character2}={slider1to2.value}, {character2}→{character1}={slider2to1.value}");
        // createChara.UpdateEmotion(character1, character2, slider1to2.value, slider2to1.value);
        UpdateEmotion(character1, character2, slider1to2.value, slider2to1.value);
    }


    // スライダー
    public void UpdateEmotion(string chara1, string chara2, float c1Toc2, float c2Toc1)
    {
        var key = (chara1, chara2);
        if (!relationshipMap.ContainsKey(key))
        {
            relationshipMap[key] = new RelationshipData(chara1, chara2);
        }

        relationshipMap[key].c1Toc2Emotion = c1Toc2;
        relationshipMap[key].c2Toc1Emotion = c2Toc1;
    }
    public RelationshipData GetRelationship(string chara1, string chara2)
    {
        if (relationshipMap.TryGetValue((chara1, chara2), out var data))
            return data;
        if (relationshipMap.TryGetValue((chara2, chara1), out data))
            return data;

        return null;
    }


    public string GetEmotionPrompt(CharacterData c1, CharacterData c2)
    {
        string chara1 = c1.characterName;
        string chara2 = c2.characterName;
        var rel = GetRelationship(chara1, chara2);
        if (rel == null)
        {
           // Debug.LogWarning($"{c1.characterName}と{c2.characterName}の関係データが存在しません");
        }
        else
        {
            Debug.Log($"関係値: {rel.c1Toc2Emotion}, {rel.c2Toc1Emotion}");
        }

        if (rel == null) return "";

        return $"{chara1}が{chara2}に抱いている感情の値は{rel.c1Toc2Emotion:F2}、{chara2}が{chara1}に抱いている感情値は{rel.c2Toc1Emotion:F2}（-1:冷たく敵意のある対応、1:親しみや信頼のある対応）";
    }

}
