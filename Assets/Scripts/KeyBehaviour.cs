using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehaviour : Interactable
{
    public override void Interact()
    {
        foreach (PlayerController.activeController ac in displayTextControllers)
        {
            if (ac.Equals(pc.CurrentActive))
            {
                pc.Keys.Add(gameObject);
                gameObject.SetActive(false);
                break;
            }
        }
    }
}
