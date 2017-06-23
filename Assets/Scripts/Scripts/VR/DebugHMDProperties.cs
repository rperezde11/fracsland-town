using UnityEngine;
using UnityEngine.VR;


public class DebugHMDProperties : MonoBehaviour
{
    public float updateEvery = 0.5f;
    public bool disable = true;

    private float _timer = 0f;

    GameObject[] eyes = new GameObject[2];
    string[] eyeAnchorNames = { "LeftEyeAnchor", "RightEyeAnchor" };

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < updateEvery || disable) return;
    
        for (int i = 0; i < 2; ++i)
        {
            // If the eye anchor is no longer a child of us, don't use it
            if (eyes[i] != null && eyes[i].transform.parent != transform)
            {
                eyes[i] = null;
            }

            // If we don't have an eye anchor, try to find one or create one
            if (eyes[i] == null)
            {
                Transform t = transform.Find(eyeAnchorNames[i]);
                if (t)
                    eyes[i] = t.gameObject;

                if (eyes[i] == null)
                {
                    eyes[i] = new GameObject(eyeAnchorNames[i]);
                    eyes[i].transform.parent = gameObject.transform;
                }
            }

            // Update the eye transform
            Debug.Log( "Eye " + i + "position: " + InputTracking.GetLocalPosition((VRNode)i) );
            Debug.Log( "Eye " + i + "rotation: " + InputTracking.GetLocalRotation((VRNode)i) );
        }

        _timer = 0;
    }
}
