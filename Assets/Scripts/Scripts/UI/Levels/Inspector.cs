using UnityEngine;
using UnityEngine.UI;
using Quests;

/// <summary>
/// Used to know some info about the entity that the player is pointing with the mouse.
/// </summary>
public class Inspector : UIElement {

    GameObject caller;
    public static Inspector instance = null;
    private float  _width;

    void Awake()
    {
        if(instance == null)
            instance = this;
        _width = GetComponent<RectTransform>().sizeDelta.x;
    }
    
	// Update is called once per frame
	void Update () {
        if (!caller && isVisible)
        {
            hide();
            GameController.instance.MouseOverObject = null;
        }

        if (Input.mousePosition.x < _width)
        {
            transform.position = Input.mousePosition + Vector3.right * _width / 2;
        }
        else
        {
            transform.position = Input.mousePosition - Vector3.right * _width / 2;
        }
   
	}

    public void ShowInspector(GameObject inspected)
    {
        if (QuestManager.instance.ActualQuest != null && QuestManager.instance.ActualQuest.isPlayerSolvingTheQuest) return;
        caller = inspected;
        GameController.instance.MouseOverObject = inspected;
        show();
    }
}
