using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueImageSetting : MonoBehaviour
{
    public List<RawImage> dialogueRaws;
    public List<RawImage> basedRaws;

    private void Update()
    {
        if (dialogueRaws.Count == basedRaws.Count)
        {
            for (int i = 0; i < dialogueRaws.Count; i++)
            {
                if (dialogueRaws[i].texture != basedRaws[i].texture)
                {
                    dialogueRaws[i].texture = basedRaws[i].texture;
                }
            }
        }
    }
}
