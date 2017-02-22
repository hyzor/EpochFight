using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackScript : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public enum AttackType
    {
        MELEE_ONEHANDED = 1,
        RANGED_BOW = 2
    }

    public float speed = 1.0f;
    public float range = 1.0f;
    public int damage = 1;
    public AttackType attackType = AttackType.MELEE_ONEHANDED;
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public GameObject targetObj;
    private Animator anim;

    // Use this for initialization
    void Start ()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();

        // Make sure that the attack range is at least as big as the NavMesh's stopping distance for melee units
        if (navMeshAgent != null && navMeshAgent.stoppingDistance < range && !isRanged)
            range = navMeshAgent.stoppingDistance;

        anim = this.gameObject.GetComponent<Animator>();
    }

    public void BeginAttack()
    {
        switch (attackType)
        {
            case AttackType.MELEE_ONEHANDED:
                if (anim != null)
                {
                    anim.SetFloat("Speed_f", 0.0f);
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 12);
                    anim.SetInteger("Animation_int", 0);
                }
                break;
            case AttackType.RANGED_BOW:
                if (anim != null)
                {
                    anim.SetFloat("Speed_f", 0.0f);
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 11);
                    anim.SetBool("Reload_b", false);
                    anim.SetBool("Shoot_b", true);
                    anim.SetInteger("Animation_int", 0);
                    anim.Play("Bow_Shoot");
                }
                break;
            }
        }

    public void DoAttack()
    {
        switch (attackType)
        {
            case AttackType.MELEE_ONEHANDED:
                // Do nothing
                break;
            case AttackType.RANGED_BOW:
                if (anim != null)
                {
                    anim.SetFloat("Speed_f", 0.0f);
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 11);
                    anim.SetBool("Shoot_b", false);
                    anim.SetBool("Reload_b", true);
                    anim.SetInteger("Animation_int", 0);
                }

                GameObject newProjectile = Instantiate(projectilePrefab, this.gameObject.transform.position, Quaternion.identity);
                newProjectile.transform.Translate(new Vector3(0.0f, this.gameObject.GetComponent<BoxCollider>().size.y * 0.5f, 0.0f));
                
                newProjectile.transform.LookAt(targetObj.transform);

                Projectile projectile = newProjectile.GetComponent<Projectile>();
                projectile.srcObject = this.gameObject;
                Rigidbody rigidBody = newProjectile.GetComponent<Rigidbody>();
                rigidBody.useGravity = false;
                rigidBody.AddForce(newProjectile.transform.forward * 2000.0f);
                newProjectile.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
                break;
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
