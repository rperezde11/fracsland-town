using UnityEngine;

/// <summary>
/// A bird wich flies inside a network of nodes
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Bird : MonoBehaviour {

    Animation _animation;
    GameObject[] _birdNodes;
    private GameObject _actualDestination;
    protected float birdSpeed = 3f;
    public AudioClip sound;
    protected AudioSource audioSource;
    private float _nextSoundPlay = 0f;

    void Awake()
    {
        _animation = GetComponent<Animation>();
        audioSource = GetComponent<AudioSource>();
        _animation.Play("fly");
    }

    void Start () {
		Debug.Log ("Birds Started");
        _birdNodes = GameObject.FindGameObjectsWithTag("BirdNode"); 
        _actualDestination = SelectRandomNode();
	}
	
	void Update ()
    {
        MoveToTarget();
        UpdateSound();
	}

    void MoveToTarget()
    {
        if(Vector3.Distance(_actualDestination.transform.position, transform.position) <= 0.2f)
        {
			if (_actualDestination != null)
				_actualDestination = SelectRandomNode ();
			else
				SelectRandomNode ();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _actualDestination.transform.position, Time.deltaTime * birdSpeed);
            var lookpos = new Vector3(_actualDestination.transform.position.x, _actualDestination.transform.position.y + 2.4f, _actualDestination.transform.position.z) - transform.position;
            lookpos.y = 0;
            var targetRotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
        }
    }

    void UpdateSound()
    {
        if(Vector3.Distance(transform.position, LevelController.instance.Hero.transform.position) < 5 && Time.time >= _nextSoundPlay)
        { 
            if(Random.Range(0, 10) > 5)
            {
                audioSource.PlayOneShot(sound);
            }
            _nextSoundPlay = Time.time + sound.length;
        }
    }

    GameObject SelectRandomNode()
    {
        return _birdNodes[Random.Range(0, _birdNodes.Length)];
    }
}
