using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarInteractable : Interactable
{
    /// <summary>
    /// Handles the picking up of the eye.
    /// </summary>
    public override void Interact()
    {
        if (pc.CurrentActive.Equals(PlayerController.activeController.PERSON))
        {
            pc.ResetEar();
        }
    }
}
