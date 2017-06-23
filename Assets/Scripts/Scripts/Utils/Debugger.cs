using UnityEngine;
using System.Collections.Generic;
using Utils;

/// <summary>
/// Used to display some info in the screen it can be accessed by clicking 
/// F9 in dev mode (see contants)
/// </summary>
public class Debugger : MonoBehaviour {

    public static Debugger instance;

    private bool is_visible = false;

    private Dictionary<string, string> debuggingInfo;

    private Rect debuggingWindowRect = new Rect(20, 20, 320, 30);

    int lineHeigth = 20;
    void Awake()
    {
        if(instance == null)
        {
            debuggingInfo = new Dictionary<string, string>();
            instance = this;
        }
    }


	void Update () {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            is_visible = !is_visible;
        }
	}

    /// <summary>
    /// Used to show userfull debug data on the screen
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    public void addDebugginInfo(string key, string val)
    {
        if (debuggingInfo.ContainsKey(key))
        {
            debuggingInfo[key] = val;
        }
        else
        {
            debuggingWindowRect.height += lineHeigth;
            debuggingInfo.Add(key, val);
        }
        Debug.Log("Registered:" + key + " : " + val);
    }

    void OnGUI()
    {
        if (Constants.DEVELOPMENT && is_visible)
        {
            debuggingWindowRect = GUI.Window(0, debuggingWindowRect, debugInfoWindow, "Debug Info");
        }
    }

    void debugInfoWindow(int windowID)
    {
        int i = 0;

        foreach (KeyValuePair<string, string> info in debuggingInfo)
        {
            Debug.Log(info.Value);
            GUI.Label(new Rect(10, 20 + i * lineHeigth, 300, 20), info.Key + " : " + info.Value);
            i++;
        }

        GUI.DragWindow();
    }
}
