using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IEntityMessageHandler
{
    public int maxHealth = 1;
    public int curHealth = 1;
    public bool isAlive = true;
    public bool deathTrigger = false;
    public bool flaggedForRemoval = false;
    private Renderer entityRenderer;
    private Unit unitComponent;

    private Color dmgColorStart = Color.white;
    private Color dmgColorEnd = Color.red;
    private float dmgDuration = 0.5f;
    private Color matColorCache;

    private bool removalStarted = false;

    public void ReceiveDamage(int dmg, GameObject src)
    {
        curHealth -= dmg;

        //float lerp = Mathf.PingPong(Time.time, dmgDuration) / dmgDuration;
        //entityRenderer.material.color = Color.Lerp(dmgColorStart, dmgColorEnd, lerp);
        StartCoroutine(OnDamageReceived(dmgDuration));

        // Is this entity a unit?
        if (unitComponent != null)
            unitComponent.OnReceiveDamage(src);
    }

    private IEnumerator OnDamageReceived(float duration)
    {
        float startTime = Time.time;

        while (startTime + duration > Time.time)
        {
            float lerp = Mathf.PingPong(Time.time, duration) / duration;
            entityRenderer.material.color = Color.Lerp(dmgColorStart, dmgColorEnd, lerp);
            yield return null;
        }

        entityRenderer.material.color = matColorCache;
        yield return null;
    }

    // Use this for initialization
    void Start ()
    {
        curHealth = maxHealth;
        entityRenderer = GetComponent<Renderer>();

        if (entityRenderer == null)
        {
            entityRenderer = GetComponentInChildren<Renderer>();
        }

        matColorCache = entityRenderer.material.color;

        unitComponent = this.gameObject.GetComponent<Unit>();
    }

    private IEnumerator DestroyAfter(float duration)
    {
        removalStarted = true;
        float startTime = Time.time;

        while (startTime + duration > Time.time)
        {
            float lerp = Mathf.PingPong(Time.time, duration) / duration;
            entityRenderer.material.color = Color.Lerp(dmgColorStart, dmgColorEnd, lerp);
            yield return null;
        }

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        if (flaggedForRemoval && !removalStarted)
        {
            StartCoroutine(DestroyAfter(5.0f));
        }

        if (curHealth <= 0)
        {
            deathTrigger = true;
        }
	}
}
