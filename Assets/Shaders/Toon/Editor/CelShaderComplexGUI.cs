using UnityEngine;
using UnityEditor;
using System;

public class CelShaderComplexGUI : ShaderGUI
{
    enum ToonOptions
    {
        None,
        Flat,
        Stepped,
        Ramp
    }

    enum ParallaxOptions
    {
        None,
        Bare,
        Steep,
        Occlussion
    }

    enum ContourOptions
    {
        None,
        Outline,
        Silhouette
    }

    enum SilhouetteOptions
    {
        None,
        Rim,
        Hard_Sil,
        Soft_Sil,
        Stepped
    }

    private MaterialEditor m_MaterialEditor;

    MaterialProperty albedoTex          = null;
    MaterialProperty bumpTex            = null;
    MaterialProperty parallaxType       = null;
    MaterialProperty parallaxTex        = null;
    MaterialProperty rampTex            = null;
    MaterialProperty albedoTint         = null;
    MaterialProperty specularity        = null;
    MaterialProperty smoothness         = null;
    MaterialProperty toonType           = null;
    MaterialProperty contourType        = null;
    MaterialProperty outlineColor       = null;
    MaterialProperty outlineWidth       = null;
    MaterialProperty silhouetteType     = null;
    MaterialProperty rimPower           = null;
    MaterialProperty rimSoftness        = null;
    MaterialProperty extraTex           = null;
    MaterialProperty emission           = null;
    MaterialProperty flickering         = null;
    MaterialProperty emissionColor      = null;
    MaterialProperty specularMap        = null;

    private static class Styles
    {
        public static readonly string[] contourNames = Enum.GetNames(typeof(ContourOptions));
        public static readonly string[] silhouetteNames = Enum.GetNames(typeof(SilhouetteOptions));
        public static readonly string[] toonNames = Enum.GetNames(typeof(ToonOptions));
    }

