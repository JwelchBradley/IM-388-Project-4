using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyChecker : Interactable
{
    [SerializeField]
    private GameObject key;

    [SerializeField]
    private GameObject[] activatableObjects;

    [SerializeField]
    private string doesntHaveKeyText = "Need a key to open this door";

    private List<Activatable> activatables = new List<Activatable>();

    private bool hasActivated = false;

    private void Start()
    {
        foreach (GameObject gm in activatableObjects)
        {
            activatables.Add(gm.GetComponentInChildren<Activatable>());
        }
    }

    public override void DisplayInteractText()
    {
        if (hasActivated)
        {
            text.text = "";
            return;
        }

        if (pc.Keys.Contains(key))
        {
            base.DisplayInteractText();
        }
        else
        {
            text.text = doesntHaveKeyText;
        }
    }

    public override void Interact()
    {
        foreach (PlayerController.activeController ac in displayTextControllers)
        {
            if (ac.Equals(pc.CurrentActive) && pc.Keys.Contains(key))
            {
                foreach (Activatable activatable in activatables)
                {
                    hasActivated = true;
                    activatable.ChangeObjectState();
                }
            }
        }
    }
}
