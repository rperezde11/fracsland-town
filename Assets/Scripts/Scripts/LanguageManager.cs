using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

    public static LanguageManager instance;

    private int _numTexts;
    private int _actualLang;
    public int ActualLang { get { return _actualLang; } set { _actualLang = value; } }
    int numTexts, actualLang;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        _actualLang = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (_numTexts == 0)
        {
            TextFactory.Load();
            LoadTexts();
        }
    }

    void OnLevelWasLoaded(int level)
    {
        _numTexts = 0;
        TextFactory.UnregisterTexts();
    }

    void LoadTexts()
    {
        try
        {
            GameObject.Find("ChangeLang").GetComponent<Dropdown>().value = actualLang;
            GameObject[] texts = GameObject.FindGameObjectsWithTag("UIMultiLangText");
            if (numTexts == texts.Length) return;
            _numTexts = texts.Length;
            foreach (GameObject text in texts)
            {
                Text t = text.GetComponent<Text>();
                if (t != null)
                {
                    string txt = TextFactory.GetText(t.text);
                    if (txt != "")
                    {
                        t.text = TextFactory.GetText(t.text, t);
                    }
                }
            }
        }
        catch (System.Exception ex) { }
    }
}
