using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractable : Interactable
{
    public override void Interact()
    {
        Destroy(pc.TPM.Hand, 0.01f);
        pc.TPM = null;
        pc.HandCam = null;
    }
}