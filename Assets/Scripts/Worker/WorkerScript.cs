using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    private CanvasTextScript canvasTextScript;

    public struct Task
    {
        public int taskId;
        public Vector3 worldPos;

        public Task(int taskId, Vector3 worldPos)
        {
            this.taskId = taskId;
            this.worldPos = worldPos;
        }
    }
    
    public enum tasks
    {
        IDLE = 0,
        COLLECT = 1,
        CARRY = 2
    }

    public enum state
    {
        IDLE = 0,
        WORKING = 1,
        TRAVELING = 2
    }

    //public int curTaskAssigned;
    public int curState;
    bool isBusy;

    private Task curTaskAssigned;

    private Queue<Task> taskQueue;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        //curTaskAssigned = (int)tasks.IDLE;
        curTaskAssigned = new Task((int)tasks.IDLE, this.transform.position);

        curState = (int)state.IDLE;

		taskQueue = new Queue<Task>();
        textMesh = (TextMesh)GetComponentInChildren(typeof(TextMesh));
        isBusy = false;
        anim = GetComponent<Animator>();

        Transform canvasTextTrans = GameObject.Find("Canvas").transform.GetChild(0);
        canvasTextScript = canvasTextTrans.GetComponent<CanvasTextScript>();
        
        // Running animation
        anim.SetFloat("Speed_f", 1.0f);
        anim.SetInteger("MeleeType_int", 12);
        anim.SetInteger("WeaponType_int", 12);
    }
	
	// Update is called once per frame
	void Update () {
        // Status text should always face camera
        //textMesh.transform.LookAt(Camera.main.transform);
        textMesh.transform.rotation = Camera.main.transform.rotation;
        //textMesh.transform.Rotate(Vector3.up - new Vector3(0, 180, 0)); // Text is mirrored so flip it back

        // Automatic queueing if not busy
        if (!isBusy)
        {
            switch (curTaskAssigned.taskId)
            {
                case (int)tasks.IDLE:
                    //taskQueue.Enqueue((int)tasks.COLLECT);
                    taskQueue.Enqueue(new Task((int)tasks.COLLECT, resTarget.position));
                    break;
                case (int)tasks.COLLECT:
                    //taskQueue.Enqueue((int)tasks.CARRY);
                    taskQueue.Enqueue(new Task((int)tasks.CARRY, baseTarget.position));
                    break;
                case (int)tasks.CARRY:
                    //taskQueue.Enqueue((int)tasks.IDLE);
                    taskQueue.Enqueue(new Task((int)tasks.IDLE, this.transform.position));
                    break;
                default:
                    break;
            }
        }

        // Begin with new task if available
        if (curState == (int)state.IDLE && taskQueue.Count > 0)
        {
            Task task = taskQueue.Dequeue();
            BeginTask(ref task);
        }

        // Check if we have reached the tasks' destination
        if (HasReachedTarget() && curState != (int)state.WORKING)
            OnDestReached(ref curTaskAssigned);
    }

    void BeginTask(ref Task _task)
    {
        curTaskAssigned = _task;

        switch (_task.taskId)
        {
            case (int)tasks.IDLE:
                // Idle animation
                anim.SetFloat("Speed_f", 0.0f);
                anim.SetInteger("MeleeType_int", 1);
                anim.SetInteger("WeaponType_int", 1);

                curState = (int)state.IDLE;
                agent.SetDestination(_task.worldPos);
                break;
            case (int)tasks.COLLECT:
                // Running animation
                anim.SetFloat("Speed_f", 1.0f);
                anim.SetInteger("MeleeType_int", 12);
                anim.SetInteger("WeaponType_int", 12);

                curState = (int)state.TRAVELING;
                agent.SetDestination(_task.worldPos);
                break;
            case (int)tasks.CARRY:
                // Running animation
                anim.SetFloat("Speed_f", 1.0f);
                anim.SetInteger("MeleeType_int", 12);
                anim.SetInteger("WeaponType_int", 12);

                curState = (int)state.TRAVELING;
                agent.SetDestination(_task.worldPos);
                break;
            default:
                break;
        }

		UpdateStatusText();
        isBusy = true;
    }

    void UpdateStatusText()
    {
        switch (curTaskAssigned.taskId)
        {
            case (int)tasks.IDLE:
                textMesh.text = "Idle";
                break;
            case (int)tasks.COLLECT:
                if (curState == (int)state.TRAVELING)
                    textMesh.text = "[Collect] Traveling to " + curTaskAssigned.worldPos;
                else
                    textMesh.text = "[Collect] Working... (" + numResources + ")";

                break;
            case (int)tasks.CARRY:
                textMesh.text = "Carrying (" + numResources + ") to " + curTaskAssigned.worldPos;
                break;
            default:
                break;
        }
    }

    void OnDestReached(ref Task _task)
    {
		if (_task.taskId == (int)tasks.COLLECT)
        {
            curState = (int)state.WORKING;
            UpdateStatusText();

            // Swinging animation
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);

            StartCoroutine(CollectResource());
        }
        else if (_task.taskId == (int)tasks.CARRY)
        {
            curState = (int)state.WORKING;

            canvasTextScript.IncrementResource(numResources);
            numResources = 0;

            curState = (int)state.IDLE;
            isBusy = false;
        }
        else if (_task.taskId == (int)tasks.IDLE)
        {
            curState = (int)state.IDLE;
            isBusy = false;
        }
    }

    IEnumerator CollectResource()
    {
        numResources = 0;

        do
        {
            yield return new WaitForSeconds(collectingTime);
            numResources++;
            UpdateStatusText();
        } while (numResources < maxResources);

        curState = (int)state.IDLE;
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
