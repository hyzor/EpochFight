using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCollider : MonoBehaviour
{
    public enum ObjTypes
    {
        UNIT = 0,
        UNIT_ENEMY = 1,
        BASE = 2,
        BASE_ENEMY = 3,
        BUILDING = 4,
        BUILDING_ENEMY = 5,
        UNKNOWN = 6
    };

    public Dictionary<ObjTypes, GameObject> objectsInSight = new Dictionary<ObjTypes, GameObject>();

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
       DecideObjTypeAndAdd(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        DecideObjTypeAndAdd(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        DecideObjTypeAndRemove(other.gameObject);
    }

    private ObjTypes GetKeyFromObj(GameObject obj)
    {
        if (obj.GetComponent<Unit>() != null)
        {
            // Is the unit an enemy?
            if (obj.GetComponent<Enemy>() != null)
                return ObjTypes.UNIT_ENEMY;
            else
                return ObjTypes.UNIT;
        }
        else if (obj.GetComponent<Base>() != null)
        {
            if (obj.GetComponent<Enemy>() != null)
                return ObjTypes.BASE_ENEMY;
            else
                return ObjTypes.BASE;
        }

        return ObjTypes.UNKNOWN;
    }

    private void DecideObjTypeAndAdd(GameObject obj)
    {
        ObjTypes key = GetKeyFromObj(obj);

        if (!objectsInSight.ContainsKey(key))
        {
            objectsInSight.Add(key, obj);
        }
    }

    private void DecideObjTypeAndRemove(GameObject obj)
    {
        if (objectsInSight.ContainsValue(obj))
        {
            ObjTypes key = GetKeyFromObj(obj);
            objectsInSight.Remove(key);
        }
    }
}
