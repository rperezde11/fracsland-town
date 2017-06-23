using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System;

public class SimpleTransparentShaderGUI : ShaderGUI
{
    private MaterialEditor m_MaterialEditor;

    MaterialProperty blendingSrc = null;
    MaterialProperty blendingDst = null;
    MaterialProperty alphaToMask = null;

    MaterialProperty albedoTex   = null;
    MaterialProperty cutoff      = null;

    enum RenderMode
    {
        ALPHA_TEST,
        ALPHA_BLEND
    }

    RenderMode mode;
    RenderQueue queue;
    bool alphaTestSmooth;
    bool manualRenderQueue;
    bool zwrite;

    int renderQueueVal;

    void FindProperties(MaterialProperty[] props)
    {
        blendingSrc = FindProperty("_BlendingSrc", props);
        blendingDst = FindProperty("_BlendingDst", props);
        alphaToMask = FindProperty("_AlphaToMaskOn", props);
        
        albedoTex = FindProperty("_MainTex", props);
        cutoff    = FindProperty("_Cutoff", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_MaterialEditor = materialEditor;

        FindProperties(properties);

        Material targetMat = materialEditor.target as Material;

        queue = (RenderQueue)Enum.Parse(typeof(RenderQueue), targetMat.GetTag("Queue", true));
        mode  = targetMat.IsKeywordEnabled("ALPHA_BLEND") ? RenderMode.ALPHA_BLEND : RenderMode.ALPHA_TEST;

        zwrite = (targetMat.GetInt("_ZWrite") == 1 ? true : false);

        EditorGUI.BeginChangeCheck();

        mode = (RenderMode)EditorGUILayout.EnumPopup("Render Mode", mode, new GUILayoutOption[]{});

        targetMat.DisableKeyword("ALPHA_TEST_HARD");
        targetMat.DisableKeyword("ALPHA_TEST_SMOOTH");
        targetMat.DisableKeyword("ALPHA_BLEND");

        if (mode == RenderMode.ALPHA_TEST)
        {
            if (!manualRenderQueue)
            {
                renderQueueVal = (int)RenderQueue.AlphaTest;
            }

            alphaTestSmooth = EditorGUILayout.Toggle("Smoother On", alphaTestSmooth, new GUILayoutOption[]{});

            if (alphaTestSmooth)
            {
                targetMat.SetInt("_AlphaToMaskOn", 1);
                targetMat.EnableKeyword("ALPHA_TEST_SMOOTH");
            }
            else
            {
                targetMat.SetInt("_AlphaToMaskOn", 0);
                targetMat.EnableKeyword("ALPHA_TEST_HARD");
            }

            targetMat.SetInt("_ZWrite", 1);
            targetMat.SetInt("_BlendingSrc", (int)BlendMode.One);
            targetMat.SetInt("_BlendingDst", (int)BlendMode.Zero);
        }

        if (mode == RenderMode.ALPHA_BLEND)
        {
            if (!manualRenderQueue)
            {
                renderQueueVal = (int)RenderQueue.Transparent;
            }

            zwrite = EditorGUILayout.Toggle("ZWrite", zwrite, new GUILayoutOption[]{});

            targetMat.SetInt("_AlphaToMaskOn", 0);
            targetMat.SetInt("_ZWrite", zwrite ? 1 : 0);

            targetMat.SetInt("_BlendingSrc", (int)BlendMode.SrcAlpha);
            targetMat.SetInt("_BlendingDst", (int)BlendMode.OneMinusSrcAlpha);

            targetMat.EnableKeyword("ALPHA_BLEND");
        }

        manualRenderQueue = EditorGUILayout.Toggle("Manual Render Queue", manualRenderQueue, new GUILayoutOption[]{});

        if (manualRenderQueue)
        {
            renderQueueVal = EditorGUILayout.IntSlider("Render Queue Value", renderQueueVal, 0, 5000, new GUILayoutOption[]{});
        }

        targetMat.renderQueue = renderQueueVal;

        VSpace(3);
        m_MaterialEditor.ShaderProperty(albedoTex, new GUIContent(albedoTex.displayName));
        m_MaterialEditor.ShaderProperty(cutoff, new GUIContent(cutoff.displayName));
        VSpace(2);

        EditorGUI.EndChangeCheck();
    }

    void VSpace(int factor)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(factor * 5);
        GUILayout.EndVertical();
    }

}