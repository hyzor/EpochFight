using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class WorkerScript : MonoBehaviour {
    private const int maxResources = 5;
    private const int collectingTime = 1; // In seconds

    public float speed;
    private int numResources;
    public Transform resTarget;
    public Transform baseTarget;

    private NavMeshAgent agent;
    private TextMesh textMesh;
    private Animator anim;
    private GameObject resourceCanvasElement;

    private UnitTaskManager taskMgr;

    //public int curTaskAssigned;
    public int curState;
    bool isBusy;

    private BaseTask curTaskAssigned;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();

        //curState = (int)state.IDLE;
        
        textMesh = (TextMesh)GetComponentInChildren(typeof(TextMesh));
        isBusy = false;
        anim = GetComponent<Animator>();

        Transform canvasResourceTextTrans = GameObject.Find("Canvas").transform.GetChild(0);
        resourceCanvasElement = canvasResourceTextTrans.gameObject;
        
        // Running animation
        anim.SetFloat("Speed_f", 1.0f);
        anim.SetInteger("MeleeType_int", 12);
        anim.SetInteger("WeaponType_int", 12);

        taskMgr = GetComponent<UnitTaskManager>();
    }
	
	// Update is called once per frame
	void Update () {
        // Status text should always face camera
        textMesh.transform.rotation = Camera.main.transform.rotation;

        //ExecuteEvents.Execute<ITaskMessageHandler>(taskMgr, null, (x, y) => x.OnClick(x, y, z));

        // Begin with new task if available
        //if (curState == (int)state.IDLE && !isBusy)
        //{
            //Task task = taskQueue.Dequeue();
            //BaseTask task = taskMgr.GetFirstTask();

            //if (task != null)
                //BeginTask(ref task);
        //}

        // Check if we have reached the tasks' destination
        if (curTaskAssigned != null)
        {
            //if (HasReachedTarget() && curState != (int)state.WORKING)
                //OnDestReached(ref curTaskAssigned);
        }
    }

    /*void BeginTask(ref BaseTask _task)
    {
        curTaskAssigned = _task;

        switch (_task.taskTypeId)
        {
            case (int)BaseTask.tasks.IDLE:
                // Idle animation
                anim.SetFloat("Speed_f", 0.0f);
                anim.SetInteger("MeleeType_int", 1);
                anim.SetInteger("WeaponType_int", 1);

                curState = (int)state.IDLE;
                agent.SetDestination(_task.goTo);
                break;
            case (int)BaseTask.tasks.COLLECT:
                // Running animation
                anim.SetFloat("Speed_f", 1.0f);
                anim.SetInteger("MeleeType_int", 12);
                anim.SetInteger("WeaponType_int", 12);

                curState = (int)state.TRAVELING;
                agent.SetDestination(_task.goTo);
                break;
            case (int)BaseTask.tasks.CARRY:
                // Running animation
                anim.SetFloat("Speed_f", 1.0f);
                anim.SetInteger("MeleeType_int", 12);
                anim.SetInteger("WeaponType_int", 12);

                curState = (int)state.TRAVELING;
                agent.SetDestination(_task.goTo);
                break;
            default:
                break;
        }

		UpdateStatusText();
        isBusy = true;
    }*/

    /*void UpdateStatusText()
    {
        switch (curTaskAssigned.taskTypeId)
        {
            case (int)BaseTask.tasks.IDLE:
                textMesh.text = "Idle";
                break;
            case (int)BaseTask.tasks.COLLECT:
                if (curState == (int)state.TRAVELING)
                    textMesh.text = "[Collect] Traveling to " + curTaskAssigned.goTo;
                else
                    textMesh.text = "[Collect] (" + numResources + ") Working...";

                break;
            case (int)BaseTask.tasks.CARRY:
                if (curState == (int)state.TRAVELING)
                    textMesh.text = "[Carry] (" + numResources + ") Traveling to " + curTaskAssigned.goTo;
                else
                    textMesh.text = "[Carry] (" + numResources + ") Working...";
                break;
            default:
                break;
        }
    }

    void OnDestReached(ref BaseTask _task)
    {
		if (_task.taskTypeId == (int)BaseTask.tasks.COLLECT)
        {
            curState = (int)state.WORKING;
            UpdateStatusText();

            // Swinging animation
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);

            StartCoroutine(CollectResource());
        }
        else if (_task.taskTypeId == (int)BaseTask.tasks.CARRY)
        {
            curState = (int)state.WORKING;
            
            // Send increment value event to the resource canvas element
            ExecuteEvents.Execute<ICanvasMessageHandler>(resourceCanvasElement, null, (x, y) => x.IncrementComponentValue(numResources));
            numResources = 0;

            curState = (int)state.IDLE;
            isBusy = false;
        }
        else if (_task.taskTypeId == (int)BaseTask.tasks.IDLE)  
        {
            curState = (int)state.IDLE;
            isBusy = false;
        }
    }
    */

    IEnumerator CollectResource()
    {
        numResources = 0;

        do
        {
            yield return new WaitForSeconds(collectingTime);
            numResources++;
            //UpdateStatusText();
        } while (numResources < maxResources);

        //curState = (int)state.IDLE;
        isBusy = false;
    }

    bool HasReachedTarget()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0.0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
