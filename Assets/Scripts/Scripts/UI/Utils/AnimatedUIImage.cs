using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to make images act like gifs
/// </summary>
public class AnimatedUIImage : MonoBehaviour {

    public Sprite[] sprites;
    public float animationSpeed = 0.3f;    
    private Image image;
    private float nextChange = 0f;
    private int actualTextureIndex = 0;

    void Awake()
    {
        image = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
	    if(nextChange < Time.time)
        {
            actualTextureIndex++;
            if(actualTextureIndex >= sprites.Length)
            {
                actualTextureIndex = 0;
            }

            image.sprite = sprites[actualTextureIndex];
        }
	}
}
