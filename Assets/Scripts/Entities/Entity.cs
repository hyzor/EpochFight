using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour, IEntityMessageHandler
{
    public int maxHealth = 1;
    public int curHealth = 1;

    public bool isAlive = true;
    public bool deathTrigger = false;
    public bool flaggedForRemoval = false;
    private Renderer entityRenderer;
    private Unit unitComponent;

    private TextMesh statusText;
    public List<string> statusTextElements = new List<string>();

    private Color dmgColorStart = Color.white;
    private Color dmgColorEnd = Color.red;
    private float dmgDuration = 0.5f;
    private Color matColorCache;

    private bool removalStarted = false;
    public int statusTextIndex = 0;

    private Collider myCollider;
    private AttackScript attackScript;

    public void ReceiveDamage(int dmg, GameObject src)
    {
        curHealth -= dmg;
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
        statusText = this.gameObject.GetComponentInChildren<TextMesh>();

        if (entityRenderer == null)
        {
            entityRenderer = GetComponentInChildren<Renderer>();
        }

        matColorCache = entityRenderer.material.color;

        unitComponent = this.gameObject.GetComponent<Unit>();

        NavMeshAgent navMesh = this.gameObject.GetComponent<NavMeshAgent>();
        attackScript = this.gameObject.GetComponent<AttackScript>();
        this.myCollider = this.gameObject.GetComponent<Collider>();
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

    public void InsertStatusTextElement(int index, string text)
    {
        if (index + 1 > statusTextElements.Count)
        {
            int gap = (index + 1) - statusTextElements.Count;

            for (int i = 0; i < gap - 1; ++i)
            {
                statusTextElements.Add("");
            }

            statusTextElements.Add(text);
        }
        else
        {
            statusTextElements[index] = text;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(this.gameObject.name + " trigger enter with " + other.name);
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(this.gameObject.name + " trigger stay with " + other.name);
    }

    // Update is called once per frame
    void Update ()
    {
        InsertStatusTextElement(statusTextIndex, curHealth + "/" + maxHealth);

        if (statusText != null)
        {
            statusText.transform.rotation = Camera.main.transform.rotation;
            statusText.text = "";

            foreach (string element in statusTextElements)
            {
                statusText.text += element;
            }
        }

        if (flaggedForRemoval && !removalStarted)
        {
            StartCoroutine(DestroyAfter(5.0f));
        }

        if (curHealth <= 0)
        {
            if (myCollider != null)
                myCollider.enabled = false;

            deathTrigger = true;
        }
	}
}
