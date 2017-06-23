using UnityEngine;
using System.Collections;
using Items;
using Utils;

/// <summary>
/// Handles the behaivoiur of the bridge
/// </summary>
public class Bridge : MonoBehaviour {

    public GameObject bridgeWoodPrefab;
    public static float BRIDGE_WIDTH = 6.5f;
    public static float BRIDGE_OFFSET = 0.2f;
    public GameObject[] drownNodes;

    public void constructBridge(int numerator, int denominator)
    {
        Vector3 bridgePosition = new Vector3(57.2f, 10.328f, 59.1f);
        float woodWidth = (BRIDGE_WIDTH - (denominator - 1) * BRIDGE_OFFSET) / denominator;
        bridgePosition.z -= woodWidth / 2;
		int numActiveParts = denominator - numerator;

        for (int i = 0; i < denominator; i++)
        {
            bridgePosition.z += (woodWidth + BRIDGE_OFFSET);
            GameObject wood = (GameObject)Instantiate(bridgeWoodPrefab, bridgePosition, this.transform.rotation);
            wood.transform.localScale = new Vector3(wood.transform.localScale.x, wood.transform.localScale.y, woodWidth);
			if(i >= numActiveParts)
            {
                wood.GetComponent<BridgeWood>().setTransparent(true);
            }
        }
        
    }

    public void DisableColliders()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
}
