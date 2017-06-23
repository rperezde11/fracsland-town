using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

/// <summary>
/// Used for multiplanguaje, finally not used because fracsland 
/// will be an english game.
/// </summary>
public static class TextFactory
{
    public const string STRINGS_FILE_PATH = "/Langs/strings.json";

    private static Dictionary<String, Dictionary<String, String>> texts;
    private static Dictionary<String, String> actualLangTexts;
    private static Dictionary<String, Text> registeredTexts;
    private static List<string> avaliableLanguages;

    public static void Load()
    {
        TextAsset json = (TextAsset)Resources.Load("Langs/strings");
        texts = new Dictionary<string, Dictionary<string, string>>();
        registeredTexts = new Dictionary<string, Text>();
        avaliableLanguages = new List<string>();

        var jsonParsed = JSON.Parse(json.text);
        var langs = jsonParsed["languages"];
        int numLanguages = langs.Count;
        
        for(int i = 0; i<numLanguages; i++)
        {
            var txtOfLang = jsonParsed[langs[i]];
            avaliableLanguages.Add(langs[i]);

            actualLangTexts = new Dictionary<string, string>();

            for(int j = 0; j<txtOfLang.Count; j++)
            {
                actualLangTexts.Add(txtOfLang[j]["key"], txtOfLang[j]["value"]);
            }
            texts.Add(langs[i], actualLangTexts);
        }

        actualLangTexts = texts["en"];
    }

    public static string GetText(string key, Text text=null)
    {
        string txt;
        try
        {
            txt = actualLangTexts[key];
        }
        catch(Exception ex)
        {
            txt = "";
        }
        if(text != null)
        {
            registeredTexts.Add(key, text);
        }
        return txt;
    }

    public static void ChangeLanguage(int language)
    {
        actualLangTexts = texts[avaliableLanguages[language]];
        foreach (KeyValuePair<string, Text > text in registeredTexts)
        {
			if (actualLangTexts.ContainsKey (text.Key))
				text.Value.text = actualLangTexts [text.Key];
			else
				Debug.Log ("TextFactory: " + text.Value.text + " Not found");
			
        }
    }

    public static void UnregisterTexts()
    {
        registeredTexts.Clear();
    }
}

