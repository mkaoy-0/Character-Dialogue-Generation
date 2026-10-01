using UnityEngine;
using UnityEngine.UI;

public class ImageVisualizeManager : MonoBehaviour
{
    private CharaData _data;
    public CharaData data { set { _data = value; } }

    private RawImage _raw;
    public RawImage raw { set { _raw = value; } }

    // ‰æ‘œ‚ğİ’è
    public void ShowImg()
    {
        _raw.texture = _data.tex;
        _raw.SetNativeSize();
        Debug.Log("‰æ‘œ‚ğ•\¦");
    }

}
