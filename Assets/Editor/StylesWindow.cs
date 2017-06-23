using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

// The names of the shader types must be 
// UPPERCASE spaced by _ to match the name 
// of the JSON on Resources/JSON/name*
[Serializable]
public enum STYLES_TYPES
{
    STANDARD,
    TOON
};

[Serializable]
public enum TEXTURE_STYLES
{
    BASIC,
    DETAIL
};

public class EditorGUIStyles
{
    public static GUILayoutOption[] emptyOpts = new GUILayoutOption[] { };
    public static GUILayoutOption [] sliderOpts  = new GUILayoutOption[] { GUILayout.MinWidth(150), GUILayout.MaxWidth(200),  GUILayout.ExpandWidth(false) };
    public static GUILayoutOption [] floatOpts   = new GUILayoutOption[] { GUILayout.MinWidth(25), GUILayout.MaxWidth(50),  GUILayout.ExpandWidth(false) };
    public static GUILayoutOption [] colorOpts   = new GUILayoutOption[] { GUILayout.MinWidth(25), GUILayout.MaxWidth(40),  GUILayout.ExpandWidth(false) };
    public static GUILayoutOption [] textureOpts = new GUILayoutOption[] { GUILayout.MinWidth(40), GUILayout.MaxWidth(60),  GUILayout.ExpandWidth(false) };
}


public class StylesWindow : EditorWindow 
{
    private const float MARGIN_WIDTH = 10;
    private const float MARGIN_HEIGHT = 2;

    [SerializeField] STYLES_TYPES currentStyle;
    [SerializeField] TEXTURE_STYLES currentTextureStyle;
    [SerializeField] int currentConfiguration;
    [SerializeField] int currentDivision;

    [SerializeField] MaterialProperty[] properties = {};
    [SerializeField] Material[] editedMaterials = {};


    private static EditorWindow windowShaders;
    private Vector2 scrollvalue;
    [SerializeField] private static StyleManager styleManager;

    [MenuItem("Window/Styles")]
    public static void Init()
    {
        EditorWindow windowShaders = EditorWindow.GetWindow<StylesWindow>("Styles") as EditorWindow;
        windowShaders.Show();
    }

    void OnGUI()
    {
        if (windowShaders == null)
        {
            windowShaders = EditorWindow.GetWindow<StylesWindow>("Styles") as EditorWindow;
        }

        if (styleManager == null)
        {
            styleManager = new StyleManager();
            styleManager.ChangeStyle(new Style(STYLES_TYPES.TOON));
        }

        Rect total = new Rect(0, 0, windowShaders.position.width, windowShaders.position.height);
        Rect title = new Rect(total.width*(7f/16f), 0, total.width*(2f/16f), total.height/4f);
        Rect container = new Rect(0, title.max.y, total.width, total.height - title.max.y);
        Rect c_left    = new Rect(container.width*(1f/16f), 0, container.width*(4f/16f), container.height);
        Rect c_center  = new Rect(c_left.max.x + container.width*(1f/16f), 0, container.width*(4f/16f), container.height);
        Rect c_right   = new Rect(c_center.max.x + container.width*(1f/16f), 0, container.width*(4f/16f), container.height);


        GUILayout.BeginArea(total); // BEGIN TOTAL



        GUILayout.BeginArea(title);
        DoTitle();
        GUILayout.EndArea();


        GUILayout.BeginArea(container);

        GUILayout.BeginArea(c_left);
        DoConfiguration();
        GUILayout.EndArea();

        GUILayout.BeginArea(c_center);
        DoTextures();
        GUILayout.EndArea();

        GUILayout.BeginArea(c_right);
        DoEditConfiguration();
        GUILayout.EndArea();

        GUILayout.EndArea();



        GUILayout.EndArea(); // END TOTAL
    }

    private void DoTitle()
    {
        EditorGUI.BeginChangeCheck();
        {
            GUILayout.Label("Style", EditorStyles.boldLabel);
            currentStyle = (STYLES_TYPES)EditorGUILayout.EnumPopup(currentStyle, EditorGUIStyles.emptyOpts);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Style newStyle = new Style(currentStyle, currentConfiguration, currentTextureStyle);
            styleManager.ChangeStyle(newStyle);
        }
    }

