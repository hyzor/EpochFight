using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public Vector3 dir;
    public GameObject srcObject;
    public AudioClip[] hitSounds;
    public float lifetime = 5.0f;
    private float startTime;

	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.time - startTime >= lifetime)
        {
            Destroy(this.gameObject);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        // Do not collide with source gameobject
        if (other.gameObject == srcObject)
            return;

        // Do not collide with other projectiles
        if (other.gameObject.GetComponent<Projectile>() != null)
            return;

        // Do not collide with friendly entities
        if (srcObject.GetComponent<Enemy>() != null && other.gameObject.GetComponent<Enemy>() != null
            || srcObject.GetComponent<Enemy>() == null && other.gameObject.GetComponent<Enemy>() == null)
            return;

        Entity otherEntity = other.gameObject.GetComponent<Entity>();

        // Make sure the collider receives damage
        if (otherEntity != null)
        {
            ExecuteEvents.Execute<IEntityMessageHandler>(other.gameObject, null, (x, y) => x.ReceiveDamage(damage, srcObject));

            SoundManager.instance.RandomizeSfx(hitSounds);

            // Kill self
            Destroy(this.gameObject);
        }
    }
}
