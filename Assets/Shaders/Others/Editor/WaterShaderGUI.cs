using UnityEngine;
using UnityEditor;
using System;

public class WaterShaderGUI : ShaderGUI
{
    enum WaterOptions
    {
        Simple,
        Complex
    }


    private MaterialEditor m_MaterialEditor;
    
    MaterialProperty waterType = null;
    MaterialProperty albedoColor = null;
    MaterialProperty toonify = null;
    MaterialProperty bumpTex = null;
    MaterialProperty fresnelAirWaterTex = null;
    MaterialProperty waveIntensity = null;
    MaterialProperty waveScale = null;
    MaterialProperty depthFog = null;
    MaterialProperty waterFogIntensity = null;
    MaterialProperty speed = null;
    MaterialProperty tideHeight = null;
    

    private static class Styles
    {
        public static readonly string[] waterNames = Enum.GetNames(typeof(WaterOptions));
    }

    void FindProperties(MaterialProperty[] props)
    {
        waterType           = FindProperty("Water", props); 
        albedoColor         = FindProperty("_Color", props); 
        toonify             = FindProperty("_Toonify", props); 
        bumpTex             = FindProperty("_BumpTex", props); 
        fresnelAirWaterTex  = FindProperty("_FresnelAirWaterTex", props); 
        waveIntensity       = FindProperty("_WaveIntensity", props); 
        waveScale           = FindProperty("_WaveScale", props); 
        depthFog            = FindProperty("_DepthFog", props); 
        waterFogIntensity   = FindProperty("_WaterFogIntensity", props); 
        speed               = FindProperty("_Speed", props); 
        tideHeight          = FindProperty("_TideHeight", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_MaterialEditor = materialEditor;

        FindProperties(properties);

        Material targetMat = materialEditor.target as Material;

        EditorGUI.BeginChangeCheck();

        m_MaterialEditor.ShaderProperty(waterType, new GUIContent(waterType.displayName));

        VSpace(3);

        m_MaterialEditor.ShaderProperty(albedoColor, new GUIContent(albedoColor.displayName));

        if (waterType.floatValue == 1) // if water is complex
        {
            m_MaterialEditor.ShaderProperty(toonify, new GUIContent(toonify.displayName));
        }

        VSpace(2);

        m_MaterialEditor.ShaderProperty(bumpTex, new GUIContent(bumpTex.displayName));
        SetKeyword(targetMat, "_BumpTex", "_BUMPMAP");

        if (waterType.floatValue == 1)  // if water is complex
        {
            m_MaterialEditor.ShaderProperty(fresnelAirWaterTex, new GUIContent(fresnelAirWaterTex.displayName));
        }

        m_MaterialEditor.ShaderProperty(waveIntensity, new GUIContent(waveIntensity.displayName));
        m_MaterialEditor.ShaderProperty(waveScale, new GUIContent(waveScale.displayName));

        m_MaterialEditor.ShaderProperty(depthFog, new GUIContent(depthFog.displayName));

        if (depthFog.floatValue == 1)
        {
            m_MaterialEditor.ShaderProperty(waterFogIntensity, new GUIContent(waterFogIntensity.displayName));
        }

        m_MaterialEditor.ShaderProperty(speed, new GUIContent(speed.displayName));
        m_MaterialEditor.ShaderProperty(tideHeight, new GUIContent(tideHeight.displayName));

        VSpace(2);

        if (EditorGUI.EndChangeCheck())
        {
            SetupMaterialWithWaterShader(targetMat, (WaterOptions)waterType.floatValue);
        }
    }


    void SetKeyword(Material mat, string propertyName, string keyword)
    {
        if (mat.GetTexture(propertyName))
        {
            mat.EnableKeyword(keyword);
        }
        else
        {
            mat.DisableKeyword(keyword);
        }
}

    void SetupMaterialWithWaterShader(Material mat, WaterOptions opt)
    {
        switch (opt)
        {
            case WaterOptions.Simple:
                mat.EnableKeyword("WATER_SIMPLE");
                mat.DisableKeyword("WATER_COMPLEX");
                break;
            case WaterOptions.Complex:
                mat.DisableKeyword("WATER_SIMPLE");
                mat.EnableKeyword("WATER_COMPLEX");
                break;
        }
    }

    void VSpace(int factor)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(factor * 5);
        GUILayout.EndVertical();
    }

}