    private void DoConfiguration()
    {
        EditorGUI.BeginChangeCheck();
        {
            GUILayout.Label("Configuration", EditorStyles.boldLabel);
            currentConfiguration = EditorGUILayout.Popup(currentConfiguration, styleManager.GetCurrentStyle().GetInfo().GetConfigurations(), EditorGUIStyles.emptyOpts);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Style newStyle = new Style(currentStyle, currentConfiguration, currentTextureStyle);
            styleManager.ChangeStyle(newStyle);
        }
    }

    private void DoTextures()
    {
        EditorGUI.BeginChangeCheck();
        {
            GUILayout.Label("Textures", EditorStyles.boldLabel);
            currentTextureStyle = (TEXTURE_STYLES)EditorGUILayout.EnumPopup(currentTextureStyle, EditorGUIStyles.emptyOpts);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Style newStyle = new Style(currentStyle, currentConfiguration, currentTextureStyle);
            styleManager.ChangeStyle(newStyle);
        }
    }

    private void DoEditConfiguration()
    {
        StyleInfo info = styleManager.GetCurrentStyle().GetInfo();

        EditorGUI.BeginChangeCheck();
        {
            GUILayout.Label("Edit", EditorStyles.boldLabel);
            currentDivision = EditorGUILayout.Popup(currentDivision, info.GetDivisions(), EditorGUIStyles.emptyOpts);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Material matConfiguration = Resources.Load<Material>("Materials/" + currentStyle.ToString().ToLower() + "/" + info.GetDivision(currentDivision) + "/__conf_" + info.GetConfiguration(currentConfiguration));
            properties = MaterialEditor.GetMaterialProperties(new UnityEngine.Object[] { matConfiguration });

            editedMaterials = styleManager.GetMaterialsByDivision(currentDivision);
        }

        scrollvalue = EditorGUILayout.BeginScrollView(scrollvalue, EditorGUIStyles.emptyOpts);

        VSpace(2);

        foreach (MaterialProperty property in properties)
        {
            VSpace(1);

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginHorizontal();
                DoProperty(property);
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < editedMaterials.Length; i++)
                {
                    SetProperty(ref editedMaterials[i], property);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DoProperty(MaterialProperty property)
    {
        GUILayout.Label(property.displayName, EditorStyles.boldLabel);

        if (property.type == MaterialProperty.PropType.Range)
        {
            property.floatValue = EditorGUILayout.Slider(property.floatValue, property.rangeLimits.x, property.rangeLimits.y, EditorGUIStyles.sliderOpts);
        }
        
        if (property.type == MaterialProperty.PropType.Float)
        {
            property.floatValue = EditorGUILayout.FloatField(property.floatValue, EditorGUIStyles.floatOpts);
        }

        if (property.type == MaterialProperty.PropType.Color)
        {
            property.colorValue = EditorGUILayout.ColorField(property.colorValue, EditorGUIStyles.colorOpts);
        }

        if (property.type == MaterialProperty.PropType.Texture)
        {
            property.textureValue = (Texture2D)EditorGUILayout.ObjectField(property.textureValue, typeof(Texture2D), true, EditorGUIStyles.textureOpts);
        }
    }

    private void SetProperty(ref Material material, MaterialProperty property)
    {
        if (property.type == MaterialProperty.PropType.Range)
        {
            material.SetFloat(property.name, property.floatValue);
        }

        if (property.type == MaterialProperty.PropType.Float)
        {
            material.SetFloat(property.name, property.floatValue);
        }

        if (property.type == MaterialProperty.PropType.Color)
        {
            material.SetColor(property.name, property.colorValue);
        }

        if (property.type == MaterialProperty.PropType.Texture)
        {
            material.SetTexture(property.name, property.textureValue);
        }
    }

    void VSpace(int factor)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(factor * 5);
        GUILayout.EndVertical();
    }
}



[Serializable]
public class Style
{
    [SerializeField] private STYLES_TYPES style;
    [SerializeField] private TEXTURE_STYLES texStyle;
    [SerializeField] private string config;
    [SerializeField] private StyleInfo info;

    public Style(STYLES_TYPES style)
    {
        this.style = style;
        texStyle = TEXTURE_STYLES.BASIC;

        info = new StyleInfo(style);
        
        string [] lstConfigurations = info.GetConfigurations();

        if (lstConfigurations.Length == 0)
        {
            throw new Exception("No configurations found for style " + this.style.ToString() + ".");
        }

        config = lstConfigurations[0];
    }

