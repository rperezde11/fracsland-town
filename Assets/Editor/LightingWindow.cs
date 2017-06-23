using System;
using UnityEditor;
using UnityEngine;


public class LightingWindow : EditorWindow {

    private const float MARGIN_WIDTH = 10;
    private const float MARGIN_HEIGHT = 2;

    [SerializeField]
    private LightSettings lightSettings;

    private static EditorWindow windowShaders;
    private Vector2 scrollvalue;

    private static class Styles
    {
        public static GUILayoutOption[] colorOptions = new GUILayoutOption[] {
            GUILayout.MinWidth(10),
            GUILayout.MaxWidth(50)
        };

        public static GUILayoutOption[] sliderOptions = new GUILayoutOption[] {
            GUILayout.MinWidth(100),
            GUILayout.MaxWidth(200)
        };
    }

    [MenuItem("Window/Lighting Values")]
    public static void Init()
    {
        windowShaders = EditorWindow.GetWindow<LightingWindow>("Lighting Values") as EditorWindow;
        windowShaders.Show();
    }

    void OnGUI()
    {
        if (lightSettings == null)
        {
            lightSettings = new LightSettings();
        }

        if (windowShaders == null)
        {
            windowShaders = EditorWindow.GetWindow<LightingWindow>("Lighting Values") as EditorWindow;
        }

        Rect total = new Rect(MARGIN_WIDTH, MARGIN_HEIGHT, windowShaders.position.width - 2 * MARGIN_WIDTH, windowShaders.position.height);
        Rect s1 = new Rect(MARGIN_WIDTH, MARGIN_HEIGHT, 400, 250);

        scrollvalue = GUI.BeginScrollView(total, scrollvalue, s1, false, false);

        GUILayout.BeginArea(s1);

        VSpace(2);

        GUILayout.Label("Directional lights", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Intensity");
        lightSettings.directionalIntensity = EditorGUILayout.Slider(lightSettings.directionalIntensity, LightSettings.MIN_DIRECTIONAL_INTENSITY, LightSettings.MAX_DIRECTIONAL_INTENSITY, Styles.sliderOptions);
        GUILayout.EndHorizontal();

        VSpace(2);

        GUILayout.Label("Spot lights", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Intensity");
        lightSettings.spotIntensity  = EditorGUILayout.Slider(lightSettings.spotIntensity, LightSettings.MIN_SPOT_INTENSITY, LightSettings.MAX_SPOT_INTENSITY, Styles.sliderOptions);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Angle");
        lightSettings.spotAngle = EditorGUILayout.Slider(lightSettings.spotAngle, LightSettings.MIN_SPOT_ANGLE, LightSettings.MAX_SPOT_ANGLE, Styles.sliderOptions);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Color");
        lightSettings.spotColor = EditorGUILayout.ColorField(lightSettings.spotColor, Styles.colorOptions);
        GUILayout.EndHorizontal();

        VSpace(2);

        GUILayout.Label("Point lights", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Intensity");
        lightSettings.pointIntensity = EditorGUILayout.Slider(lightSettings.pointIntensity, LightSettings.MIN_POINT_INTENSITY, LightSettings.MAX_POINT_INTENSITY, Styles.sliderOptions);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Range");
        lightSettings.pointRange = EditorGUILayout.Slider(lightSettings.pointRange, LightSettings.MIN_POINT_RANGE, LightSettings.MAX_POINT_RANGE, Styles.sliderOptions);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Color");
        lightSettings.pointColor = EditorGUILayout.ColorField(lightSettings.pointColor, Styles.colorOptions);
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        GUI.EndScrollView();

        lightSettings.Apply();
    }

    void VSpace(int factor)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(factor * 5);
        GUILayout.EndVertical();
    }
}

[Serializable]
public class LightSettings
{

    public const float MIN_DIRECTIONAL_INTENSITY  = 0;
    public const float MAX_DIRECTIONAL_INTENSITY  = 8;
    public const float MIN_SPOT_INTENSITY         = 0;
    public const float MAX_SPOT_INTENSITY         = 8;
    public const float MIN_POINT_INTENSITY        = 0;
    public const float MAX_POINT_INTENSITY        = 8;

    public const float MIN_SPOT_ANGLE  = 0;
    public const float MAX_SPOT_ANGLE  = 180;
    public const float MIN_POINT_RANGE = 0;
    public const float MAX_POINT_RANGE = 20;


    public float directionalIntensity;
    public float pointIntensity;
    public float pointRange;
    public float spotIntensity;
    public float spotAngle;

    public Color pointColor;
    public Color spotColor;

    public LightSettings()
    {
        directionalIntensity = 0.5f;
        pointIntensity       = 1.0f;
        pointRange           = 6.0f;
        spotIntensity        = 1.0f;
        spotAngle            = 30.0f;

        pointColor = Color.white;
        spotColor  = Color.white;
    }

    public void Apply()
    {
        Light[] lights = GameObject.FindObjectsOfType<Light>();

        for (int i = 0; i < lights.Length; i++)
        {
            // If the light has a name that start with "no-"
            // light is not affected.

            if (lights[i].name.Substring(0, 3) == "no-")
                continue;

            switch (lights[i].type)
            {
                case LightType.Directional:
                    lights[i].intensity = directionalIntensity;
                    break;

                case LightType.Spot:
                    lights[i].intensity = spotIntensity;
                    lights[i].spotAngle = spotAngle;
                    lights[i].color     = spotColor;
                    break;
                
                case LightType.Point:
                    lights[i].intensity = pointIntensity;
                    lights[i].range     = pointRange;
                    lights[i].color     = pointColor;
                    break;
            }

        }
    }

}