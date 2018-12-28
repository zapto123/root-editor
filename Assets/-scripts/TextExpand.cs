using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
public class TextExpand : MonoBehaviour
{
    private RectTransform rt;
    private Text txt;
    public InterfaceManager uiManager;
    void Start ()
    {
        uiManager = FindObjectOfType<InterfaceManager>();
        print(uiManager.gameObject.name);
        rt = gameObject.GetComponent<RectTransform>(); // Acessing the RectTransform 
        txt = gameObject.GetComponent<Text>(); // Accessing the text component
    }

    public void Resize()
    {
        rt.sizeDelta = new Vector2(rt.rect.width, txt.preferredHeight); // Setting the height to equal the height of text
    }

    public void Rewrite()
    {
        uiManager.EditMainText(Int32.Parse(transform.parent.parent.name), txt.text);
    }
}
