using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IClickable {
    public enum State
    {
        IDLE = 0,
        WORKING = 1,
        TRAVELING = 2,
        FIGHTING = 3
    }

    public int health = 1;
    public float speed = 1.0f;

    private UnitTaskManager taskMgr;
    private NavMeshAgent navMeshAgent;
    private TextMesh textMesh;
    private State curState;

	// Use this for initialization
	void Start () {
        taskMgr = GetComponent<UnitTaskManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        textMesh = (TextMesh)GetComponentInChildren(typeof(TextMesh));
        curState = State.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnLeftClick()
    {
        Debug.Log("Unit left clicked!");
    }

    public void OnRightClick()
    {
        Debug.Log("Unit right clicked!");
    }
}
