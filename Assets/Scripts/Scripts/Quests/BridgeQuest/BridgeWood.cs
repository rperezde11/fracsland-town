using UnityEngine;
using System.Collections;

/// <summary>
/// Defines the behaivour of a part of the bridge
/// </summary>
public class BridgeWood : MonoBehaviour {

    public Material transparent;
    private Material initial;
    private Material glass;

    private Quaternion _initialRotation;
    private Vector3 _initialPosition;
	
    void Awake()
    {
        initial = this.GetComponent<Renderer>().material;
        transparent.color = new Color(1.0f, 1.0f, 0f, 0.0f);
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

	// Update is called once per frame
	void Update () {
    }

    public void setTransparent(bool isTransparent)
    {
        if (isTransparent)
        {
            gameObject.GetComponent<Renderer>().material = transparent;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = initial;
        }
        this.GetComponent<BoxCollider>().enabled = isTransparent;
    }

    public void resetPosition()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        transform.localRotation = _initialRotation;
        transform.localPosition = _initialPosition;
    }
}
