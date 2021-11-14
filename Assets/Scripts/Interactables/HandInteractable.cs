using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractable : Interactable
{
    PlayerController pc;

    // Start is called before the first frame update
    private void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        Destroy(pc.TPM.Hand, 0.01f);
        pc.TPM = null;
        pc.HandCam = null;
    }
}
