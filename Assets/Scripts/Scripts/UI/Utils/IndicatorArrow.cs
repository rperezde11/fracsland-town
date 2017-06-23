using UnityEngine;

/// <summary>
/// Alwais look thrught the quest target
/// </summary>
public class IndicatorArrow : MonoBehaviour {

    public static IndicatorArrow instance;

    private GameObject _target = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if(_target != null)
            transform.LookAt(new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z));
	}

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
}
