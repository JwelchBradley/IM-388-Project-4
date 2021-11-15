/*****************************************************************************
// File Name :         IInteractable.cs
// Author :            Jacob Welch
// Creation Date :     13 November 2021
//
// Brief Description : An interface for interactable objects.
*****************************************************************************/
using TMPro;
using UnityEngine;

public interface IInteractable
{
    public void DisplayInteractText();

    public void Interact();
}

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField]
    [Tooltip("The text that will be displayed when this object can be interacted with")]
    protected string displayText = "Press F to pickup Hand";

    protected TextMeshProUGUI text;

    protected PlayerController pc;

    [SerializeField]
    protected PlayerController.activeController[] displayTextControllers;

    private void Awake()
    {
        text = GameObject.Find("Pickup Text").GetComponent<TextMeshProUGUI>();

        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public virtual void DisplayInteractText()
    {
        foreach(PlayerController.activeController ac in displayTextControllers)
        {
            if(ac.Equals(pc.CurrentActive))
            {
                text.text = displayText;
                break;
            }
        }
    }

    public virtual void Interact()
    {
        throw new System.NotImplementedException();
    }
}