using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An engine piece showed in the UI
/// </summary>
public class EnginePieceUI : MonoBehaviour {

    private Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetAlreadyHasColor()
    {
        _image.color = Color.green;
    }

    public void SetNotHasColor()
    {
        _image.color = Color.red;
    }
}
