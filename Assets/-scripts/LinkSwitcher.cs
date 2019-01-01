using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkSwitcher : MonoBehaviour
{
    private InterfaceManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<InterfaceManager>();
    }

    public void SwitchLink()
    {
        uiManager.LinkSwitchMain(gameObject.name);
    }
}
