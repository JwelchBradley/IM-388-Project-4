using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractable : Interactable
{
    public override void Interact()
    {
        pc.ResetHand();
    }
}
