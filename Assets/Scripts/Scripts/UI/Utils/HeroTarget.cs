using UnityEngine;

/// <summary>
/// Used to give some feedback of where the player wants to go
/// after a mouse click over the terrainshowing some particles
///  in the place selected.
/// </summary>
public class HeroTarget : MonoBehaviour {

    private const float EMISSION_TIME = 0.4f;
    private ParticleSystem _particles;
    private float _nextEmission = 0f;

    void Awake () {
        _particles = GetComponent<ParticleSystem>();
    }

	// Update is called once per frame
	void Update () {
	    if(_nextEmission <= Time.time && _particles.enableEmission)
        {
            _particles.enableEmission = false;
        }
	}

    public void MarkDestination()
    {
        _particles.enableEmission = true;
        _nextEmission = Time.time + EMISSION_TIME;
    }

    public void StopBlinking()
    {
        _particles.enableEmission = false;
    }
}
