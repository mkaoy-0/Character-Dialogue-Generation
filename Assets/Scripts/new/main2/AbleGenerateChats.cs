using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbleGenerateChats : MonoBehaviour
{
    // キャラ2人つくったよ

    public GameObject[] charaDataObj = new GameObject[2];
    public Button generateChatsBtn;
    public GameObject CMManualWindow, GCManualWindow, SceneManualWindow;

    [SerializeField]private bool _GCBtnIsClicked = false;
    public bool GCBtnIsClicked => _GCBtnIsClicked;

    private bool PWindowClosed = false;
    public GameObject PWindow;

    // キャラ編集画面の×ボタンを押したとき
    public void CanStartChats()
    {
        if (charaDataObj[0].activeSelf && charaDataObj[1].activeSelf)
        {
            CMManualWindow.SetActive(false);
            generateChatsBtn.interactable = true;
            if (!_GCBtnIsClicked)
            {
                GCManualWindow.SetActive(true);
                SceneManualWindow.SetActive(true);
            }
        }
        else
        {
            CMManualWindow.SetActive(true);
        }
    }

    // 会話生成を一回でも行った
    public void GCBtnClick()
    {
        _GCBtnIsClicked = true;
    }


    // --------------------
    // 会話生成後、吹き出し押して会話表示させたときに
    // 「性格推定ボタン押してね」ウィンドウを表示させる
    public void ShowPWindow()
    {
        if (!PWindowClosed) PWindow.SetActive(true);      
    }
    public void PWCloseBtnClicked()
    {
        PWindowClosed = true;
    }
}
