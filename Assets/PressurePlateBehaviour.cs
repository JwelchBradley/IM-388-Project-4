using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateBehaviour : MonoBehaviour
{
    BoxCollider bc;
    [SerializeField]
    private LayerMask playerMask;

    private Animator anim;

    private RaycastHit hit;

    private bool heldDown = false;

    public bool HeldDown
    {
        get => heldDown;
    }

    // Start is called before the first frame update
    void Awake()
    {
        bc = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        heldDown = Physics.BoxCast(transform.position - transform.up.normalized, bc.bounds.extents / 2, transform.up, out hit, Quaternion.identity, 2, playerMask);

        if (heldDown && !hit.transform.gameObject.CompareTag("Hand"))
        {
            anim.SetBool("Pressed", true);
        }
        else
        {
            anim.SetBool("Pressed", false);
        }
    }
}