    void FindProperties(MaterialProperty[] props)
    {
        albedoTex = FindProperty("_MainTex", props);
        albedoTint = FindProperty("_TintColor", props);
        bumpTex = FindProperty("_BumpTex", props);
        parallaxType = FindProperty("_ParallaxType", props);
        parallaxTex = FindProperty("_ParallaxTex", props);
        rampTex = FindProperty("_RampTex", props);
        smoothness = FindProperty("_Smoothness", props);
        specularity = FindProperty("_Specularity", props);
        toonType = FindProperty("_ToonType", props);
        contourType = FindProperty("_ContourType", props);
        outlineWidth = FindProperty("_OutlineWidth", props);
        outlineColor = FindProperty("_OutlineColor", props);
        silhouetteType = FindProperty("_RimType", props);
        rimPower = FindProperty("_RimPower", props);
        rimSoftness = FindProperty("_RimSoftness", props);
        extraTex = FindProperty("_ExtraTex", props);
        emission = FindProperty("_Emission", props);
        flickering = FindProperty("_FlickerEmission", props);
        emissionColor = FindProperty("_EmissionColor", props);
        specularMap = FindProperty("_SpecularMap", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_MaterialEditor = materialEditor;

        FindProperties(properties);

        Material targetMat = materialEditor.target as Material;

        EditorGUI.BeginChangeCheck();

        m_MaterialEditor.ShaderProperty(toonType, new GUIContent(toonType.displayName));

        VSpace(3);

        m_MaterialEditor.ShaderProperty(albedoTex, new GUIContent(albedoTex.displayName));
        m_MaterialEditor.ShaderProperty(albedoTint, new GUIContent(albedoTint.displayName));

        VSpace(2);

        m_MaterialEditor.ShaderProperty(bumpTex, new GUIContent(bumpTex.displayName));

        if (targetMat.GetTexture("_BumpTex"))
        {
            targetMat.EnableKeyword("_BUMPMAP");
        }
        else
        {
            targetMat.DisableKeyword("_BUMPMAP");
        }

        m_MaterialEditor.ShaderProperty(parallaxType, new GUIContent(parallaxType.displayName));

        if (parallaxType.floatValue != 0)
        {
            m_MaterialEditor.ShaderProperty(parallaxTex, new GUIContent(parallaxTex.displayName));

            if (targetMat.GetTexture("_ParallaxTex"))
            {
                targetMat.EnableKeyword("_PARALLAX_MAP");
            }
            else
            {
                targetMat.DisableKeyword("_PARALLAX_MAP");
            }
        }
        else
        {
            targetMat.DisableKeyword("_PARALLAX_MAP");
        }

        if (toonType.floatValue != 3)  // if not Ramp toon shading
        {
            m_MaterialEditor.ShaderProperty(specularity, new GUIContent(specularity.displayName));
            VSpace(2);
            m_MaterialEditor.ShaderProperty(smoothness, new GUIContent(smoothness.displayName));
        }
        else
        {
            VSpace(2);
            m_MaterialEditor.ShaderProperty(rampTex, new GUIContent(rampTex.displayName));
            VSpace(2);
        }

        m_MaterialEditor.ShaderProperty(contourType, new GUIContent(contourType.displayName));

        if (contourType.floatValue != 0)
        {
            if (contourType.floatValue == 1)
            {
                VSpace(2);
                m_MaterialEditor.ShaderProperty(outlineWidth, new GUIContent(outlineWidth.displayName));
                m_MaterialEditor.ShaderProperty(outlineColor, new GUIContent(outlineColor.displayName));
                VSpace(2);
            }

            if (contourType.floatValue == 2)
            {
                VSpace(2);
                m_MaterialEditor.ShaderProperty(silhouetteType, new GUIContent(silhouetteType.displayName));
                m_MaterialEditor.ShaderProperty(rimPower, new GUIContent(rimPower.displayName));

                if (silhouetteType.floatValue == 4)
                {
                    m_MaterialEditor.ShaderProperty(rimSoftness, new GUIContent(rimSoftness.displayName));
                }

                VSpace(2);
            }

        }

        VSpace(3);

        m_MaterialEditor.ShaderProperty(extraTex, new GUIContent(extraTex.displayName));
        m_MaterialEditor.ShaderProperty(emission, new GUIContent(emission.displayName));

        if(emission.floatValue == 1) 
        {
            VSpace(1);
            m_MaterialEditor.ShaderProperty(emissionColor, new GUIContent(emissionColor.displayName));
            m_MaterialEditor.ShaderProperty(flickering, new GUIContent(flickering.displayName));
            VSpace(1);
        }

        VSpace(1);
        m_MaterialEditor.ShaderProperty(specularMap, new GUIContent(specularMap.displayName));

        if (EditorGUI.EndChangeCheck())
        {
            SetupMaterialWithToonShader(targetMat, (ToonOptions)toonType.floatValue);
            SetupMaterialWithParallax(targetMat, (ParallaxOptions)parallaxType.floatValue);
            SetupMaterialWithSilhouette(targetMat, (SilhouetteOptions)silhouetteType.floatValue);
            SetupMaterialWithContour(targetMat, (ContourOptions)contourType.floatValue);
        }
    }


    void SetupMaterialWithToonShader(Material mat, ToonOptions opt)
    {
        switch (opt)
        {
            case ToonOptions.None:
                mat.EnableKeyword("_TOONTYPE_NONE");
                mat.DisableKeyword("_TOONTYPE_FLAT");
                mat.DisableKeyword("_TOONTYPE_STEPPED");
                mat.DisableKeyword("_TOONTYPE_RAMP");
                break;
            case ToonOptions.Flat:
                mat.DisableKeyword("_TOONTYPE_NONE");
                mat.EnableKeyword("_TOONTYPE_FLAT");
                mat.DisableKeyword("_TOONTYPE_STEPPED");
                mat.DisableKeyword("_TOONTYPE_RAMP");
                break;
            case ToonOptions.Stepped:
                mat.DisableKeyword("_TOONTYPE_NONE");
                mat.DisableKeyword("_TOONTYPE_FLAT");
                mat.EnableKeyword("_TOONTYPE_STEPPED");
                mat.DisableKeyword("_TOONTYPE_RAMP");
                break;
            case ToonOptions.Ramp:
                mat.DisableKeyword("_TOONTYPE_NONE");
                mat.DisableKeyword("_TOONTYPE_FLAT");
                mat.DisableKeyword("_TOONTYPE_STEPPED");
                mat.EnableKeyword("_TOONTYPE_RAMP");
                break;
        }

    }

    void SetupMaterialWithParallax(Material mat, ParallaxOptions opt)
    {
        switch (opt)
        {
            case ParallaxOptions.None:
                mat.EnableKeyword("_PARALLAXTYPE_NONE");
                mat.DisableKeyword("_PARALLAXTYPE_BARE");
                mat.DisableKeyword("_PARALLAXTYPE_STEEP");
                mat.DisableKeyword("_PARALLAXTYPE_OCCLUSSION");
                break;
            case ParallaxOptions.Bare:
                mat.DisableKeyword("_PARALLAXTYPE_NONE");
                mat.EnableKeyword("_PARALLAXTYPE_BARE");
                mat.DisableKeyword("_PARALLAXTYPE_STEEP");
                mat.DisableKeyword("_PARALLAXTYPE_OCCLUSSION");
                break;
            case ParallaxOptions.Steep:
                mat.DisableKeyword("_PARALLAXTYPE_NONE");
                mat.DisableKeyword("_PARALLAXTYPE_BARE");
                mat.EnableKeyword("_PARALLAXTYPE_STEEP");
                mat.DisableKeyword("_PARALLAXTYPE_OCCLUSSION");
                break;
            case ParallaxOptions.Occlussion:
                mat.DisableKeyword("_PARALLAXTYPE_NONE");
                mat.DisableKeyword("_PARALLAXTYPE_BARE");
                mat.DisableKeyword("_PARALLAXTYPE_STEEP");
                mat.EnableKeyword("_PARALLAXTYPE_OCCLUSSION");
                break;
        }

    }

    void SetupMaterialWithContour(Material mat, ContourOptions opt)
    {
        switch (opt)
        {
            case ContourOptions.None:
                mat.EnableKeyword("_CONTOURTYPE_NONE");
                mat.DisableKeyword("_CONTOURTYPE_OUTLINE");
                mat.DisableKeyword("_CONTOURTYPE_SILHOUETTE");
                break;
            case ContourOptions.Outline:
                mat.DisableKeyword("_CONTOURTYPE_NONE");
                mat.EnableKeyword("_CONTOURTYPE_OUTLINE");
                mat.DisableKeyword("_CONTOURTYPE_SILHOUETTE");

                break;
            case ContourOptions.Silhouette:
                mat.DisableKeyword("_CONTOURTYPE_NONE");
                mat.DisableKeyword("_CONTOURTYPE_OUTLINE");
                mat.EnableKeyword("_CONTOURTYPE_SILHOUETTE");
                break;
        }

        if (opt != ContourOptions.Silhouette)
        {
            SetupMaterialWithSilhouette(mat, SilhouetteOptions.None);
        }
    }

    void SetupMaterialWithSilhouette(Material mat, SilhouetteOptions opt)
    {
        mat.DisableKeyword("_RIMTYPE_NONE");
        mat.DisableKeyword("_RIMTYPE_RIM");
        mat.DisableKeyword("_RIMTYPE_HARD_SILHOUETTE");
        mat.DisableKeyword("_RIMTYPE_SOFT_SILHOUETTE");
        mat.DisableKeyword("_RIMTYPE_STEPPED");

        switch (opt)
        {
            case SilhouetteOptions.None:
                mat.EnableKeyword("_RIMTYPE_NONE");
                break;
            case SilhouetteOptions.Rim:
                mat.EnableKeyword("_RIMTYPE_RIM");
                break;
            case SilhouetteOptions.Hard_Sil:
                mat.EnableKeyword("_RIMTYPE_HARD_SILHOUETTE");
                break;
            case SilhouetteOptions.Soft_Sil:
                mat.EnableKeyword("_RIMTYPE_SOFT_SILHOUETTE");
                break;
            case SilhouetteOptions.Stepped:
                mat.EnableKeyword("_RIMTYPE_STEPPED");
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