using UnityEngine;

/**
 * VRUIElement represents an element on the VR User Interface.
 *
 * CANT put inside namespace or Unity will not allow to attach 
 * inheriting classes to a gameObject.
 */
public class VRUIElement : MonoBehaviour
{

    // Canvas where the UI Element is located
    protected Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public void SetVisibility(bool visibility)
    {
        // Disable all children of the gameobject VRUIElement
        foreach (Transform child in gameObject.transform)
            child.gameObject.SetActive(visibility);
    }
}