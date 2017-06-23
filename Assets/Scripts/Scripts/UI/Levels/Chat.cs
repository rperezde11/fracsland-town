using UnityEngine;
using UnityEngine.UI;
using Utils;
using Items;
using Quests;


/// <summary>
/// Used to show an input where the developper can 
/// introduce commands in order to obtain, wood or fractions.
/// </summary>
public class Chat : UIElement {

	private Text guiChatText;
	private Scrollbar chatScrollbar;
	private InputField chatInput;
	Camera mainCamera;

    string messagesPendingToshow;


    void Start () {
		chatInput = GameObject.Find ("ChatInput").GetComponent<InputField> ();
		mainCamera = Camera.main;
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Return)){
			sendMessage();
		}
        chatInput.Select();
    }

    public void showMessageOnScreen(string newMessage, bool gameMessage = false){
    }

	public void sendMessage(){
		if (isValidMessage()) {
            if (chatInput.text.Substring(0, 1).Equals("/"))
            {
                processCommand(chatInput.text);
            }
            else
            {
                showMessageOnScreen(chatInput.text);
            }
            resetChatInput ();
		}
    }

    public void processCommand(string command)
    {
        //get all the blocs
        string[] commandBlocs = command.Substring(1, command.Length - 1).Split(' ');
        if(commandBlocs[0].Equals("wood"))
        {
            QuestManager.instance.Inventory.addItem(Item.Wood, int.Parse(commandBlocs[1]));
            showMessageOnScreen("Cheat: Added " + commandBlocs[1] +" of " + commandBlocs[0], true);
        }
        else if(commandBlocs[0].Equals("fraction"))
        {
            QuestManager.instance.Inventory.addItem(new Fraction(Constants.ID_FRACTION, "Fraction", "inventory_fraction", int.Parse(commandBlocs[1]), int.Parse(commandBlocs[2])), 1);
            showMessageOnScreen("Cheat: Added" + commandBlocs[0], true);
        }
		else if(commandBlocs[0].Equals("restart"))
		{
			Application.LoadLevel(Constants.SCENE_NAME_BEACH);
		}
    }

	public void resetChatInput(){
		chatInput.text = "";
    }

    public bool areMessagesPendingToShow()
    {
        if (messagesPendingToshow.Length > 0)
        {
            return true;
        }
        return false;
    }

    public void addMessage(string newMessage)
    {
        messagesPendingToshow += "\n" + newMessage + "\n";
    }

    public bool isValidMessage(){
		if (chatInput.text != "") {
			return true;
		}
		return false;
	}
    
}
