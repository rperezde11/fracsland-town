using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Utils;
using SimpleJSON;

/// <summary>
/// A list of avaliable games.
/// </summary>
public class AvaliableGamesList : MonoBehaviour {

    public GameObject itemPrefab;
    public GameObject playButton;
    public int gameSelectedID = -1;

    // Use this for initialization
    void Start()
    {
        GetGames();
    }

    public void GetGames()
    {
        DataManager.instance.Api.ShowLoadingDialog();
        ClearGameList();
        DataManager.instance.Api.sendGETRequest(Constants.getAPIURL(Constants.API_ACTIVE_GAMES), API.AUTH_TYPE.HARD, RenderGames, null, null, null);
    }

    void ClearGameList()
    {
        gameSelectedID = -1;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("UIActiveGame"))
        {
            Destroy(go);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GetGames();
        }

        if(gameSelectedID == -1)
        {
            playButton.GetComponent<UIElement>().hide();
        }
        else
        {
            playButton.GetComponent<UIElement>().show();
        }
    }
    
    public void GetGameConfiguration()
    {
        DataManager.instance.Api.sendGETRequest(Constants.getAPIURL(Constants.API_GET_CONFIG, gameSelectedID), API.AUTH_TYPE.HARD, DataManager.instance.ParseGameConfiguration, null, null, null);
	}

    void RenderGames(WWW www)
    {
        var json = JSON.Parse(www.text);

        var gamesList = json["games_data"].AsArray;

        int itemCount = gamesList.Count;

        RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform>();
        RectTransform containerRectTransform = gameObject.GetComponent<RectTransform>();
        float height = rowRectTransform.rect.height;

        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.offsetMax.x, itemCount * rowRectTransform.GetComponent<LayoutElement>().minHeight + GetComponent<VerticalLayoutGroup>().spacing * itemCount);

        for (int i = 0; i < itemCount; i++)
        {
            var game = gamesList[i];
            //create a new item, name it, and set the parent
            GameObject newItem = Instantiate(itemPrefab) as GameObject;
            newItem.GetComponent<AvaliableGame>().SetGameData(game["game_id"].AsInt, game["classroom"], game["date"], this);
            newItem.transform.parent = gameObject.transform;
        }

        DataManager.instance.Api.HideLoadingDialog ();
    }
}
