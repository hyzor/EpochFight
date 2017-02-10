using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryTask : BaseTask {
    private GameObject baseTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    GameObject FindClosestBase(GameObject src)
    {
        Base[] bases = GameObject.FindObjectsOfType<Base>();
        Base closestBase = null;
        float closestBaseDist = -1;

        foreach (Base baseObj in bases)
        {
            float dist = Vector3.Distance(src.transform.position, baseObj.transform.position);

            if (closestBaseDist == -1 || dist < closestBaseDist)
            {
                closestBaseDist = dist;
                closestBase = baseObj;
            }
        }

        return closestBase.gameObject;
    }

    void SetBaseTarget(GameObject target)
    {

    }
}
