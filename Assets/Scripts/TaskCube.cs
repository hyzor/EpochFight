using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCube : MonoBehaviour
{
    public Renderer renderer;
    public bool isColliding = false;
    private bool isStay = false;

    private List<Collider> colliders = new List<Collider>();

    public GameObject attachedTo;
    public Renderer attachmentRenderer;
    public SkinnedMeshRenderer attachmentSkinRenderer;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    private void FixedUpdate()
    {
        if (isStay)
        {
            isColliding = true;
            isStay = false;
        }
        else
        {
            isColliding = false;
        }

        if (isColliding)
        {
            renderer.material.color = Color.red;

            if (attachmentRenderer != null)
                attachmentRenderer.material.color = Color.red;
            else if (attachmentSkinRenderer != null)
                attachmentSkinRenderer.material.color = Color.red;
        }
        else
        {
            renderer.material.color = Color.green;

            if (attachmentRenderer != null)
                attachmentRenderer.material.color = Color.white;
            else if (attachmentSkinRenderer != null)
                attachmentSkinRenderer.material.color = Color.white;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Ground>() == null 
            && other.gameObject.GetComponent<Entity>() != null
            && other.gameObject != attachedTo)
            isStay = true;
    }
}
