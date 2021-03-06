using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBehaviour : Interactable
{
    Rigidbody rb;
    GameObject player;
    private PlayerMovement pm;
    private Vector3 offset;
    [SerializeField]
    private float pullDist = 1;
    private bool canPull = false;
    private IInteractable interactable;

    [SerializeField]
    private float maxPushSpeed = 5;

    [SerializeField]
    private float pullSpeed = 20;

    private bool isPulling = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        pm = player.GetComponent<PlayerMovement>();
        interactable = GetComponent<Interactable>();
    }

    private void FixedUpdate()
    {
        if (!isPulling)
        {
            ClampVelocity();
        }
        canPull = Vector3.Distance(player.transform.position, transform.position) < pullDist;
    }

    private void ClampVelocity()
    {
        if(rb.velocity.sqrMagnitude > 64)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 8);
        }
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
        pm.pullMod = 0.5f;
        isPulling = true;

        while ((Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Mouse0)) && canPull)
        {
            Vector3 velocity = ((pm.PullPos.transform.position - transform.position).normalized);
            velocity.y = 0;

            float dist = Vector3.Distance(pm.PullPos.transform.position, transform.position) * 0.5f;
            float pullTempSpeed = Mathf.Lerp(0, pullSpeed, dist);
            velocity *= pullTempSpeed;
            //velocity = Vector3.Lerp(Vector3.zero, ((pm.PullPos.transform.position - transform.position).normalized) * pullSpeed, 1);

            rb.velocity = velocity;

            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        isPulling = false;
        pm.pullMod = 1;
        //MoveBoxForward();
    }
}
