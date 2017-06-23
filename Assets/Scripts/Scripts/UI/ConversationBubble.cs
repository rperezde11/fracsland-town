using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// Used to display a dialog over a game object.
/// </summary>
public class ConversationBubble : MonoBehaviour {

    public const float DISSAPPEAR_TIME = 5f;
    private float _dissapearTime = 0f;

	public static ConversationBubble instance;
	string _stringToRender;
	Text _text;
	float _letterSpeed = 0.3f;
	GameObject _objectToFollow;
	UIElement _uiElement;
	CanvasGroup _group;
	float _nextLetterSpawn;
	public bool hasfinishedShowingText = false;
	private bool _hideDialog = false;

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		_text = transform.FindChild("Text").GetComponent<Text> ();
		_uiElement = GetComponent<UIElement> ();
		_group = GetComponent<CanvasGroup> ();
		_nextLetterSpawn = 0f;
	}


	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(_dissapearTime < Time.time)
        {
            _hideDialog = true;
        }
        
		if (_hideDialog) {
			_objectToFollow = null;
			_uiElement.hide ();
		}
		if (_objectToFollow != null) {
			SetPosition ();
			if (_stringToRender.Length > 0 && _nextLetterSpawn < Time.time) {
				_text.text =_text.text + _stringToRender.Substring (0, 1);
				_stringToRender = _stringToRender.Substring (1, _stringToRender.Length - 1);
				_nextLetterSpawn = Time.time + _letterSpeed;
				if (_stringToRender.Length == 0)
                {
					hasfinishedShowingText = true;
                }
			}
		}
	}

	void SetPosition()
	{
		Vector3 positionToDisplay = Camera.main.WorldToScreenPoint (_objectToFollow.transform.position) - new Vector3 (Screen.width / 2, Screen.height / 2, 0) + new Vector3(0, 121f, 0);
		transform.localPosition = positionToDisplay;
	}

	public void ShowText(string textToShow, GameObject objectToFollow){
		_hideDialog = false;
		hasfinishedShowingText = true;
		_text.text = "";
		_uiElement.show ();
		_group.blocksRaycasts = false;
		_objectToFollow = objectToFollow;
		_text.text = textToShow;
		_stringToRender = textToShow;
	}

	public void ShowProgressiveText(string textToShow, GameObject objectToFollow, float letterSpeed=0.3f)
	{
		_hideDialog = false;
		hasfinishedShowingText = false;
		_text.text = "";
		_uiElement.show ();
		_group.blocksRaycasts = false;
		_stringToRender = textToShow;
		_objectToFollow = objectToFollow;
		_letterSpeed = letterSpeed;
		_nextLetterSpawn = 0;
        _dissapearTime = Time.time + letterSpeed * textToShow.Length + DISSAPPEAR_TIME;
	}

	public void HideDialog()
	{
		_hideDialog = true;
		hasfinishedShowingText = true;
	}
}
