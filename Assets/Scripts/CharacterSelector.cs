using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{

    [Header("会話生成ボタン")]
    public GameObject StartConvButton;


    [Header("各設定セット")]
    public GameObject charaSettingsSet;
    public GameObject relationShipSettingsSet;
    public RawImage rawImg;

    // スクリプト
    private CreateChara createChara;
    private RelationShipSliderManager rsSliderManager;
    private StatusSlider statusSlider;
    private DalleImageGenerator ImgGenerator;

    private Dictionary<string, TMP_InputField> characterFields = new();

    // クリックしたキャラの情報
    private List<ChatCharacter> selectedCharacters = new List<ChatCharacter>();
    // 編集対象キャラ
    private ChatCharacter currentSelectedCharacter = null;





    private void Start()
    {
        createChara = FindObjectOfType<CreateChara>();
        rsSliderManager = FindObjectOfType<RelationShipSliderManager>();
        statusSlider = FindObjectOfType<StatusSlider>();
        ImgGenerator = FindObjectOfType<DalleImageGenerator>();

        // characterFields を CreateChara から受け取る
        characterFields = createChara.GetCharacterFields();
    }

    // キャラをクリックしたときの処理
    public void OnCharacterClicked(ChatCharacter clicked)
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            // Shiftが押されていたら追加選択
            if (!selectedCharacters.Contains(clicked))
                selectedCharacters.Add(clicked);
        }
        else
        {
            // 通常クリックで選択をリセット
            selectedCharacters.Clear();
            selectedCharacters.Add(clicked);
            // クリックしたキャラの情報を表示
            ClickedCharaData(clicked);
        }

        // 2体選ばれていたらパネルを表示
        if (selectedCharacters.Count >= 2)
        {
            StartConvButton.SetActive(true);

            charaSettingsSet.SetActive(false);
            relationShipSettingsSet.SetActive(true);
            // スライダーの値
            rsSliderManager.Setup(selectedCharacters[0].GetData().characterName, selectedCharacters[1].GetData().characterName, createChara);

            // インプットフィールドリセット
            ClearInputFields();
            currentSelectedCharacter = null;
        }
        else
        {
            StartConvButton.SetActive(false);

            charaSettingsSet.SetActive(true);
            relationShipSettingsSet.SetActive(false);
        }
    }

    // クリックしたキャラの情報を表示
    private void ClickedCharaData(ChatCharacter clicked)
    {
        // キャラの情報を入力欄に反映
        var data = clicked.GetData();
        foreach (var kv in characterFields)
        {
            if (kv.Key == "名前") kv.Value.text = data.characterName;
            else if (data.attributes.ContainsKey(kv.Key))
                kv.Value.text = data.attributes[kv.Key];
            else
                kv.Value.text = "";
        }
        statusSlider.ApplyToSliders(data); // スライダー反映

        // キャラの画像表示
        ImgGenerator.ShowCharaImage(rawImg, data);

        currentSelectedCharacter = clicked;
    }

    // インプットフィールドの中身をクリア
    private void ClearInputFields()
    {
        foreach (var field in characterFields.Values)
        {
            field.text = "";
        }
    }

    // 現在選択中のキャラを取得
    public ChatCharacter GetSelected()
    {
        return selectedCharacters.Count == 1 ? selectedCharacters[0] : null;
    }

    // 会話するキャラを取得
    public List<CharacterData> GetConversationChara()
    {
        List<CharacterData> data = new List<CharacterData>();
        for(int i = 0; i < selectedCharacters.Count; i++)
        {
            data.Add(selectedCharacters[i].GetData());
        }

        return data;
    }

}
