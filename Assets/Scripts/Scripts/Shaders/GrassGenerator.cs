using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassGenerator : MonoBehaviour {

    public enum BillboardFollowY
    { 
        ALWAYS,
        HALF,
        NEVER
    }

    [Range(0f,1f)] public float density  = 1;
    [Range(0f,1f)] public float wellness = 1;
    [Range(0f,1f)] public float variety  = 1;
    [Range(0f,1f)] public float scale    = 0;

    [Range(1,5)] public int numberOfPlanes = 1;


    public Vector3 windDirection;
    [Range(0f,1f)] public float windIntensity;
    public BillboardFollowY billboardY = BillboardFollowY.HALF;
    [Range(0f,1f)] public float cutoff;

    private Vector3 _windDirection;
    private float _windIntensity;
    private BillboardFollowY _billboardY;
    private float _scale;
    private float _cutoff;

    private Vector2 minmax_x;
    private Vector2 minmax_z;

    private Vector3 _baseSize;
    private Vector3 _leftBottomPos;

    private List<Material> _grassMaterials;
    private List<Material> _usedMaterials;
    private List<Material> _grassTypesMaterials;

    enum GrassType { A, B, C }
    enum GrassRipeness { GREEN, YELLOW, BROWN }

    void Start ()
    {
        SetupInfo();
        FillGrassMaterialsPool();
        FillArea();
    }

    void Update()
    {
        HandlePropertyChange();
    }

    void FillArea()
    {
        Vector3 position = _leftBottomPos;

        while (position.z <= (_leftBottomPos.z + _baseSize.z))
        {
            while (position.x <= (_leftBottomPos.x + _baseSize.x))
            {
                float z_diff = Random.Range(-0.1f, 0.1f);
                position.z = position.z + z_diff;
                
                GenerateBillboardGrassUnit(position);

                position.x += Random.Range(minmax_x.x, minmax_x.y);
                position.z = position.z - z_diff;
            }

            position.z += Random.Range(minmax_z.x, minmax_z.y);
            position.x = _leftBottomPos.x;
        }
    }


    void GenerateBillboardGrassUnit(Vector3 position)
    {
        GameObject p = new GameObject();
        p.name = "GrassUnit";
        p.transform.parent = gameObject.transform;
        p.transform.localPosition = position;

        GameObject plane;

        float angleDiff = 180f / numberOfPlanes;
        float seed = Random.Range(0f, 1f);
        float sinSeed = Mathf.Abs((p.transform.position.x + p.transform.position.z));

        Material grass = _grassMaterials[Random.Range(0, _grassMaterials.Count - 1)];

        for (int i = 0; i < numberOfPlanes; i++)
        {
            plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "GrassUnitPlane" + i;
            plane.transform.parent = p.transform;
            grass = new Material(grass);

            grass.SetFloat("_Angle", -90.0f + (angleDiff / 2.0f) + i * angleDiff);
            grass.SetFloat("_Cutoff", 0.2f);
            grass.SetFloat("_Seed", seed);
            grass.SetFloat("_SinSeed", sinSeed);
            plane.GetComponent<MeshRenderer>().material = grass;
            plane.transform.localPosition = Vector3.zero;

            _usedMaterials.Add(grass);
        }
    }

    void SetupInfo()
    {
        _baseSize = gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
        Vector3 _baseScale = gameObject.transform.localScale;
        _leftBottomPos = new Vector3(-_baseSize.x * 0.5f, 5f*(scale+0.25f), -_baseSize.z * 0.5f);

        float densityFactor = Mathf.Lerp(1f, 0.5f, density);

        minmax_x.x = 0.5f * 10f * (scale+0.25f) * (1.0f/_baseScale.x);
        minmax_x.y = densityFactor * 10f * (scale+0.25f) * (1.0f/_baseScale.x);

        minmax_z.x = 0.5f * 10f * (scale+0.25f) * (1.0f/_baseScale.z);
        minmax_z.y = densityFactor * 10f * (scale+0.25f) * (1.0f/_baseScale.z);

        _grassTypesMaterials = new List<Material>(Resources.LoadAll<Material>("Materials/_Grass/CrossBillboardGrass/"));

        _grassMaterials = new List<Material>();
        _usedMaterials = new List<Material>();
    }

    void FillGrassMaterialsPool()
    {
        int numOfMaterials = 20;

        for(int i = 0; i < numOfMaterials; i++)
        {
            _grassMaterials.Add(GetNewGrassMaterial());
        }
    }

    private Material GetNewGrassMaterial()
    {
        Material grass = null;
     
        GrassType type = GetGrassType();
        GrassRipeness ripeness = GetGrassRipeness();

        grass = new Material(_grassTypesMaterials[(int)type]);

        AddColorToGrass(ref grass, ripeness);

        return grass;
    }

    private GrassRipeness GetGrassRipeness()
    {
        float randWellness = Random.Range(0f, 1f);

        float perGreen = wellness * wellness * (-2f * wellness + 3f);
        float perYellow = 0.1f;
        float perBrown = 1f - perGreen;

        Vector3 percentages = (new Vector3(perGreen, perYellow, perBrown)) / (perGreen + perYellow + perBrown);

        float min = 0;
        float max = 0;

        int i;

        for (i = 0; i < 3; i++)
        {
            max += percentages[i];

            if (randWellness >= min && randWellness <= max)
            {
                break;
            }

            min += percentages[i];
        }

        return (GrassRipeness)i;
    }

    private GrassType GetGrassType()
    {
        float randVariety = Random.Range(0f, 1f);

        float perA = 0.5f + 0.5f * variety;
        float perB = 1f - (perA/2f);
        float perC = perB;

        Vector3 percentages = (new Vector3(perA, perB, perC)) / (perA+perB+perC);

        float min = 0;
        float max = 0;

        int i;

        for (i = 0; i < 3; i++)
        {
            max += percentages[i];

            if (randVariety >= min && randVariety <= max)
            {
                break;
            }

            min += percentages[i];
        }

        return (GrassType)i;
    }

    private void AddColorToGrass(ref Material grass, GrassRipeness ripeness)
    {
        Color color;    

        switch (ripeness)
        {
            case GrassRipeness.GREEN:
                color = Color.white;
                break;
            case GrassRipeness.YELLOW:
                color = Color.yellow;
                break;
            case GrassRipeness.BROWN:
                color = new Color(1f, 0.75f, 0.25f);
                break;
            default:
                color = Color.white;
                break;

        }

        grass.SetColor("_TintColor", color);
    }

    private void HandlePropertyChange()
    {
        bool hasWDChanged = _windDirection != windDirection;
        bool hasWIChanged = _windIntensity != windIntensity;
        bool hasBillboardChanged = _billboardY != billboardY;
        bool hasScaleChanged = _scale != scale;
        bool hasCutoffChanged = _cutoff != cutoff;

        if (!hasWDChanged && !hasWIChanged && !hasBillboardChanged && !hasScaleChanged && !hasCutoffChanged) return;

        foreach (Material mat in _usedMaterials)
        {
            if (hasWDChanged)
            {
                mat.SetVector("_WindDirection", windDirection);
                _windDirection = windDirection;
            }

            if (hasWIChanged)
            {
                mat.SetFloat("_WindIntensity", windIntensity);
                _windIntensity = windIntensity;
            }

            if (hasBillboardChanged)
            {
                SetBillboardY(mat, billboardY);
                _billboardY = billboardY;
            }

            if (hasScaleChanged)
            {
                mat.SetFloat("_Scale", scale);
                _scale = scale;
            }
            
            if (hasCutoffChanged)
            {
                mat.SetFloat("_Cutoff", Mathf.Lerp(0.1f, 0.5f, cutoff));
                _cutoff = cutoff;
            }

        }
    }

    private void SetBillboardY(Material material, BillboardFollowY b)
    {
        switch (b)
        {
            case BillboardFollowY.ALWAYS:
                material.EnableKeyword("ALWAYS_FOLLOW_Y");
                material.DisableKeyword("HALF_FOLLOW_Y");
                material.DisableKeyword("NEVER_FOLLOW_Y");
                break;
            case BillboardFollowY.HALF:
                material.DisableKeyword("ALWAYS_FOLLOW_Y");
                material.EnableKeyword("HALF_FOLLOW_Y");
                material.DisableKeyword("NEVER_FOLLOW_Y");
                break;
            case BillboardFollowY.NEVER:
                material.DisableKeyword("ALWAYS_FOLLOW_Y");
                material.DisableKeyword("HALF_FOLLOW_Y");
                material.EnableKeyword("NEVER_FOLLOW_Y");
                break;
        }
    }
}
