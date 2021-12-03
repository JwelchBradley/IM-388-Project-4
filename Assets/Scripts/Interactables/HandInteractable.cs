using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractable : Interactable
{
    public override void Interact()
    {
        Destroy(pc.TPM.Hand.transform.parent.gameObject, 0.01f);
        pc.TPM = null;
        pc.HandCam = null;
        pc.HandMesh.SetActive(true);
        pc.RightHandArmMesh.SetActive(true);
        pc.RemovePickupBodyPartReminder();
    }
}
