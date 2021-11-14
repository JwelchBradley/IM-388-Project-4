using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverBehaviour : Interactable
{
    [SerializeField]
    [Tooltip("The door that this pressure plate opens")]
    private DoorBehaviour door;

    private Animator anim;

    private bool activated = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void Interact()
    {
        activated = !activated;

        //anim.SetBool("Active", activated);

        int change = -1;

        if (activated)
        {
            change = 1;
        }

        door.ChangeState(change);
    }
}
