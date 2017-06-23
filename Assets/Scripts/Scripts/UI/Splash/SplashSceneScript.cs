using UnityEngine;
using Utils;
using UnityEngine.UI;

/// <summary>
/// Manages all the login inside the splash scene.
/// </summary>
public class SplashSceneScript : MonoBehaviour {

	Text guiTxtNick, guiTxtMoney, guiTxtPlayerLevel, WelcomeTextPlayerName;

	void Start () {

		guiTxtNick = GameObject.Find("txtNick").GetComponent<Text>();
		WelcomeTextPlayerName = GameObject.Find ("TextPlayerName").GetComponent<Text> ();

        guiTxtNick.text = DataManager.instance.NickName;

        WelcomeTextPlayerName.text = DataManager.instance.NickName;

		JukeBox.instance.Play(JukeBox.instance.weather[0], 0.3f);
    }

	public void Update(){

	}

	public void LoadTownLevel(){
		Application.LoadLevel (Constants.SCENE_NAME_TOWN);
	}

	public void quitGame(){
		Application.Quit ();
	}

    public void changeLanguage(int lang)
    {
        GameController.instance.changeLanguage(lang);
    }
}
