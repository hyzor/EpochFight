using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {
    public float noiseFrequency;
    public float noiseFrequencyVariation;
    private float timeSinceLastNoise;
    private float nextVariation;
    private Entity entity;

    public List<AudioClip> noiseSounds = new List<AudioClip>();

	// Use this for initialization
	void Start ()
    {
        timeSinceLastNoise = Time.time;
        nextVariation = Random.Range(-noiseFrequencyVariation, noiseFrequencyVariation);
        entity = this.gameObject.GetComponent<Entity>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (noiseSounds.Count > 0 && entity.isAlive)
        {
            if (Time.time - timeSinceLastNoise >= noiseFrequency + nextVariation)
            {
                SoundManager.instance.RandomizeSfx(noiseSounds.ToArray());
                timeSinceLastNoise = Time.time;
                nextVariation = Random.Range(-noiseFrequencyVariation, noiseFrequencyVariation);
            }
        }
	}
}
