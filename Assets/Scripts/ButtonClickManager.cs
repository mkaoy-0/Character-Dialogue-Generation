using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickManager : MonoBehaviour
{
    private CreateChara createChara;
    private GPTforPersonalitySlider gptManager;
    private CharacterSelector characterSelector;
    private StatusSlider statusSlider;
    private ChatManager_nextTurn chatManager_byTurn;
    private LogManager logManager;
    private PromptForGenerateImage promptForImg;
    private DalleImageGenerator dalleImageGenerator;


    private void Start()
    {
        createChara = FindObjectOfType<CreateChara>();
        gptManager = FindObjectOfType<GPTforPersonalitySlider>();
        characterSelector = FindObjectOfType<CharacterSelector>();
        statusSlider = FindObjectOfType<StatusSlider>();
        chatManager_byTurn = FindObjectOfType<ChatManager_nextTurn>();
        logManager = FindObjectOfType<LogManager>();
        promptForImg = FindObjectOfType<PromptForGenerateImage>();
        dalleImageGenerator = FindObjectOfType<DalleImageGenerator>();
    }

    // キャラ作成ボタンから呼び出す
    public void OnCreateCharacterButtonClicked()
    {
        CharacterData newCharacter = createChara.CreateCharaFromInput(); // キャラ生成
        gptManager.AnalyzePersonality(newCharacter);  // GPTで性格分析

        /*/
        // プロンプト生成 → 画像生成へ
        promptForImg.GenerateImagePrompt(newCharacter, (string prompt) =>
        {
            StartCoroutine(dalleImageGenerator.GenerateImageFromPrompt(newCharacter, prompt));
        });
        /**/
    }


    // キャラ更新時の処理
    // 同じくボタンから呼び出す
    public void OnUpdateCharacterButtonClicked()
    {
        var selected = characterSelector.GetSelected();
        if (selected != null)
        {
            statusSlider.SaveFromSliders(selected.GetData());
        }
    }


    // 会話生成ボタン
    // 1ターンずつ生成
    public void GenerateConversation()
    {
        // CharacterData char1 = createChara.characters[0].GetData();
        // CharacterData char2 = createChara.characters[1].GetData();
        List<CharacterData> _chara = characterSelector.GetConversationChara();
        CharacterData char1 = _chara[0];
        CharacterData char2 = _chara[1];

        dalleImageGenerator.ShowCharaImage(logManager.CharaImg[0], char1);
        dalleImageGenerator.ShowCharaImage(logManager.CharaImg[1], char2);

        chatManager_byTurn.GenerateNextTurn(char1, char2);
    }

}