    public Style(STYLES_TYPES style, TEXTURE_STYLES texStyle)
    {
        this.style = style;
        this.texStyle = texStyle;

        info = new StyleInfo(style);

        string[] lstConfigurations = info.GetConfigurations();

        if (lstConfigurations.Length == 0)
        {
            throw new Exception("No configurations found for style " + this.style.ToString() + ".");
        }

        config = lstConfigurations[0];
    }

    public Style(STYLES_TYPES style, int indexConfig)
    {
        this.style = style;
        this.texStyle = TEXTURE_STYLES.BASIC;

        info = new StyleInfo(style);

        string[] lstConfigurations = info.GetConfigurations();

        if (lstConfigurations.Length <= indexConfig)
        {
            throw new Exception("No configurations found for style " + this.style.ToString() + " with the index " + indexConfig + ".");
        }

        config = lstConfigurations[indexConfig];
    }

    public Style(STYLES_TYPES style, int indexConfig, TEXTURE_STYLES texStyle)
    {
        this.style = style;
        this.texStyle = texStyle;

        info = new StyleInfo(style);

        string[] lstConfigurations = info.GetConfigurations();

        if (lstConfigurations.Length <= indexConfig)
        {
            throw new Exception("No configurations found for style " + this.style.ToString() + " with the index " + indexConfig + ".");
        }

        config = lstConfigurations[indexConfig];
    }

    public STYLES_TYPES GetStyle()
    {
        return style;
    }

    public void SetTextureStyle(TEXTURE_STYLES texStyle)
    {
        this.texStyle = texStyle;
    }

    public TEXTURE_STYLES GetTextureStyle()
    {
        return texStyle;
    }

    public void SetConfiguration(int indexConfig)
    {
        int maxIndex = info.GetConfigurations().Length - 1;

        if (indexConfig > maxIndex)
        {
            throw new Exception("The index for this style configuration is incorrect. MAX is " + maxIndex + ".");
        }

        this.config = info.GetConfigurations()[indexConfig];
    }

    public string GetConfiguration()
    {
        return this.config;
    }

    public StyleInfo GetInfo()
    {
        return info;
    }
}

[Serializable]
public class StyleInfo {

    [SerializeField] private STYLES_TYPES style;
    [SerializeField] private string [] configurations;
    [SerializeField] private string [] divisions;

    public StyleInfo(STYLES_TYPES style)
    {
        this.style = style;

        FillConfigurationList();
    }

    public STYLES_TYPES GetStyle()
    {
        return style;
    }

    public string [] GetConfigurations()
    {
        return configurations;
    }

    public string GetConfiguration(int index)
    {
        string configuration = null;

        if (index < configurations.Length)
        {
            configuration = configurations[index];
        }

        return configuration;
    }

    public string [] GetDivisions()
    {
        return divisions;
    }

    public string GetDivision(int index)
    {
        string division = null;

        if (index < divisions.Length)
        {
            division = divisions[index];
        }

        return division;
    }

    private void FillConfigurationList()
    {
        bool gotConf = false;
        
        string pathStyleMaterials = "Assets/Resources/Materials/" + ParsePath(style.ToString()) + "/";

        List<string> lstConf = new List<string>();
        List<string> lstDiv = new List<string>();

        DirectoryInfo[] folderInfo = new DirectoryInfo(pathStyleMaterials).GetDirectories();

        for (int i = 0; i < folderInfo.Length; i++)
        {
            string [] pathSubfolders = folderInfo[i].ToString().Split('\\');
            lstDiv.Add(pathSubfolders[pathSubfolders.Length-1]);

            FileInfo [] filesInfo = new DirectoryInfo(folderInfo[i].ToString()).GetFiles("*", SearchOption.AllDirectories);

            if (gotConf) continue;

            for (int j = 0; j < filesInfo.Length; j++)
            {
                gotConf = true;

                GroupCollection capture = Regex.Match(filesInfo[j].ToString(), "__conf_(.*).mat$").Groups;

                if (capture.Count > 1)
                {
                    lstConf.Add(capture[1].ToString());
                }
            }

        }

        configurations = lstConf.ToArray();
        divisions = lstDiv.ToArray();
    }

    private string ParsePath(string str)
    {
        return str.ToLower();
    }
}

[Serializable]
class StyleManager 
{
    [SerializeField] private Style currentStyle;

