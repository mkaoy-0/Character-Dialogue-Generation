using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateChara : MonoBehaviour
{
    /// <summary>
    /// キャラを生成、更新
    /// </summary>
    /// 

    [Header("Character Prefab")]
    public GameObject characterPrefab;
    public Transform characterParent;


    [Header("UI Input Fields")]
    public List<FieldEntry> inputFields;  // インスペクタで設定する（key: "name" など）


    [Header("性格スライダー")]
    public StatusSlider statusSlider;

    private Dictionary<string, TMP_InputField> characterFields = new();
    // キャラの情報
    private List<ChatCharacter> _characters = new();
    public List<ChatCharacter> characters => _characters;

    // 編集対象キャラ
    private ChatCharacter currentSelectedCharacter = null;

    private GPTforPersonalitySlider gptPersonalSlider;

    private void Awake()
    {
        gptPersonalSlider = FindObjectOfType<GPTforPersonalitySlider>();
    }

    private void Start()
    {
        // 入力フィールドを辞書に登録（入力欄が空なら無視）
        foreach (var entry in inputFields)
        {
            if (!string.IsNullOrWhiteSpace(entry.key) && entry.field != null)
            {
                characterFields[entry.key] = entry.field;
            }
        }
    }

    public Dictionary<string, TMP_InputField> GetCharacterFields()
    {
        return characterFields;
    }

    /*/
    // ボタンから呼び出し
    public void CreateOrUpdateCharacterFromUI()
    {
        if (currentSelectedCharacter == null)
        {
            // まだ誰も選択されていなければ → 新規作成
            CreateCharacterFromUI();
            Debug.Log("新キャラ生成");
        }
        else
        {
            // すでにキャラが選択されていれば → 情報更新
            UpdateSelectedCharacter();
            Debug.Log("キャラ更新");
        }
    }
    /*/
    // ==================================================================
    /*/
    // 新キャラ作成
    public void CreateCharacterFromUI()
    {
        string name = GetInputOrDefault("名前", "未定");

        Dictionary<string, string> attributes = new();
        foreach (var kv in characterFields)
        {
            if (kv.Key == "名前") continue; // 名前は別管理
            attributes[kv.Key] = GetInputOrDefault(kv.Key, "未定");
        }

        CharacterData data = new CharacterData(name, attributes);
        CreateCharacter(data);
        // インプットフィールドの中身をクリア
        foreach (var field in characterFields.Values)
        {
            field.text = "";
        }

        gptPersonalSlider.AnalyzePersonality(data);
    }
    /**/
    public CharacterData CreateCharaFromInput()
    {
        string name = GetInputOrDefault("名前", "未定");
        Dictionary<string, string> attributes = new();
        foreach (var kv in characterFields)
        {
            if (kv.Key == "名前") continue;
            attributes[kv.Key] = GetInputOrDefault(kv.Key, "未定");
        }

        CharacterData data = new CharacterData(name, attributes);
        CreateCharacter(data);
        // インプットフィールドの中身をクリア
        foreach (var field in characterFields.Values)
        {
            field.text = "";
        }
        return data;
    }


    // 入力されたテキストを取得
    private string GetInputOrDefault(string key, string defaultValue)
    {
        return characterFields.ContainsKey(key) && characterFields[key] != null &&
               !string.IsNullOrWhiteSpace(characterFields[key].text)
            ? characterFields[key].text
            : defaultValue;
    }

    // キャラを生成
    public void CreateCharacter(CharacterData data)
    {
        GameObject obj = Instantiate(characterPrefab, characterParent); // プレハブを生成

        // X軸を -400～500 の範囲でランダムに設定（ローカル座標）
        float randomX = Random.Range(-400f, 500f);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, 0f);

        ChatCharacter chatChar = obj.GetComponent<ChatCharacter>();
        chatChar.Setup(data, this);
        _characters.Add(chatChar);
    }

    /*/
    // キャラ情報更新
    public void UpdateSelectedCharacter()
    {
        if (currentSelectedCharacter == null) return;

        string name = GetInputOrDefault("名前", currentSelectedCharacter.GetData().characterName);
        Dictionary<string, string> attributes = new();
        foreach (var kv in characterFields)
        {
            if (kv.Key == "名前") continue;
            attributes[kv.Key] = GetInputOrDefault(kv.Key, "未定");
        }
        statusSlider.SaveFromSliders(currentSelectedCharacter.GetData()); // スライダー値更新
        // データを更新
        currentSelectedCharacter.UpdateData(new CharacterData(name, attributes));
    }
    /**/

}
