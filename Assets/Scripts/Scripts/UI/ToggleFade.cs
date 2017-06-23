using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// An element which fades in showing some text and after fades out
/// (Used to show zone names or level up)
/// </summary>
public class ToggleFade : MonoBehaviour {

    public delegate void Method();
    public static ToggleFade instance;

    private float _changePhaseTime;
    private CanvasGroup _canvasGroup;
    private Method CompletionCallback;
    private Method ActualUpdate = null;
    private Text _text;

    float t;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            _text = transform.FindChild("Text").GetComponent<Text>();
        }


    }

	void Update () {
        if (ActualUpdate != null) ActualUpdate();
	}

    void AppearingUpdate()
    {
        t += Time.deltaTime;
        _canvasGroup.alpha = t / _changePhaseTime;

        if (t > _changePhaseTime)
        {
            ActualUpdate = ShowingUpdate;
            t = 0;
        }
    }

    void ShowingUpdate()
    {
        t += Time.deltaTime;

        if (t > _changePhaseTime)
        {
            ActualUpdate = DissappearingUpdate;
            t = 0;
        }
    }

    void DissappearingUpdate()
    {
        t += Time.deltaTime;
        _canvasGroup.alpha = (_changePhaseTime - t) / _changePhaseTime;

        if (t > _changePhaseTime)
        {
            ActualUpdate = AppearingUpdate;
            CompletedCycle();
            t = 0;
        }
    }

    public void FadeWithTextAndColor(string textToShow, Color color, float duration, Method callback=null)
    {
        t = 0;
        CompletionCallback = callback;
        _changePhaseTime = duration / 3;
        _text.color = color;
        _text.text = textToShow;
        ActualUpdate = AppearingUpdate;
    }

    public void CompletedCycle()
    {
        if (CompletionCallback != null) CompletionCallback();
        ActualUpdate = null;
    }

}
