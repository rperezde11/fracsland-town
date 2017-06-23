using UnityEngine;
using System.Collections;

public class SnowScript : MonoBehaviour {


	void Start () {
	
	}

	void Update () {
        this.transform.position = Camera.main.transform.position + new Vector3(0, 3f, 0);
	}
}
