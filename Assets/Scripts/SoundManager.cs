using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static int numFxSources = 10;
    public AudioSource[] fxSources = new AudioSource[numFxSources];

    //public AudioSource fxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    void OnValidate()
    {
        if (fxSources.Length < numFxSources)
        {

        }
    }

    // Use this for initialization
    void Awake ()
    {
		if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(this.gameObject);

        for (int i = 0; i < numFxSources; ++i)
        {
            fxSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySingleClip(AudioClip clip)
    {
        AudioSource audioSource = FindAvailableSource();

        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private AudioSource FindAvailableSource()
    {
        foreach (AudioSource audioSource in fxSources)
        {
            if (!audioSource.isPlaying)
                return audioSource;
        }

        return null;
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randIdx = UnityEngine.Random.Range(0, clips.Length);

        AudioSource audioSource = FindAvailableSource();

        if (audioSource != null)
        {
            audioSource.clip = clips[randIdx];
            audioSource.Play();
        }
    }
}
