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
    private IInteractable interactable;

    [SerializeField]
    private float pullSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        interactable = GetComponent<Interactable>();
    }

    private void FixedUpdate()
    {
        canPull = Vector3.Distance(player.transform.position, transform.position) < pullDist;
    }

    public override void DisplayInteractText()
    {
        //base.DisplayInteractText();
        //StartCoroutine(DisplayInteractTextHelper());
        if (canPull)
        {
            base.DisplayInteractText();
        }
        else
        {
            text.text = "";
        }
    }

    public override void Interact()
    {
        offset = transform.position - player.transform.position;
        StartCoroutine(PullBox());

    }

    private IEnumerator PullBox()
    {
        while ((Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Mouse0)) && canPull)
        {
            Vector3 velocity = (player.transform.position - transform.position).normalized * pullSpeed;
            velocity.y = 0;
            rb.velocity = velocity;

            yield return null;
        }

        rb.velocity = Vector3.zero;
    }
}
