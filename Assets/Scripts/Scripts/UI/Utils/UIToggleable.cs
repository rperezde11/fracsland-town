using UnityEngine;

/// <summary>
/// This class is used for simple items of our in order to hide and show them correctly
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class UIToggleable : MonoBehaviour
{
    public bool isVisible { get { return _isVisible; } }
    private bool _isVisible = false;

    public void hide()
    {
        CanvasRenderer cr = GetComponent<CanvasRenderer>();
        cr.SetAlpha(0f);
    }

    public void show()
    {
        CanvasRenderer cr = GetComponent<CanvasRenderer>();
        cr.SetAlpha(1f);
    }
}

