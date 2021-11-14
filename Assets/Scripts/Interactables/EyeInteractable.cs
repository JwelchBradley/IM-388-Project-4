using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeInteractable : Interactable
{
    public override void Interact()
    {
        if(pc.CurrentActive.Equals(PlayerController.activeController.PERSON))
        {
            Destroy(pc.EC.Eye, 0.01f);
            pc.EC = null;
            pc.EyeCam = null;
        }
    }
}
