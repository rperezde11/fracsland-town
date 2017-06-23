using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the movement of an entity.
/// </summary>
public class MovementManager : MonoBehaviour {

	public float speed = 2f;
	Vector3 _destination;

    bool _isMovementForced = false;

	private delegate void UpdateFunction ();
	UpdateFunction _actualUpdate;

	bool _isMoving;
	public bool isMoving { get { return _isMoving; }}
	private UnityEngine.AI.NavMeshAgent _agent;
    public UnityEngine.AI.NavMeshAgent NavAgent { get { return _agent; } set { _agent = value; } }

	private Vector3 targetToFollow;

    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }

	void Awake(){
		_agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	void Update () 
	{
		if(_actualUpdate != null) _actualUpdate ();
	}

	void ForcedMovement()
	{
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, _destination, step);
		CheckIsMoving ();
	}

	void CheckIsMoving()
	{
		if (Vector3.Distance (transform.position, _destination) > 0.2f) {
			_isMoving = true;
		} else {
			_isMoving = false;
		}
	}

	public bool SetNewDestination(Vector3 destination, bool forced=false)
	{
        _isMovementForced = forced;
		_destination = destination;
        if (_agent == null) return false;
		if (forced) {
            _agent.Stop();
			_agent.enabled = false;
			_actualUpdate = ForcedMovement;
            return true;
		} else {
			_agent.enabled = true;
			_actualUpdate = null;
			return _agent.SetDestination(destination);
		}
	}

	public void StopMovement()
	{
		_actualUpdate = null;
		_isMoving = false;
		_agent.enabled = false;
	}

    public void PauseMovement()
    {
        _isPaused = true;
        _agent.speed = 0f;
    }

    public void RestoreMovement()
    {
        _isPaused = false;
        _agent.speed = 2f;   
    }
}
