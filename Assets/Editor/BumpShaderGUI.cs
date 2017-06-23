using System;
using UnityEngine;

namespace UnityEditor
{
    internal class BumpShaderGUI : ShaderGUI
    {

        private static class Styles
        {
            public static string emptyTooltip = "";
            public static string whiteSpaceString = " ";

            public static GUIContent albedoMapText = new GUIContent("Albedo", "Albedo (RGBA)");
            public static GUIContent bumpMapText = new GUIContent("Normal Map", "Normal Map");
            public static GUIContent albedoColorText = new GUIContent("Diffuse Color", "Diffuse");
            public static GUIContent smoothnessText = new GUIContent("Smoothness", "Smoothness value");
            public static GUIContent specularityText = new GUIContent("Specularity", "Specularity value");
        }

        MaterialProperty albedoMap = null;
        MaterialProperty bumpMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty smoothness = null;
        MaterialProperty specularity = null;

        MaterialEditor m_MaterialEditor;
        ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);

        bool m_FirstTimeApply = true;

        public void FindProperties(MaterialProperty[] props)
        {
            albedoMap = FindProperty("_MainTex", props);
            bumpMap = FindProperty("_BumpTex", props);
            albedoColor = FindProperty("_Color", props);
            smoothness  = FindProperty("_Smoothness", props);
            specularity = FindProperty("_Specularity", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a standard shader.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (m_FirstTimeApply)
            {
                //MaterialChanged(material, m_WorkflowMode);
                m_FirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                // Primary properties
                m_MaterialEditor.TexturePropertySingleLine(Styles.albedoMapText, albedoMap);
                m_MaterialEditor.TexturePropertySingleLine(Styles.bumpMapText, bumpMap);
                m_MaterialEditor.ShaderProperty(albedoColor, Styles.albedoColorText);
                m_MaterialEditor.ShaderProperty(smoothness, Styles.smoothnessText);
                m_MaterialEditor.ShaderProperty(specularity, Styles.specularityText);
            }
            if (EditorGUI.EndChangeCheck())
            {
                
            }
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
        }

        static void SetMaterialKeywords(Material material)
        {
            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)
            SetKeyword(material, "_BUMPMAP", material.GetTexture("_BumpTex"));
        }

        static void MaterialChanged(Material material)
        {
            SetMaterialKeywords(material);
        }

        static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }
    }

} // namespace UnityEditor
