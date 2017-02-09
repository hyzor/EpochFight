using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WorkerScript : MonoBehaviour {

    private NavMeshAgent agent;
    public float speed;
    public Transform resTarget;
    public Transform baseTarget;
    private int numResources;
    private TextMesh textMesh;

    private Queue<int> taskQueue;

    private GameObject UI_resText;

    private Animator anim;
    int idleHash = Animator.StringToHash("Idles.Idle_CrossedArms");
    int walkHash = Animator.StringToHash("Movement.Walk");
    int meleeOneHandedHash = Animator.StringToHash("Movement.Melee_OneHanded");

    private const int maxResources = 5;

    private CanvasTextScript canvasTextScript;
    private GameObject canvasTextObj;

    public enum task
    {
        IDLE = 0,
        COLLECT = 1,
        CARRY = 2
    }

    public int curTask;
    bool isBusy;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        curTask = (int)task.IDLE;
		taskQueue = new Queue<int>();
        textMesh = (TextMesh)GetComponentInChildren(typeof(TextMesh));
        isBusy = false;
        anim = GetComponent<Animator>();

        Transform canvasTextTrans = GameObject.Find("Canvas").transform.GetChild(0);
        canvasTextScript = canvasTextTrans.GetComponent<CanvasTextScript>();
    }
	
	// Update is called once per frame
	void Update () {

        // Begin with new task if available
        if (!isBusy && taskQueue.Count > 0)
            BeginTask(taskQueue.Dequeue());

        // Check if we have reached the tasks' destination
        if (HasReachedTarget())
            OnDestReached(curTask);

        // Automatic queueing if not busy
        if (!isBusy)
        {
            switch(curTask)
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
    }

    void BeginTask(int _task)
    {
        curTask = _task;

        switch (_task)
        {
            case (int)task.IDLE:
				agent.SetDestination(this.transform.position);
                break;
            case (int)task.COLLECT:
                agent.SetDestination(resTarget.position);
                break;
            case (int)task.CARRY:
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
        switch (curTask)
        {
            case (int)task.IDLE:
                textMesh.text = "Idle";
                break;
            case (int)task.COLLECT:
                textMesh.text = "Collecting " + numResources;
                break;
            case (int)task.CARRY:
                textMesh.text = "Carrying " + numResources + " resources";
                break;
            default:
                break;
        }
    }

    void OnDestReached(int _task)
    {

		if (_task == (int)task.COLLECT)
        {
            CollectResource();
        }
        else if (_task == (int)task.CARRY)
        {
            canvasTextScript.IncrementResource(numResources);
            numResources = 0;
        }

        isBusy = false;
    }

    void CollectResource()
    {
        numResources = 0;

        do
        {
            numResources++;
            Thread.Sleep(100);
        } while (numResources < maxResources);
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
