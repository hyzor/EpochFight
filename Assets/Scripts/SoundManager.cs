using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource fxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

	// Use this for initialization
	void Awake ()
    {
		if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(this.gameObject);
	}

    public void PlaySingleClip(AudioClip clip)
    {
        fxSource.clip = clip;
        fxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randIdx = Random.Range(0, clips.Length);
        fxSource.clip = clips[randIdx];
        fxSource.Play();
    }
}
