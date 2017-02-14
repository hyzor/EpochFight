using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Worker : MonoBehaviour {
    public int resourceCapacity = 5;
    public int resPerCollect = 1;
    public int numResources = 0;
    private Entity entity;

    // Use this for initialization
    void Start ()
    {
        entity = this.gameObject.GetComponent<Entity>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (numResources > 0)
        {
            entity.AppendStatusText(" [" + numResources + "/" + resourceCapacity + "]");
        }
    }
}
