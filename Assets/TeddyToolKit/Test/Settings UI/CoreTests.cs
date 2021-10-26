using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeddyToolKit.Core;
using TeddyToolKit.Settings;
using TeddyToolKit.UI;

/// <summary>
/// just a test class to demonstrate the uimanager component of the framework
/// </summary>
public class CoreTests : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //watch ui keypresses
        TeddyToolKit.UI.UiManager.Instance.UIKeyPress();
    }
}
