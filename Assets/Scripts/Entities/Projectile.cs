using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public Vector3 dir;
    public GameObject srcObject;

	// Use this for initialization
	void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // Do not collide with source gameobject
        if (other.gameObject == srcObject)
            return;

        // Do not collide with other projectiles
        if (other.gameObject.GetComponent<Projectile>() != null)
            return;

        Entity otherEntity = other.gameObject.GetComponent<Entity>();

        // Make sure the collider receives damage
        if (otherEntity != null)
        {
            ExecuteEvents.Execute<IEntityMessageHandler>(other.gameObject, null, (x, y) => x.ReceiveDamage(damage, srcObject));

            // Kill self
            Destroy(this.gameObject);
        }
    }
}
