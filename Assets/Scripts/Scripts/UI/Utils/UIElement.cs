using UnityEngine;

/// <summary>
/// A very usefull class used to show or hide UI elements without
/// deactivating them.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIElement : MonoBehaviour
{
    public bool isVisible { get { return _isVisible; } }
    private bool _isVisible = false;

    public void hide()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.alpha = 0f;
        _isVisible = false;
    }

    public void show()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.interactable = true;
        cg.blocksRaycasts = true;
        cg.alpha = 1f;
        _isVisible = true;
    }

    public void toggle()
    {
        if (_isVisible)
        {
            hide();
        }
        else
        {
            show();
        }
    }
}

