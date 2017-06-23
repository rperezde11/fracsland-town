using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FogGenerator : MonoBehaviour {

    [Range(0.0f, 1.0f)] public float fogIntensity = 0.5f;
    [Range(0.0f, 1.0f)] public float speed = 0.5f;
    public FogMode fogMode = FogMode.ExponentialSquared;

    private bool fogOn;
    private float _speed;
    private float _fogIntensity;
    private FogMode _fogMode;

    private MeshRenderer meshRenderer;
    private Material fogMat;

    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if (meshRenderer == null)
            throw new System.Exception("There is no MeshRenderer Component on Fog plane");

        fogMat = meshRenderer.sharedMaterial;

        _fogMode = FogMode.Exponential;
        _fogIntensity = 0.01f;
    }

    void Update()
    {
        ActualizeSpeed();
        ActualizeFogIntensity();
    }

    void ActualizeSpeed()
    {
        if (speed != _speed)
        {
            fogMat.SetFloat("_Speed", speed);

            _speed = speed;
        }
    }

    void ActualizeFogIntensity()
    {
        bool fogModeChanged = false;
            
        if (fogMode != _fogMode)
        {
            RenderSettings.fogMode = fogMode;

            fogModeChanged = true;
            _fogMode = fogMode;
        }

        if (fogIntensity != _fogIntensity || fogModeChanged)
        {
            if (fogMode == FogMode.Linear)
            {
                RenderSettings.fogStartDistance = 0;
                RenderSettings.fogEndDistance = 30 - 30*fogIntensity;
            }
            else 
            {
                RenderSettings.fogDensity = 0.1f * fogIntensity;
            }
            
            fogOn = (fogIntensity == 0) ? false : true;

            fogMat.SetFloat("_Cutoff", 1 - fogIntensity);
            RenderSettings.fog = fogOn;
            meshRenderer.enabled = fogOn;

            _fogIntensity = fogIntensity;
        }

    }

}
