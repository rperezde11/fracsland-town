using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Refraction : MonoBehaviour {

    public float refractionIndex1 = 1.00f;
    public float refractionIndex2 = 1.33f;

    private Camera refractionCamera;
    private Camera mainCamera;
    private RenderTexture _renderTextureRefraction;
    private Material _material;

    private Mesh mesh;

    private Vector3 mainCameraPosition, _mainCameraPosition;
    private Quaternion mainCameraRotation, _mainCameraRotation;
    private Vector3 planePosition, _planePosition;
    private Quaternion planeRotation, _planeRotation;
    

    // This is called only when the object 
    // is going to be rendered.
    void OnWillRenderObject()
    {
        //Debug.Log(refractionCamera == null);
        
        if (refractionCamera == null)
        {
            CreateCamera();

            _material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

            if (Application.isPlaying) {
                _material.EnableKeyword("INVERTED_Y");
            } else {
                _material.DisableKeyword("INVERTED_Y");
            }

            _renderTextureRefraction = new RenderTexture(Screen.width, Screen.height, 1);
            refractionCamera.targetTexture = _renderTextureRefraction;
            _material.SetTexture("_RefractionTex", _renderTextureRefraction);
        }

        UpdateRefractionCameraPositionAndRotation();
    }

    void OnDisable()
    {
        if (refractionCamera != null)
        {
            Debug.Log("Destroying Camera " + refractionCamera.GetInstanceID() + " " + (Application.isPlaying ? "play" : "edit"));
            DestroyImmediate(refractionCamera.gameObject);
            refractionCamera = null;
        }
    }

    void CreateCamera()
    {
        GameObject cameraGO = new GameObject("Refraction Camera", typeof(Camera));

        refractionCamera = cameraGO.GetComponent<Camera>();
        refractionCamera.depthTextureMode = DepthTextureMode.Depth;
        mainCamera = Camera.current;

        Debug.Log("Create Camera " + refractionCamera.GetInstanceID() + " " + (Application.isPlaying ? "play" : "edit"));

        refractionCamera.CopyFrom(mainCamera);
        refractionCamera.farClipPlane = 150;
        refractionCamera.cullingMask = 7; // Avoid Rendering the layers Water and UI
        //refractionCamera.hideFlags = HideFlags.DontSave;
    }

    private Vector4 ComputeAngles()
    {
        float refractionCoefficient;
        Vector3 normal, forward;
        Vector2 sines1, sines2, cosines2;
        Vector2 angles1, angles2;
        Vector3 rotation = Vector3.zero;

        refractionCoefficient = refractionIndex2 / refractionIndex1;

        normal  = mesh.normals[0];
        forward = refractionCamera.transform.forward;

        cosines2 = new Vector2(
            forward.x * normal.x + forward.y * normal.y,
            forward.y * normal.y + forward.z * normal.z
        );

        sines2 = new Vector2(
            1 - cosines2.x * cosines2.x,
            1 - cosines2.y * cosines2.y
        );

        sines2.x = Mathf.Sqrt(sines2.x);
        sines2.y = Mathf.Sqrt(sines2.y);

        sines1 = refractionCoefficient * sines2;
        
        angles1.x = Mathf.Asin(sines1.x) * Mathf.Rad2Deg;
        angles1.y = Mathf.Asin(sines1.y) * Mathf.Rad2Deg;

        angles2.x = Mathf.Asin(sines2.x) * Mathf.Rad2Deg;
        angles2.y = Mathf.Asin(sines2.y) * Mathf.Rad2Deg;

        return new Vector4(angles1.x, angles1.y, angles2.x, angles2.y);
    }

    private void UpdateRefractionCameraPositionAndRotation()
    {
        Mesh mesh;
        Ray camForward;
        RaycastHit info;

        mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        camForward = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        CollisionOnPlaneHunter(camForward, out info, 20, gameObject.transform.position, mesh);

        Vector3 r = Vector3.Normalize(Refract(camForward.direction, mesh.normals[0], refractionIndex1/refractionIndex2));
        Vector3 newpoint = info.point - r * info.distance;

        newpoint = Vector3.Lerp(mainCamera.transform.position, newpoint, 0.25f);
        refractionCamera.transform.position = newpoint;
        refractionCamera.transform.forward = info.point - newpoint;
    }

    private void CollisionOnPlaneHunter(Ray ray, out RaycastHit hitInfo, float clampDistance, Vector3 worldCenter, Mesh mesh)
    {
        Vector3 p0;
        Vector3 l, l0;
        float d;

        p0 = worldCenter;
        
        l = ray.direction;
        l0 = ray.origin;
        
        d = Vector3.Dot((p0 - l0), mesh.normals[0]);
        d /= Vector3.Dot(l, mesh.normals[0]);

        d = d < 0 ? clampDistance : d;

        if (d > clampDistance)
        {
            Debug.Log("max distance reached");
            Vector3 clampedPoint = l0 + clampDistance * l;
            Ray normalRay = new Ray(clampedPoint, -mesh.normals[0]);
            CollisionOnPlaneHunter(normalRay, out hitInfo, 100, p0, mesh);
            hitInfo.distance = Vector3.Distance(ray.origin, hitInfo.point);
        }
        else 
        {
            hitInfo = new RaycastHit();
            hitInfo.point = l0 + d * l;
            hitInfo.distance = Vector3.Distance(ray.origin, hitInfo.point);
        }
    }

    private Vector3 Refract(Vector3 direction, Vector3 normal, float eta)
    {
        float k = 1.0f - eta * eta * (1.0f - Vector3.Dot(normal, direction) * Vector3.Dot(normal, direction));
        
        return ((k < 0) ? 0 : 1) * eta * direction - (eta * Vector3.Dot(normal, direction) + Mathf.Sqrt(k)) * normal;
    }

}
