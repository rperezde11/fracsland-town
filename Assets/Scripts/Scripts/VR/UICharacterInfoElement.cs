using UnityEngine;

/**
 * UICharacterInfoElement objects give info about the character they are on.
 */
public class UICharacterInfoElement : VRUIElement
{
    public float rotationSpeed = 5f;

    void Start()
    {
        if (canvas == null)
        {
            throw new System.Exception("GameObjects using class UICharacterInfoElement must have a Canvas Component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas == null) return;

        canvas.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
    }
}