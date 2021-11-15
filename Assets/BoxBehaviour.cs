using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBehaviour : Interactable
{
    Rigidbody rb;
    GameObject player;
    private Vector3 offset;
    [SerializeField]
    private float pullDist = 1;
    private bool canPull = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(player.transform.position, transform.position) < pullDist)
        {
            canPull = true;
        }
        else
        {
            canPull = false;
        }
    }

    public override void DisplayInteractText()
    {
        if (canPull)
        {
            base.DisplayInteractText();
        }
    }

    public override void Interact()
    {
        offset = transform.position - player.transform.position;
        StartCoroutine(PullBox());
    }

    private IEnumerator PullBox()
    {
        while (true)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                StopAllCoroutines();
            }

            //rb.velocity = transform.position;

            yield return new WaitForEndOfFrame();
        }
    }
}
