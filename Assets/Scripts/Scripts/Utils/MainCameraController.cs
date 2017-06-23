using UnityEngine;
using System.Collections;
using UnityEngine.VR;

/// <summary>
/// Used to make the camera follow the hero.
/// </summary>
public class MainCameraController : MonoBehaviour
{
    GameObject[] eyes = new GameObject[2];
    string[] eyeAnchorNames = { "LeftEyeAnchor", "RightEyeAnchor" };

    public static MainCameraController instance = null;
    GameObject _target, _cameraContainer;
    public enum CAMERA_ORIENTATION { NORTH, WEST, SOUTH, EAST }
    public enum CAMERA_PERSPECTIVE { FIRST_PERSON, THIRD_PERSON }
    private CAMERA_PERSPECTIVE perspective;
    private float zOffset, xOffset;

    void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            _cameraContainer = new GameObject("Camera");
            transform.parent = _cameraContainer.transform;
            transform.localPosition = Vector3.zero;
            DontDestroyOnLoad(_cameraContainer);
            
            perspective = CAMERA_PERSPECTIVE.THIRD_PERSON;
            ChangeCameraOrientation(CAMERA_ORIENTATION.NORTH);
        }
    }

    void Start()
    {
        ChangeCameraOrientation(GameObject.Find("Portal").GetComponent<Portal>().levelOrientation);
    }


    void Update()
    {
        if (_target != null)
        {
            if (perspective == CAMERA_PERSPECTIVE.FIRST_PERSON)
            {
                _cameraContainer.transform.position = new Vector3(_target.transform.position.x, 12.34144f, _target.transform.position.z);

                if (VRSettings.enabled)
                {
                    Camera.main.fieldOfView = 96;
                    gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                    _cameraContainer.transform.rotation = Quaternion.Euler(new Vector3(0, _target.transform.localEulerAngles.y, 0));
                }
            }

            if (perspective == CAMERA_PERSPECTIVE.THIRD_PERSON)
            {
                Camera.main.fieldOfView = 60;
                _cameraContainer.transform.position = new Vector3(_target.transform.position.x + xOffset, 17.34144f, _target.transform.position.z + zOffset);
                ChangeCameraOrientation(CAMERA_ORIENTATION.NORTH);
            }
        }
    }

    public void AttachCameraToGameObject(GameObject target)
    {
        _target = target;
    }

    public void SetCameraPerspective(CAMERA_PERSPECTIVE perspective)
    {
            this.perspective = perspective;
    }

    public void ChangeCameraOrientation(CAMERA_ORIENTATION orientation)
    {
        float yRoot = 0f; ;
        switch (orientation)
        {
            case CAMERA_ORIENTATION.NORTH:
                zOffset = 3f;
                xOffset = 0;
                yRoot = 180f;
                break;
            case CAMERA_ORIENTATION.WEST:
                zOffset = 0;
                xOffset = -3f;
                yRoot = 90;
                break;
            case CAMERA_ORIENTATION.SOUTH:
                zOffset = -3f;
                xOffset = 0f;
                yRoot = 0f;
                break;
            case CAMERA_ORIENTATION.EAST:
                zOffset = 0;
                xOffset = 3f;
                yRoot = 270;
                break;
        }

        gameObject.transform.rotation = Quaternion.Euler(58.3f, yRoot, 0f);
    }


}
