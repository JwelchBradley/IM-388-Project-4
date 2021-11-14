using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeInteractable : Interactable
{
    PlayerController pc;

    // Start is called before the first frame update
    private void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public override void Interact()
    {
        Destroy(pc.EC.Eye, 0.01f);
        pc.EC = null;
        pc.EyeCam = null;
    }
}
