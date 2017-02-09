using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WorkerScript : MonoBehaviour {
    private const int maxResources = 5;
    private const int collectingTime = 3; // In seconds

    public float speed;
    private int numResources;
    public Transform resTarget;
    public Transform baseTarget;

    private NavMeshAgent agent;
    private TextMesh textMesh;
    private Animator anim;
    private CanvasTextScript canvasTextScript;

    private Queue<int> taskQueue;
    
    public enum task
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

    public int curTaskAssigned;
    public int curState;
    bool isBusy;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        curTaskAssigned = (int)task.IDLE;
        curState = (int)state.IDLE;

		taskQueue = new Queue<int>();
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
        textMesh.transform.LookAt(Camera.main.transform);
        textMesh.transform.Rotate(Vector3.up - new Vector3(0, 180, 0)); // Text is mirrored so flip it back

        // Automatic queueing if not busy
        if (!isBusy)
        {
            switch (curTaskAssigned)
            {
                case (int)task.IDLE:
                    taskQueue.Enqueue((int)task.COLLECT);
                    break;
                case (int)task.COLLECT:
                    taskQueue.Enqueue((int)task.CARRY);
                    break;
                case (int)task.CARRY:
                    taskQueue.Enqueue((int)task.IDLE);
                    break;
                default:
                    break;
            }
        }

        // Begin with new task if available
        if (curState == (int)state.IDLE && taskQueue.Count > 0)
            BeginTask(taskQueue.Dequeue());

        // Check if we have reached the tasks' destination
        if (HasReachedTarget() && curState != (int)state.WORKING)
            OnDestReached(curTaskAssigned);
    }

    void BeginTask(int _task)
    {
        curTaskAssigned = _task;

        switch (_task)
        {
            case (int)task.IDLE:
                // Idle animation
                anim.SetFloat("Speed_f", 0.0f);
                anim.SetInteger("MeleeType_int", 1);
                anim.SetInteger("WeaponType_int", 1);

                curState = (int)state.IDLE;
                agent.SetDestination(this.transform.position);
                break;
            case (int)task.COLLECT:
                // Running animation
                anim.SetFloat("Speed_f", 1.0f);
                anim.SetInteger("MeleeType_int", 12);
                anim.SetInteger("WeaponType_int", 12);

                curState = (int)state.TRAVELING;
                agent.SetDestination(resTarget.position);
                break;
            case (int)task.CARRY:
                // Running animation
                anim.SetFloat("Speed_f", 1.0f);
                anim.SetInteger("MeleeType_int", 12);
                anim.SetInteger("WeaponType_int", 12);

                curState = (int)state.TRAVELING;
                agent.SetDestination(baseTarget.position);
                break;
            default:
                break;
        }

		UpdateStatusText();
        isBusy = true;
    }

    void UpdateStatusText()
    {
        switch (curTaskAssigned)
        {
            case (int)task.IDLE:
                textMesh.text = "Idle";
                break;
            case (int)task.COLLECT:
                if (curState == (int)state.TRAVELING)
                    textMesh.text = "Traveling to collect";
                else
                    textMesh.text = "Collecting (" + numResources + ")";

                break;
            case (int)task.CARRY:
                textMesh.text = "Carrying (" + numResources + ")";
                break;
            default:
                break;
        }
    }

    void OnDestReached(int _task)
    {
		if (_task == (int)task.COLLECT)
        {
            curState = (int)state.WORKING;
            UpdateStatusText();

            // Swinging animation
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);

            StartCoroutine(CollectResource());
        }
        else if (_task == (int)task.CARRY)
        {
            curState = (int)state.WORKING;

            canvasTextScript.IncrementResource(numResources);
            numResources = 0;

            curState = (int)state.IDLE;
            isBusy = false;
        }
        else if (_task == (int)task.IDLE)
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
            numResources++;
        } while (numResources < maxResources);

        yield return new WaitForSeconds(3);

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
