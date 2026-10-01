using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusSlider : MonoBehaviour
{

    /// <summary>
    /// 性格スライダー
    /// </summary>
    /// 

    [System.Serializable]
    public class PersonalitySlider
    {
        public string key;       // 例: "理性感情"
        public Slider slider;    // Unity UIのスライダー
    }

    public List<PersonalitySlider> sliders = new();

    private Dictionary<string, Slider> sliderDict = new();

    [SerializeField] private Transform sliderParent;


    private void Awake()
    {
        sliders.Clear();
        for (int i = 0; i < sliderParent.childCount; i++)
        {
            Transform child = sliderParent.GetChild(i);
            Slider slider = child.GetComponentInChildren<Slider>(); // スライダーを取得
            if (slider != null)
            {
                string key = child.name; // オブジェクト名をキーとする（必要に応じて変更）
                sliders.Add(new PersonalitySlider { key = key, slider = slider });
            }
        }
        foreach (var s in sliders)
        {
            if (!string.IsNullOrWhiteSpace(s.key) && s.slider != null)
            {
                sliderDict[s.key] = s.slider;
            }
        }
    }


    public void SetSliderValue(string key, float value)
    {
        if (sliderDict.ContainsKey(key))
        {
            sliderDict[key].value = Mathf.Clamp(value, -50f, 50f);
        }
    }


    public void ApplyToSliders(CharacterData data)
    {
        foreach (var kv in sliderDict)
        {
            float value = data.GetScore(kv.Key, 0f);
            kv.Value.value = value;
        }
        Debug.Log("スライダー反映");
    }

    public void SaveFromSliders(CharacterData data)
    {
        foreach (var kv in sliderDict)
        {
            data.SetScore(kv.Key, kv.Value.value);
        }
    }

    public void ApplyFromGPT(string gptOutput)
    {
        Debug.Log(gptOutput);

        string[] lines = gptOutput.Split('\n'); // 行ごとに分ける
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // 空行をスキップ

            string[] parts = line.Split(':'); // 「軸名: 数値」という形式であることを期待して、: で分割
            if (parts.Length != 2) continue;  // [0] = 軸名, [1] = 数値

            string key = parts[0].Trim(); 
            if (float.TryParse(parts[1], out float value))
            {
                SetSliderValue(key, value);
            }
        }
    }

}
