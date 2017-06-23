using UnityEngine;
using UnityEngine.VR;

public class ToggleVR : MonoBehaviour
{
    private bool toggleVR = false;

    void Start()
    {
        SetVRConfiguration(false);
        VRSettings.enabled = false;
    }

    void Update()
    {
        //If Space is pressed, toggle VRSettings.enabled
        if (Input.GetKeyDown(KeyCode.Space) && VRDevice.isPresent)
        {
            SetVRConfiguration(!VRSettings.enabled);

            toggleVR = true;
        }

    }

    // Enable of VR device is done after Update to 
    // give time to the camera to change the settings.
    void FixedUpdate()
    {
        if (toggleVR)
        {
            VRSettings.enabled = !VRSettings.enabled;
        }

        toggleVR = false;
    }

    /**
     * Function to change between VR on and off.
     */
    void SetVRConfiguration(bool on)
    {
        MainCameraController c = FindObjectOfType<MainCameraController>();

        if (on)
        {
            c.SetCameraPerspective(MainCameraController.CAMERA_PERSPECTIVE.FIRST_PERSON);
            VRSettings.renderScale = 1.25f;
        }
        else
        {
            c.SetCameraPerspective(MainCameraController.CAMERA_PERSPECTIVE.THIRD_PERSON);
        }

        SetHeroVisibility(!on);
        SetVRUIVisibility(on);
        DisableNavMeshObstacles(on);
    }

    /**
     * Function to make the main character invisible on VR mode.
     */
    void SetHeroVisibility(bool visible)
    {
        GameObject heroMesh = GameObject.Find("HeroMesh");
        GameObject heroHair = GameObject.Find("HeroHair");

        if (heroMesh == null || heroHair == null) return;

        heroMesh.GetComponent<SkinnedMeshRenderer>().enabled = visible;
        heroHair.GetComponent<MeshRenderer>().enabled = visible;
    }

    void SetVRUIVisibility(bool visible)
    {
        VRUIElement [] UI_WorldSpaceElements = FindObjectsOfType<VRUIElement>();

        foreach (VRUIElement e in UI_WorldSpaceElements)
        {
            e.SetVisibility(visible);
        }
    }

    void DisableNavMeshObstacles(bool disable)
    {
        GameObject constructionsContainer = GameObject.Find("Constructions");
        GameObject natureContainer = GameObject.Find("Nature");
        
        foreach (Transform constructionsChildren in constructionsContainer.transform)
        {
            UnityEngine.AI.NavMeshObstacle obstacle = constructionsChildren.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();

            if(obstacle) obstacle.enabled = !disable;
        }

        foreach (Transform natureChildren in natureContainer.transform)
        {
            UnityEngine.AI.NavMeshObstacle obstacle = natureChildren.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();

            if(obstacle) obstacle.enabled = !disable;
        }
    }

}