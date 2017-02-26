using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class TaskBarScript : MonoBehaviour
{
    // Necro skin control attributes
    private float leafOffset;
    private float frameOffset;
    private float skullOffset;

    private float RibbonOffsetX;
    private float FrameOffsetX;
    private float SkullOffsetX;
    private float RibbonOffsetY;
    private float FrameOffsetY;
    private float SkullOffsetY;

    private float WSwaxOffsetX;
    private float WSwaxOffsetY;
    private float WSribbonOffsetX;
    private float WSribbonOffsetY;

    private float spikeCount;

    // This script will only work with the Necromancer skin
    public GUISkin mySkin;

    // Task bar window
    public static float taskBarHeight = 95.0f;
    public static float taskBarMarginX = 50.0f;
    public static float taskBarWidth = 400.0f;
    private Rect windowTaskBar = new Rect(0.0f, 0.0f, taskBarWidth, taskBarHeight);

    // Task bar window top bar
    private static float windowBarWidthOversize = 15.0f;
    private static float windowBarWidth = taskBarWidth + windowBarWidthOversize;
    private static float windowBarHeight = 35.0f;
    private Rect windowTaskBarTopBar = new Rect(0.0f, 0.0f, windowBarWidth, windowBarHeight);

    // Custom skin attributes
    private Vector2 scrollPosition;
    private float HroizSliderValue = 0.5f;
    private float VertSliderValue = 0.5f;
    private bool ToggleBTN = false;

    // Buttons
    public static float btnWidth = 80.0f;
    public static float btnHeight = 80.0f;
    public static float btnPadding = 10.0f;

    private Rect btnRect = new Rect(btnWidth * 0.5f, (taskBarHeight * 0.5f) - (btnHeight * 0.5f), btnWidth, btnHeight);

    public static int numTasks = 5;
    public static int numBtns = 4;

    public Dictionary<GameObject, GUIContent> btns = new Dictionary<GameObject, GUIContent>();

    private bool btnPressed = false;

    public GameObject taskObj;
    public GameObject taskPrefab;
    public GameObject taskCubePrefab;
    private GameObject taskCubeInstance;
    private Collider taskObjCollider;
    private BoxCollider taskObjBoxCollider;

    private int objLayerCache;

    [Serializable]
    public struct Task
    {
        public GUIContent btnContent;
        public GameObject taskObj;
    }

    [Serializable]
    public struct TaskButton
    {
        public Rect rect;
        public Vector2 origPos;
        public bool isPressed;
        public GUIContent btnContent;
        public GameObject taskObj;
        public Task task;
    }

    public TaskButton[] taskBtns = new TaskButton[numBtns];
    public Task[] tasks = new Task[numTasks];

    private void OnValidate()
    {
        if (taskBtns.Length != numBtns)
        {
            Array.Resize(ref taskBtns, numTasks);
        }

        if (tasks.Length != numTasks)
        {
            Array.Resize(ref tasks, numBtns);
        }
    }

    public void AddSpikes(float winX)
    {
        spikeCount = Mathf.Floor(winX - 152) / 22;
        GUILayout.BeginHorizontal();
        GUILayout.Label("", "SpikeLeft");

        for (int i = 0; i < spikeCount; i++)
        {
            GUILayout.Label("", "SpikeMid");
        }

        GUILayout.Label("", "SpikeRight");
        GUILayout.EndHorizontal();
    }

    public void FancyTop(float topX)
    {
        leafOffset = (topX / 2) - 64;
        frameOffset = (topX / 2) - 27;
        skullOffset = (topX / 2) - 20;
        GUI.Label(new Rect(leafOffset, 18, 0, 0), "", "GoldLeaf");
        GUI.Label(new Rect(frameOffset, 3, 0, 0), "", "IconFrame");	
        GUI.Label(new Rect(skullOffset, 12, 0, 0), "", "Skull");
    }

    public void WaxSeal(float x, float y)
    {
        WSwaxOffsetX = x - 120;
        WSwaxOffsetY = y - 115;
        WSribbonOffsetX = x - 114;
        WSribbonOffsetY = y - 83;

        GUI.Label(new Rect(WSribbonOffsetX, WSribbonOffsetY, 0, 0), "", "RibbonBlue");
        GUI.Label(new Rect(WSwaxOffsetX, WSwaxOffsetY, 0, 0), "", "WaxSeal");
    }

    public void DeathBadge(int x, int y)
    {
        RibbonOffsetX = x;
        FrameOffsetX = x + 3;
        SkullOffsetX = x + 10;
        RibbonOffsetY = y + 22;
        FrameOffsetY = y;
        SkullOffsetY = y + 9;

        GUI.Label(new Rect(RibbonOffsetX, RibbonOffsetY, 0, 0), "", "RibbonRed");
        GUI.Label(new Rect(FrameOffsetX, FrameOffsetY, 0, 0), "", "IconFrame");	
        GUI.Label(new Rect(SkullOffsetX, SkullOffsetY, 0, 0), "", "Skull");	
    }

    public void DoTaskBar(int windowID)
    {
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();
    }

    void Start()
    {
        windowTaskBar.x = (Screen.width * 0.5f) - (taskBarWidth * 0.5f);
        windowTaskBar.y = Screen.height - (taskBarHeight);

        for (int i = 0; i < taskBtns.Length; ++i)
        {
            taskBtns[i].rect = btnRect;
            taskBtns[i].rect.x = windowTaskBar.x + (btnWidth * 0.25f) + (btnWidth * i) + (btnPadding * i);
            taskBtns[i].rect.y = windowTaskBar.y + (btnHeight * 0.25f);
            taskBtns[i].btnContent = tasks[i].btnContent;
            taskBtns[i].taskObj = tasks[i].taskObj;
        }
    }

    private void Update()
    {
        if (btnPressed)
        {
            // Now ray cast from the mouse pos to world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                taskObj.transform.position = hit.point;
                //taskCubeInstance.transform.position = hit.point;

                GameObject hitObj = hit.transform.gameObject;
                Debug.Log("Gui hit " + hitObj.name);
            }
            
            taskCubeInstance.transform.position = taskObj.transform.position;

            if (taskObjBoxCollider != null)
                taskCubeInstance.transform.Translate(taskObjBoxCollider.center * 0.5f);
        }
    }

    void OnDrag(GameObject obj)
    {
        MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            component.enabled = false;
        }

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.enabled = true;
            renderer.material.color = Color.gray;
            return;
        }

        SkinnedMeshRenderer skinRenderer = obj.GetComponent<SkinnedMeshRenderer>();

        if (skinRenderer != null)
        {
            skinRenderer.enabled = true;
            skinRenderer.material.color = Color.gray;
            return;
        }
    }

    void OnDrop(GameObject obj)
    {
        MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            component.enabled = true;
        }

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = Color.white;
            return;
        }

        SkinnedMeshRenderer skinRenderer = obj.GetComponent<SkinnedMeshRenderer>();

        if (skinRenderer != null)
        {
            skinRenderer.material.color = Color.white;
            return;
        }
    } 

    void OnGUI()
    {
        GUI.skin = mySkin;
        Event ev = Event.current;

        // Draw task bar window
        windowTaskBar.x = (Screen.width * 0.5f) - (taskBarWidth * 0.5f);
        windowTaskBar.y = Screen.height - (taskBarHeight);
        windowTaskBar = GUI.Window(0, windowTaskBar, DoTaskBar, "", GUIStyle.none);
        GUI.Box(windowTaskBar, "", new GUIStyle("window"));

        // Draw task bar top bar
        windowTaskBarTopBar.x = windowTaskBar.x - (windowBarWidthOversize * 0.5f);
        windowTaskBarTopBar.y = windowTaskBar.y - windowBarHeight;
        GUI.Label(windowTaskBarTopBar, "", "WindowBar");

        if (btnPressed)
        {
            // Invalid position
            if (windowTaskBar.Contains(ev.mousePosition))
            {
                taskObj.SetActive(false);
            }

            // Invalid position
            else if (windowTaskBarTopBar.Contains(ev.mousePosition))
            {
                taskObj.SetActive(false);
            }

            // Valid position
            else
            {
                taskObj.SetActive(true);
            }
        }

        // Update bar task "buttons"
        for (int i = 0; i < taskBtns.Length; ++i)
        {
            taskBtns[i].origPos.x = windowTaskBar.x + (btnWidth * 0.25f) + (btnWidth * i) + (btnPadding * i);
            taskBtns[i].origPos.y = windowTaskBar.y + (btnHeight * 0.25f);

            if (taskBtns[i].rect.Contains(ev.mousePosition))
            {
                if (ev.type == EventType.MouseDown)
                {
                    if (!taskBtns[i].isPressed)
                    {
                        taskObj = Instantiate(taskBtns[i].taskObj);
                        taskCubeInstance = Instantiate(taskCubePrefab);
                        taskCubeInstance.transform.SetParent(taskObj.transform);

                        taskObjCollider = taskObj.GetComponent<Collider>();
                        taskObjBoxCollider = taskObj.GetComponent<BoxCollider>();

                        if (taskObjBoxCollider != null)
                            taskCubeInstance.transform.localScale = taskObjCollider.bounds.size;

                        TaskCube taskCube = taskCubeInstance.GetComponent<TaskCube>();
                        taskCube.GetComponent<Renderer>().enabled = false;

                        taskCube.attachedTo = taskObj;
                        taskCube.attachmentRenderer = taskObj.GetComponent<Renderer>();
                        taskCube.attachmentSkinRenderer = taskObj.GetComponent<SkinnedMeshRenderer>();

                        objLayerCache = taskObj.layer;
                        taskObj.layer = 2; // Ignore raycast layer
                        taskObj.SetActive(true);
                        OnDrag(taskObj);
                    }

                    taskBtns[i].isPressed = true;
                    taskBtns[i].rect.width = btnWidth - 25.0f;
                    taskBtns[i].rect.height = btnHeight - 25.0f;
                    //taskBtns[i].rect.x += (25.0f * 0.5f);
                    //taskBtns[i].rect.y += (25.0f * 0.5f);

                    taskBtns[i].rect.position = new Vector2(
                        ev.mousePosition.x - (taskBtns[i].rect.width * 0.5f),
                        ev.mousePosition.y - (taskBtns[i].rect.height * 0.5f));
                }
            }

            if (taskBtns[i].isPressed)
                btnPressed = true;

            if (taskBtns[i].isPressed && ev.type == EventType.MouseUp)
            {
                btnPressed = false;
                taskBtns[i].isPressed = false;
                taskBtns[i].rect.width = btnWidth;
                taskBtns[i].rect.height = btnHeight;

                taskBtns[i].rect.x = taskBtns[i].origPos.x;
                taskBtns[i].rect.y = taskBtns[i].origPos.y;

                if (taskObj != null)
                {
                    OnDrop(taskObj);

                    TaskCube taskCube = taskCubeInstance.GetComponent<TaskCube>();

                    if (taskCube.isColliding)
                        Destroy(taskObj);
                    else if (taskObj.activeSelf == false)
                        Destroy(taskObj);

                    Destroy(taskCubeInstance);
                    taskObj.layer = objLayerCache;
                }
                //Destroy(taskObj);
            }

            else if (taskBtns[i].isPressed && ev.type == EventType.MouseDrag)
            {
                //taskBtns[i].rect.x += ev.delta.x;
                //taskBtns[i].rect.y += ev.delta.y;

                // Transform from GUI space to ScreenPoint space (top-down to bottom-up)
                taskBtns[i].rect.position = new Vector2(
                    ev.mousePosition.x - (taskBtns[i].rect.width * 0.5f),
                    ev.mousePosition.y - (taskBtns[i].rect.height * 0.5f));
            }

            else if (!taskBtns[i].isPressed)
            {
                taskBtns[i].rect.x = windowTaskBar.x + (btnWidth * 0.25f) + (btnWidth * i) + (btnPadding * i);
                taskBtns[i].rect.y = windowTaskBar.y + (btnHeight * 0.25f);
            }

            GUI.Button(taskBtns[i].rect, taskBtns[i].btnContent);
        }
    }
}