    public StyleManager()
    {

    }

    public Style GetCurrentStyle()
    {
        return currentStyle;
    }

    public void ChangeStyle(Style newStyle)
    {
        currentStyle = newStyle;

        Dictionary<string, Texture> textures = GetDictTextures();
        Dictionary<string, Material> materials = GetDictMaterials(currentStyle, textures);

        List<GameObject> gameobjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());

        for (int i = 0; i < gameobjects.Count; i++)
        {
            Renderer renderer;

            if (gameobjects[i].activeInHierarchy)
            {
                renderer = gameobjects[i].GetComponent<Renderer>();

                if (!renderer) continue;

                List<Material> t_mats = new List<Material>();

                for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                {
                    if (!renderer.sharedMaterials[j]) continue;

                    Material t_mat;

                    materials.TryGetValue(renderer.sharedMaterials[j].name, out t_mat);

                    if (t_mat)
                    {
                        t_mats.Add(t_mat);
                    }
                }

                if (t_mats.Count > 0)
                {
                    renderer.sharedMaterials = t_mats.ToArray();
                }
            }
        }
    }

    private Dictionary<string, Texture> GetDictTextures()
    {
        string texturesPath = "Textures/";

        Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        Texture[] lstTextures = Resources.LoadAll<Texture>(texturesPath);

        foreach (Texture t in lstTextures)
        {
            textures.Add(t.name, t);
        }

        return textures;
    }

    private Dictionary<string, Material> GetDictMaterials(Style style, Dictionary<string, Texture> textures)
    {
        string stylePath;
        Material matConf;
        Dictionary<string, Material> materials;

        stylePath = "Materials/" + currentStyle.GetStyle().ToString().ToLower() + "/";
        materials = new Dictionary<string, Material>();

        foreach (string d in style.GetInfo().GetDivisions())
        {
            matConf = Resources.Load<Material>(stylePath + d + "/" + "__conf_" + style.GetConfiguration());

            foreach (Material m in Resources.LoadAll<Material>(stylePath + d + "/"))
            {
                if (Regex.IsMatch(m.name, "__conf_"))
                    continue;

                materials.Add(m.name, MergeMaterials(matConf, m, textures, style.GetTextureStyle()));
            }
        }

        return materials;
    }

    public Material MergeMaterials(Material matConf, Material mat, Dictionary<string, Texture> locTextures, TEXTURE_STYLES texStyle) 
    {
        Material copy;
        Texture texture;

        string [] names;

        copy = new Material(matConf);
        names = new string[4] { "main", "bump", "parallax", "extra" };

        foreach (string name in names)
        {
            string propertyName = "_" + name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower() + "Tex";

            if (copy.HasProperty(propertyName) && copy.GetTexture(propertyName))
            {
                string textureName = mat.name;

                if (name == "main")
                {
                    string t_textureName = textureName + "__" + texStyle.ToString().ToLower();
                    
                    if (!locTextures.TryGetValue(t_textureName, out texture))
                    {
                        t_textureName = textureName + "__detail";
                        locTextures.TryGetValue(t_textureName, out texture);
                    }
                }
                else 
                {
                    textureName += "__" + name;
                    locTextures.TryGetValue(textureName, out texture);
                }

                copy.SetTexture(propertyName, texture);
                copy.name = mat.name;
            }
        }

        return copy;
    }

    public Material[] GetMaterialsByDivision(int division)
    {
        List<Material> materials = new List<Material>();
        List<GameObject> gameobjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());

        string pathToMaterials = "Materials/" + currentStyle.GetStyle().ToString().ToLower() + "/" + currentStyle.GetInfo().GetDivision(division).ToLower() + "/";

        Material [] loadedMaterials = Resources.LoadAll<Material>(pathToMaterials);
        List<string> materialNames = new List<string>();

        foreach (Material m in loadedMaterials)
        {
            if (Regex.IsMatch(m.name, "__conf_")) continue;
            materialNames.Add(m.name);
        }

        foreach (GameObject gameobject in gameobjects)
        {
            Renderer renderer = gameobject.GetComponent<Renderer>();

            if (renderer == null) continue;

            foreach (Material m in renderer.sharedMaterials)
            {
                if (m != null && materialNames.Contains(m.name))
                {
                    materials.Add(m);
                }
            }

        }

        return materials.ToArray();
    }

}