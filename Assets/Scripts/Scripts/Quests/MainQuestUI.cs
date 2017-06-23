using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Used to show how much engine pieces has our player
/// </summary>
public class MainQuestUI : MonoBehaviour {

    public GameObject GameEnginePieceUI;
    private List<EnginePieceUI> _engineComponents;
    int counter = 0;

    void Awake()
    {
        _engineComponents = new List<EnginePieceUI>();
    }

    public void ShowEnginePieces(int numPieces)
    {
        for(int i = 0; i < numPieces; i++)
        {
            GameObject ui = Instantiate(GameEnginePieceUI);
            ui.transform.SetParent(transform);
            _engineComponents.Add(ui.GetComponent<EnginePieceUI>());
        }
    }

    public void AddAddedPiece()
    {
        _engineComponents[counter].SetAlreadyHasColor();
        counter++;
    }

}
