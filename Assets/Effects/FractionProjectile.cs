using UnityEngine;
using System.Collections;

public class FractionProjectile : MonoBehaviour {

	float _rotationSpeed = 1000;
	float speed = 10;

	public GameObject target = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(_rotationSpeed * Time.deltaTime, 0 , 0);

		if (target != null) {
			transform.position = Vector3.MoveTowards (this.transform.position, target.transform.position, speed * Time.deltaTime);
			if (Vector3.Distance (this.transform.position, target.transform.position) < 0.1f) {
				Destroy (gameObject);
			}
		}

	}
}
