using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class AttackScript : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public enum AttackType
    {
        MELEE_ONEHANDED = 1,
        RANGED_BOW = 2
    }

    public float duration = 1.0f;
    public float cooldown = 0.0f;
    public float range = 1.0f;
    public int damage = 1;
    public AttackType attackType = AttackType.MELEE_ONEHANDED;
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public GameObject weaponPrefab;
    public GameObject targetObj;
    private Animator anim;
    private Entity entity;

    public AudioClip attackSound;
    public List<AudioClip> hitSounds;

    // Use this for initialization
    void Start ()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();

        // Make sure that the attack range is at least as big as the NavMesh's stopping distance for melee units
        if (navMeshAgent != null && navMeshAgent.stoppingDistance > range && !isRanged)
            range = navMeshAgent.stoppingDistance;

        anim = this.gameObject.GetComponent<Animator>();
        entity = this.gameObject.GetComponent<Entity>();

        GameObject weaponInstance = Instantiate(weaponPrefab, this.gameObject.transform.position, Quaternion.identity);
        entity.NotifyNewGameObject(weaponInstance);

        switch (attackType)
        {
            case AttackType.MELEE_ONEHANDED:
                weaponInstance.transform.parent = this.transform.Find(
                    "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Right_jnt/Arm_Right_jnt/Forearm_Right_jnt/Hand_Right_jnt");
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.forward = this.gameObject.transform.forward;
                weaponInstance.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                break;
            case AttackType.RANGED_BOW:
                weaponInstance.transform.parent = this.transform.Find(
                    "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Left_jnt/Arm_Left_jnt/Forearm_Left_jnt/Hand_Left_jnt");
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.forward = this.gameObject.transform.forward;
                weaponInstance.transform.Translate(new Vector3(0.0f,
                    -(weaponInstance.GetComponent<Renderer>().bounds.size.y * 0.5f),
                    weaponInstance.GetComponent<Renderer>().bounds.size.z));
                break;
        }
    }

    public void BeginAttack()
    {
        switch (attackType)
        {
            case AttackType.MELEE_ONEHANDED:
                if (anim != null)
                {
                    anim.speed = 1.0f / duration;
                    //anim.Play("Melee_OneHanded", 0, 0.0f);
                    //anim.Play("Melee_OneHanded", 6, 0.0f);
                    anim.SetFloat("Speed_f", 0.0f);
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 12);
                    anim.SetInteger("Animation_int", 0);
                }
                break;
            case AttackType.RANGED_BOW:
                if (anim != null)
                {
                    anim.Play("BowShoot", 0, 0.0f);
                    anim.Play("Bow_Shoot", 6, 0.0f);
                    anim.SetFloat("Speed_f", 0.0f);
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 11);
                    anim.SetBool("Reload_b", false);
                    anim.SetBool("Shoot_b", true);
                    anim.SetInteger("Animation_int", 0);
                }
                break;
            }

        if (attackSound != null)
            SoundManager.instance.PlaySingleClip(attackSound);
    }

    public void DoAttack()
    {
        switch (attackType)
        {
            case AttackType.MELEE_ONEHANDED:
                // Do nothing
                SoundManager.instance.RandomizeSfx(hitSounds.ToArray());
                anim.SetInteger("MeleeType_int", 1);
                anim.SetInteger("WeaponType_int", 0);
                anim.SetInteger("Animation_int", 2);

                break;
            case AttackType.RANGED_BOW:
                if (anim != null)
                {
                    //anim.Play("Bow_Idle", 6, 0.0f);
                    //anim.SetBool("Shoot_b", false);
                }

                GameObject newProjectile = Instantiate(projectilePrefab, this.gameObject.transform.position, Quaternion.identity);

                // Move projectile up by source object's half height
                newProjectile.transform.Translate(new Vector3(0.0f, this.gameObject.GetComponent<BoxCollider>().size.y * 0.5f, 0.0f));

                // Turn arrow towards target
                newProjectile.transform.LookAt(targetObj.transform);

                Projectile projectile = newProjectile.GetComponent<Projectile>();
                projectile.srcObject = this.gameObject;
                projectile.damage = damage;
                projectile.hitSounds = hitSounds.ToArray();

                Rigidbody rigidBody = newProjectile.GetComponent<Rigidbody>();
                rigidBody.useGravity = false;

                // Add force to arrow in its forward direction
                rigidBody.AddForce(newProjectile.transform.forward * 100.0f);

                // Correct arrow rotation
                newProjectile.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
                break;
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
