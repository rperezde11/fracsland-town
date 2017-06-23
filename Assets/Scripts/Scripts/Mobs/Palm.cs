using UnityEngine;
using System.Collections;
using Items;
using Utils;
using Quests;


/// <summary>
/// Defines the behaivour of a palm
/// </summary>
public class Palm : MonoBehaviour {

    Rigidbody rb;
	AudioClip cutEffect, destroyEffect;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
		cutEffect = Resources.Load (Constants.PATH_TO_SOUNDS + "atxe_hit_wood") as AudioClip;
		destroyEffect = Resources.Load (Constants.PATH_TO_SOUNDS + "got_wood") as AudioClip;
    }

    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CutPalm()
    {
        StartCoroutine(CutEffect());
    }

    private IEnumerator CutEffect()
    {
		JukeBox.instance.PlayWithTimeOut (cutEffect, 1f, 0.4f);
		JukeBox.instance.PlayWithTimeOut (cutEffect, 1f, 1.3f); 
        yield return new WaitForSeconds(1.4f);
		JukeBox.instance.Play (destroyEffect, 1); 
		rb.isKinematic = false;
        rb.AddForce(new Vector3(UnityEngine.Random.RandomRange(10, 20), 10, UnityEngine.Random.RandomRange(10, 20)));
        QuestManager.instance.Inventory.addItem(Item.Wood, UnityEngine.Random.Range(1, 4));
		LevelController.instance.Hero.canFireEvents = true;
        Destroy(this.gameObject, 1.4f);
    }
}
