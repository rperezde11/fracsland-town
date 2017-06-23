using UnityEngine;
using System.Collections;

/// <summary>
/// used to make a game object rotate over itself
/// </summary>
public class ConstantRotation : MonoBehaviour {

    // Use this for initialization

    public float speed = 40f;
    public Vector3 rotationDir = new Vector3(0, 1, 0);

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotationDir.x * speed * Time.deltaTime, rotationDir.y * speed * Time.deltaTime, rotationDir.z * speed * Time.deltaTime);
    }
}
