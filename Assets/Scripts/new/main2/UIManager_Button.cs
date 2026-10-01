using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager_Button : MonoBehaviour
{
    public List<GameObject> obj;

    public void ActivateObj()
    {
        if (obj == null) return;

        foreach(var o in obj)
        {
            o.SetActive(!o.activeSelf);
        }
    }
}
