using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Utils;

/// <summary>
/// This class is used to play sounds that we need to ensure that the player will hear
/// </summary>
public class JukeBox : MonoBehaviour {
    
    public static JukeBox instance;
     
    public AudioSource audioSource;
    public List<AudioClip> bso;
    public List<AudioClip> weather;
	public List<KeyValuePair<float , KeyValuePair<AudioClip, float>>> scheduledPlays;
	private Dictionary<string, AudioClip> _loadedSounds;


    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            Camera.main.gameObject.AddComponent<AudioSource>();
            audioSource = Camera.main.GetComponent<AudioSource>();

            scheduledPlays = new List<KeyValuePair<float, KeyValuePair<AudioClip, float>>>();
            _loadedSounds = new Dictionary<string, AudioClip>();
            DontDestroyOnLoad(gameObject);
        }

    }

	void Update(){
        if (scheduledPlays != null)
        {
            if (scheduledPlays.Count > 0)
            {
                foreach (var toPlaySound in scheduledPlays)
                {
                    if (Time.time >= toPlaySound.Key)
                    {
                        this.Play(toPlaySound.Value.Key, toPlaySound.Value.Value);
                        scheduledPlays.Remove(toPlaySound);
                    }
                }
            }
        } 
	}

	public void PlayWithTimeOut(AudioClip clip, float volume, float secondsToPlay){
		KeyValuePair<AudioClip, float> soundAndVolume = new KeyValuePair<AudioClip, float> (clip, volume);
		scheduledPlays.Add(new KeyValuePair<float, KeyValuePair<AudioClip, float>>(Time.time + secondsToPlay, soundAndVolume));
	}

    void OnLevelWasLoaded(int level)
    {
        try
        {
            audioSource = LevelController.instance.Hero.GetComponent<AudioSource>();
        }
        catch (Exception ex)
        {
            Camera.main.gameObject.AddComponent<AudioSource>();
            audioSource = Camera.main.GetComponent<AudioSource>();
        }
    }

	public void Play(AudioClip clip, float volume = 1, bool loop=false)
	{
		StartCoroutine(PlaySong(clip, volume, loop));
	}


	IEnumerator PlaySong(AudioClip clip, float volume = 1, bool loop=false)
    {
		if (loop) {
			audioSource.clip = clip;
			audioSource.Play();
			if(Constants.DEVELOPMENT) Debug.Log("JukeBox: Playing (loop mode=on) " + clip.name);
		} else {
			audioSource.PlayOneShot(clip, volume);
			if(Constants.DEVELOPMENT) Debug.Log("JukeBox: Playing (loop mode=off) " + clip.name);
		}
        
        
		yield return null;
    }

	public void StopAllSounds()
	{
		audioSource.Stop ();
	}

	public AudioClip LoadSound(string soundName)
	{
		if (_loadedSounds.ContainsKey (soundName)) {
			return _loadedSounds [soundName];
		} else {
			AudioClip sound = UnityEngine.Resources.Load (Constants.PATH_TO_SOUNDS + soundName) as AudioClip;
			_loadedSounds.Add (soundName, sound);
			return sound;
		}
	}

	public void LoadAndPlaySound(string soundName, float volume, bool is_loop=false){
		Play(LoadSound(soundName), volume, is_loop);
	}
}
