using UnityEngine;
using UnityEditor;
using System;

public class ReflectiveShaderGUI : ShaderGUI
{
    enum WaterOptions
    {
        Simple,
        Complex
    }


    private MaterialEditor m_MaterialEditor;
    
    MaterialProperty albedoTex              = null;
    MaterialProperty albedoColor            = null;
    MaterialProperty bumpTex                = null;
    MaterialProperty metallic               = null;
    MaterialProperty smoothness             = null;
    MaterialProperty extraTex               = null;
    MaterialProperty optimizeReflection     = null;
    MaterialProperty reflectionType         = null;

    private static class Styles
    {
        public static readonly string[] waterNames = Enum.GetNames(typeof(WaterOptions));
    }

    void FindProperties(MaterialProperty[] props)
    {
        albedoTex            = FindProperty("_MainTex", props); 
        albedoColor          = FindProperty("_Color", props); 
        bumpTex              = FindProperty("_BumpTex", props); 
        metallic             = FindProperty("_Metallic", props); 
        smoothness           = FindProperty("_Smoothness", props); 
        extraTex             = FindProperty("_ExtraTex", props); 
        optimizeReflection   = FindProperty("_Optimize", props); 
        reflectionType       = FindProperty("_Reflection", props); 
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_MaterialEditor = materialEditor;

        FindProperties(properties);

        Material targetMat = materialEditor.target as Material;

        EditorGUI.BeginChangeCheck();

        m_MaterialEditor.ShaderProperty(albedoTex, new GUIContent(albedoTex.displayName));

        VSpace(1);

        m_MaterialEditor.ShaderProperty(albedoColor, new GUIContent(albedoColor.displayName));

        VSpace(3);

        m_MaterialEditor.ShaderProperty(bumpTex, new GUIContent(bumpTex.displayName));
        SetKeyword(targetMat, "_BumpTex", "_BUMPMAP");

        VSpace(3);

        m_MaterialEditor.ShaderProperty(extraTex, new GUIContent(extraTex.displayName));
        SetKeyword(targetMat, "_ExtraTex", "EXTRA_MAP");

        if (!targetMat.IsKeywordEnabled("EXTRA_MAP"))
        {
            m_MaterialEditor.ShaderProperty(metallic, new GUIContent(metallic.displayName));
            m_MaterialEditor.ShaderProperty(smoothness, new GUIContent(smoothness.displayName));
        }

        VSpace(3);

        m_MaterialEditor.ShaderProperty(optimizeReflection, new GUIContent(optimizeReflection.displayName));
        m_MaterialEditor.ShaderProperty(reflectionType, new GUIContent(reflectionType.displayName));

        VSpace(2);

        if (EditorGUI.EndChangeCheck())
        {
            
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


    void VSpace(int factor)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(factor * 5);
        GUILayout.EndVertical();
    }

}