using UnityEngine;

/// <summary>
/// Used to move the player between the scenes when he aprouches to this entity
/// </summary>
public class Portal : MonoBehaviour {

	public string DestinationScene = "Farm";
	public int portalID = 1;
    public GameObject spawnPosition;
    public Vector3 destinationPosition;
    public MainCameraController.CAMERA_ORIENTATION levelOrientation;

	void Update () {
        if (LevelController.instance.Hero == null) return;
		if (Vector3.Distance (LevelController.instance.Hero.transform.position, transform.position) < 1f) {
            LevelController.instance.RequestPositioningInfrontOfPortal(portalID, DestinationScene, destinationPosition);
        }	
	}
}
