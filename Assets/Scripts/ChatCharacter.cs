using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChatCharacter : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// プレハブで生成したキャラについて
    /// クリックしたときにデータを引き出す
    /// (キャラクリック時に呼ばれる)
    /// </summary>
    /// 

    public TextMeshProUGUI nameText;
    public Image avatarImage;

    private CharacterData data;
    private CreateChara createChara;
    private CharacterSelector characterSelector;

    private void Awake()
    {
        characterSelector = FindObjectOfType<CharacterSelector>();
    }

    public void Setup(CharacterData characterData, CreateChara createChara)
    {
        data = characterData;
        nameText.text = data.characterName;
        // avatarImage.sprite = 任意の画像を設定

        this.createChara = createChara;
    }

    public CharacterData GetData()
    {
        return data;
    }

    // キャラをクリックしたとき
    public void OnPointerClick(PointerEventData eventData)
    {
        characterSelector.OnCharacterClicked(this);
    }

    // キャラ情報更新
    public void UpdateData(CharacterData newData)
    {
        this.data = newData;
        nameText.text = newData.characterName; // 表示も更新
    }

}
