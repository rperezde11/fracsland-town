using UnityEngine;
using System.Collections;

/// <summary>
/// Used to make a game object bounce
/// </summary>
public class ConstantBounce : MonoBehaviour {

    public float bounceDinstance = 0.4f;
    public float bounceSpeed = 0.1f;
    private Vector3 _desiredPosition;
    private int direction = 1;
    public Vector3 BounceDirection = new Vector3(0f, 1f, 0f);

	void Start () {
        _desiredPosition = transform.position + BounceDirection * (bounceDinstance / 2);
    }
	
	void Update () {
	    if(Vector3.Distance(_desiredPosition, transform.position) > 0)
        {
                transform.position = Vector3.MoveTowards(transform.position, _desiredPosition, Time.deltaTime * bounceSpeed);
        }
        else
        {
            _desiredPosition = transform.position + BounceDirection * bounceDinstance * direction;
            direction *= -1;
        }
	}
}
