using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Shows the detail of an avaliable game in the splash screen, allowing the player to 
/// join into this game.
/// </summary>
public class AvaliableGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Image _backgroundImage;
    private Color _initialColor, _hoverColor, _selectedColor;
    private AvaliableGamesList _gameList;
    private bool _selected = false;
    private int _id;
    private string _name, _date;
    private Text _txtDate, _txtName;

    void Awake()
    {
        _backgroundImage = GetComponent<Image>();
        _initialColor = _backgroundImage.color;
        _selectedColor = Color.yellow;
        _hoverColor = Color.white;
        transform.FindChild("imgSelected").GetComponent<UIToggleable>().hide();
        _txtDate = transform.FindChild("txtDate").GetComponent<Text>();
        _txtName = transform.FindChild("txtClass").GetComponent<Text>();
    }
     
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!_selected) _backgroundImage.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_selected) _backgroundImage.color = _initialColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("UIActiveGame"))
        {
            AvaliableGame ag = go.GetComponent<AvaliableGame>();
            ag.Disselect();
        }
        Select();
    }

    public void Disselect()
    {
        _backgroundImage.color = _initialColor;
        _selected = false;
        transform.FindChild("imgSelected").GetComponent<UIToggleable>().hide();
    }

    public void Select()
    {
        _backgroundImage.color = _selectedColor;
        _selected = true;
        transform.FindChild("imgSelected").GetComponent<UIToggleable>().show();
        _gameList.gameSelectedID = _id;
    }
    
    public void SetGameData(int id, string name, string date, AvaliableGamesList avaliableGameList)
    {
        _id = id;
        _txtName.text = name;
        _txtDate.text = date;
        _gameList = avaliableGameList;
    }
}
