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
        MELEE_TWOHANDED = 2,
        RANGED_BOW = 3
    }

    public enum OffHandType
    {
        NONE = 0,
        SHIELD = 1
    }

    public float duration = 1.0f;
    public float cooldown = 0.0f;
    public float range = 1.0f;
    public int damage = 1;
    public AttackType attackType = AttackType.MELEE_ONEHANDED;
    public OffHandType offHandType = OffHandType.NONE;

    public bool isRanged = false;
    public GameObject projectilePrefab;
    public GameObject weaponPrefab;
    public GameObject offHandPrefab;
    public GameObject targetObj;

    private Animator anim;
    private Entity entity;
    private Unit unit;

    public List<AudioClip> attackSounds = new List<AudioClip>();
    public List<AudioClip> hitSounds = new List<AudioClip>();

    private GameObject weaponInstance = null;
    private GameObject offHandInstance = null;

    private Renderer weaponInstanceRenderer = null;
    
    private Vector3 weaponSheathPos;
    private Vector3 weaponSheathRot;

    private Vector3 weaponAttackPos;
    private Vector3 weaponAttackRot;

    // Use this for initialization
    void Start()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();

        // Make sure that the attack range is at least as big as the NavMesh's stopping distance for melee units
        if (navMeshAgent != null && navMeshAgent.stoppingDistance > range && !isRanged)
            range = navMeshAgent.stoppingDistance;

        anim = this.gameObject.GetComponent<Animator>();
        entity = this.gameObject.GetComponent<Entity>();
        unit = this.gameObject.GetComponent<Unit>();

        if (weaponPrefab != null)
        {
            weaponInstance = Instantiate(weaponPrefab, this.gameObject.transform.position, Quaternion.identity);
            weaponInstanceRenderer = weaponInstance.GetComponent<Renderer>();
            entity.NotifyNewGameObject(weaponInstance);
        }


        if (offHandPrefab != null)
        {
            offHandInstance = Instantiate(offHandPrefab, this.gameObject.transform.position, Quaternion.identity);
            entity.NotifyNewGameObject(offHandInstance);
        }

        switch (attackType)
        {
            case AttackType.MELEE_ONEHANDED:

                if (weaponInstance != null)
                {
                    weaponInstance.transform.parent = this.transform.Find(
                    "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Right_jnt/Arm_Right_jnt/Forearm_Right_jnt/Hand_Right_jnt");
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localScale = weaponInstance.transform.parent.localScale;
                    weaponInstance.transform.forward = this.gameObject.transform.forward;
                    weaponInstance.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                }

                if (offHandInstance != null)
                {
                    if (offHandType == OffHandType.SHIELD)
                    {
                        offHandInstance.transform.parent = this.transform.Find(
                        "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Left_jnt/Arm_Left_jnt/Forearm_Left_jnt/Hand_Left_jnt");
                        offHandInstance.transform.localPosition = Vector3.zero;
                        offHandInstance.transform.localScale = offHandInstance.transform.parent.localScale;
                        offHandInstance.transform.forward = this.gameObject.transform.forward;
                        offHandInstance.transform.Rotate(new Vector3(-65.0f, 0.0f, 0.0f));
                        offHandInstance.transform.Translate(new Vector3(0.0f, 0.0f, 0.45f));
                    }
                }

                break;
            case AttackType.MELEE_TWOHANDED:
                if (weaponInstance != null)
                {
                    weaponInstance.transform.parent = this.transform.Find(
                    "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Right_jnt/Arm_Right_jnt/Forearm_Right_jnt/Hand_Right_jnt");
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localScale = weaponInstance.transform.parent.localScale;
                    weaponInstance.transform.forward = this.gameObject.transform.forward;
                    weaponInstance.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                }

                break;
            case AttackType.RANGED_BOW:

                if (weaponInstance != null)
                {
                    weaponInstance.transform.parent = this.transform.Find(
                    "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Left_jnt/Arm_Left_jnt/Forearm_Left_jnt/Hand_Left_jnt");
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localScale = weaponInstance.transform.parent.localScale;
                    weaponInstance.transform.forward = this.gameObject.transform.forward;

                    // Calcuate weapon sheath position
                    weaponSheathPos = new Vector3(0.0f, 0.0f, weaponInstanceRenderer.bounds.size.y * 0.5f);
                    weaponSheathRot = new Vector3(-90.0f, 0.0f, 0.0f);

                    // Calcuate weapon attack position
                    weaponAttackPos = new Vector3(0.0f,
                    -(weaponInstanceRenderer.bounds.size.y * 0.5f),
                    weaponInstanceRenderer.bounds.size.z);
                    weaponAttackRot = Vector3.zero;

                    // Weapon starts at sheath position
                    weaponInstance.transform.Translate(weaponSheathPos);
                    weaponInstance.transform.Rotate(weaponSheathRot);
                }

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
                    if (attackSounds.Count > 0)
                        SoundManager.instance.RandomizeSfx(attackSounds.ToArray());

                    anim.speed = 1.0f / duration;
                    anim.SetFloat("Speed_f", 0.0f);
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 12);
                    anim.SetInteger("Animation_int", 0);
                }
                break;
            case AttackType.MELEE_TWOHANDED:
                if (anim != null)
                {
                    if (attackSounds.Count > 0)
                        SoundManager.instance.RandomizeSfx(attackSounds.ToArray());

                    anim.speed = 1.0f / duration;
                    anim.SetFloat("Speed_f", 0.0f);
                    anim.SetInteger("MeleeType_int", 2);
                    anim.SetInteger("WeaponType_int", 12);
                    anim.SetInteger("Animation_int", 0);
                }
                break;
            case AttackType.RANGED_BOW:
                if (anim != null)
                {
                    if (weaponInstance != null)
                    {
                        weaponInstance.transform.Rotate(-weaponSheathRot);
                        weaponInstance.transform.Translate(-weaponSheathPos);

                        weaponInstance.transform.Translate(weaponAttackPos);
                        weaponInstance.transform.Rotate(weaponAttackRot);
                    }

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
    }

    public void DoAttack()
    {
        switch (attackType)
        {
            case AttackType.MELEE_ONEHANDED:

                if (hitSounds.Count > 0)
                    SoundManager.instance.RandomizeSfx(hitSounds.ToArray());

                anim.SetInteger("MeleeType_int", 1);
                anim.SetInteger("WeaponType_int", 0);
                anim.SetInteger("Animation_int", 2);

                break;
            case AttackType.MELEE_TWOHANDED:

                if (hitSounds.Count > 0)
                    SoundManager.instance.RandomizeSfx(hitSounds.ToArray());

                anim.SetInteger("MeleeType_int", 2);
                anim.SetInteger("WeaponType_int", 0);
                anim.SetInteger("Animation_int", 2);

                break;
            case AttackType.RANGED_BOW:
                if (anim != null)
                {
                    //anim.Play("Bow_Idle", 6, 0.0f);
                    //anim.SetBool("Shoot_b", false);
                }

                if (weaponInstance != null)
                {
                    weaponInstance.transform.Rotate(-weaponAttackRot);
                    weaponInstance.transform.Translate(-weaponAttackPos);

                    weaponInstance.transform.Translate(weaponSheathPos);
                    weaponInstance.transform.Rotate(weaponSheathRot);
                }

                if (attackSounds.Count > 0)
                    SoundManager.instance.RandomizeSfx(attackSounds.ToArray());

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
        //weaponInstance.transform.localPosition = weaponSheathPos;
        //weaponInstance.transform.rotation = weaponSheathRot;
    }
}
