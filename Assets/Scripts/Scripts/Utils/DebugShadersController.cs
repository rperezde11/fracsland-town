using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugShadersController : MonoBehaviour {

    public enum SHADERS_TYPE
    {
        NORMAL,
        TOON,
        TOON_BUMPED,
        BUMPED
    };

    private SHADERS_TYPE type = SHADERS_TYPE.NORMAL;

    private Dropdown dropdownShaders;

    void Start ()
    {
        ShowDebugShadersWindow();
	}

    public static void ChangeShader()
    {

    }

    public void ShowDebugShadersWindow()
    {
        if (!dropdownShaders)
        {
            LoadDebugShadersWindow();
        }

        if (dropdownShaders)
        {
            dropdownShaders.enabled = true;
        }
    }

    public void HideDebugShadersWindow()
    {
        if (dropdownShaders)
        {
            dropdownShaders.enabled = false;
        }
    }

    public void ToggleDebugShadersWindow()
    {
        if (dropdownShaders)
        {
            if (dropdownShaders.enabled)
            {
                HideDebugShadersWindow();
            }
            else
            {
                ShowDebugShadersWindow();
            }
        }
    }

    private void LoadDebugShadersWindow()
    {
        GameObject parent = Instantiate(Resources.Load("UI/DebugShaders")) as GameObject;

        dropdownShaders = parent.GetComponentInChildren(typeof(Dropdown), false) as Dropdown;

        dropdownShaders.ClearOptions();
        dropdownShaders.AddOptions(new List<string>(Enum.GetNames(typeof(SHADERS_TYPE))));
    }
}
