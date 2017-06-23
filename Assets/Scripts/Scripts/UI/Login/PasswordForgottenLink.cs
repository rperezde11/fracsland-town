using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using Utils;

/// <summary>
/// Opens a webbrowser to allow the player recover a forgotten password
/// </summary>
public class PasswordForgottenLink : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    Text _text;

    public void OnPointerClick(PointerEventData eventData)
    {
        string url = Constants.PASSWORD_REQUEST_LINK_PRODUCTION;
        if (Constants.DEVELOPMENT)
        {
            url = Constants.PASSWORD_REQUEST_LINK_DEVELOPMENT;
        }
        Application.OpenURL(url);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = Color.black;
    }

    // Use this for initialization
    void Start () {
        _text = GetComponent<Text>();
        _text.color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
