using UnityEngine;

/// <summary>
/// A bunch of particles that are only visible on the minimap usefull
/// to show the player where has to go.
/// </summary>
public class MinimapMarker : MonoBehaviour {

    public bool IsActive = false;

    private ParticleSystem _particles;
    public GameObject VisibleMarker = null;
    
    void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Used to enable a marker with the option to disable others
    /// </summary>
    /// <param name="disableOthers"></param>
    public void EnableMarker(bool disableOthers=true)
    {   
        if (disableOthers)
        {
            GameObject[] markers = GameObject.FindGameObjectsWithTag("MapMarker");
            foreach(GameObject marker in markers)
            {
                MinimapMarker m = marker.GetComponent<MinimapMarker>();
                m.DisableMarker();
            }
        }
        _particles.enableEmission = true;
        if(VisibleMarker != null) VisibleMarker.active = true;
    }

    public void DisableMarker()
    {
        _particles.enableEmission = false;
        if (VisibleMarker != null) VisibleMarker.active = false;
    }

}
