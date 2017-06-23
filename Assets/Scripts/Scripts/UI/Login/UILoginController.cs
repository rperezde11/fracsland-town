using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utils;

/// <summary>
/// Manages the logic of the login screen
/// </summary>
public class UILoginController : MonoBehaviour {

    InputField txtEmail, txtPassword;
    UIElement loginError;

    public static UILoginController instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

	// Use this for initialization
	void Start () {

        txtEmail = GameObject.Find("txt_email").GetComponent<InputField>();
        txtPassword = GameObject.Find("txt_password").GetComponent<InputField>();
        EventSystem.current.SetSelectedGameObject(txtEmail.gameObject, null);
        txtEmail.OnPointerClick(new PointerEventData(EventSystem.current));

        loginError = GameObject.Find("BadCredentials").GetComponent<UIElement>();
        DataManager.instance.Api.createLoadingDialog();

		if (Constants.DEVELOPMENT) {
			txtEmail.text = "test@fracsland.com";
			txtPassword.text = "1234";
		}
		JukeBox.instance.Play(JukeBox.instance.weather[0], 0.3f);
    }

    public void Update()
    {
    }

    public void showLoginError()
    {
        loginError.show();
    }

    public void hideLoginError()
    {
        loginError.hide();
    }

    public void changeLanguage(int lang)
    {
        LanguageManager.instance.ActualLang = lang;
    }
}
