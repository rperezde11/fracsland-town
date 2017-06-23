using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Utils;
using Quests;

/// <summary>
/// A very simple script to show the player it's progress
/// </summary>
public class WinScreen : MonoBehaviour {

	private Button nextButton;
    public GameObject fishingBoat;

	void Start () {
		ShowEndGameDialog ();
	}

	void ShowEndGameDialog(){
		int numQuests = PlayerPrefs.GetInt (Constants.STORAGE_NUM_QUESTS);
		float beguinTime = PlayerPrefs.GetFloat (Constants.STORAGE_BEGUIN_TIME);
		int numErrors = PlayerPrefs.GetInt (Constants.STORAGE_NUM_FAILS);
		Text txtNumQuests, txtNumErrors, txtTime;
		txtNumQuests = GameObject.Find ("txtNumQuests").GetComponent<Text> ();
		txtNumErrors = GameObject.Find ("txtNumErrors").GetComponent<Text> ();
		txtTime = GameObject.Find ("txtTime").GetComponent<Text> ();
		txtNumErrors.text = numErrors.ToString ();
		txtNumQuests.text = numQuests.ToString ();
		txtTime.text = ((Mathf.Round((Time.time - beguinTime) / 60f * 100) / 100)).ToString ();

		GameObject.Find ("EndGameDialog").GetComponent<UIElement> ().show ();
	}

    void Update()
    {
        fishingBoat.transform.Translate(transform.forward * Time.deltaTime * 6f);
    }
}
