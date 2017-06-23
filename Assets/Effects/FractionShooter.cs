using UnityEngine;
using System.Collections;
using Utils;

public class FractionShooter : MonoBehaviour {

	GameObject _fractionProjectilePrefab, _target;

	void Awake(){
		_fractionProjectilePrefab = Resources.Load (Constants.PATH_RESOURCES + "Level 1/FractionProjectile") as GameObject;
	}
		

	public void ShootFraction(GameObject target){
		GameObject fractionProj = Instantiate (_fractionProjectilePrefab, transform.position, transform.rotation) as GameObject;
		fractionProj.GetComponent<FractionProjectile> ().target = target;
		_target = null;
	}
}
