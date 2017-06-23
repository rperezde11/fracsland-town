using UnityEngine;
using System.Collections;

/// <summary>
/// Used to perform a little animation with the splash camera.
/// </summary>
public class SplashCamera : MonoBehaviour {

	//Used for camera transition
	private const float CAMERA_MIN_HEIGHT = 3.0f;
	private const float CAMERA_SPEED = 40.0f;


	void Update () {
		if(this.transform.position.y > CAMERA_MIN_HEIGHT){
			this.transform.Translate(Vector3.down * CAMERA_SPEED * Time.deltaTime);
		}
	}
}
