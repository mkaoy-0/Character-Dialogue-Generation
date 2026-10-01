using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class RC_AddButtonManager : MonoBehaviour
{
    public Button addButton;
    public GameObject tmp_if;
    TMP_InputField tmpif;

    public void OnThisButtonClick()
    {
        tmp_if.SetActive(!tmp_if.activeSelf);
    }

    private void Update()
    {
        tmpif = tmp_if.GetComponent<TMP_InputField>();
        addButton.interactable = !string.IsNullOrWhiteSpace(tmpif.text);
    }
}
