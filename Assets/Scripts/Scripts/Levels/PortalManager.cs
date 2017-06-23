using UnityEngine;
using System.Collections;

/// <summary>
/// Used to manage what portals we have on a level
/// </summary>
public class PortalManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (LevelController.instance != null) GUIController.instance.ShowZoneName();
        Destroy(gameObject);
    }

    public static GameObject GetPortalBySceneName(string sceneName)
    {
        var portals = GameObject.FindGameObjectsWithTag("portal");
        foreach(GameObject portal in portals)
        {
            var p = portal.GetComponent<Portal>();
            if(p.DestinationScene == sceneName)
            {
                return portal;
            }
        }
        return null;
    }

}
