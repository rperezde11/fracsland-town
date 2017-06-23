using UnityEngine;
using UnityEngine.UI;

public class FadeUIElementOnStart : MonoBehaviour
{

    private CanvasRenderer _canvas;

    void Start()
    {
        _canvas = GetComponent<CanvasRenderer>();

        if (_canvas == null)
            throw new System.Exception("GameObject must have a Canvas Renderer component attached!");
    }
   
    void Update()
    {
        _canvas.SetAlpha(_canvas.GetAlpha() - 0.25f * Time.deltaTime);

        if (_canvas.GetAlpha() <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
