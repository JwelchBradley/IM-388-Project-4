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
    /// <summary>
    /// Displays the text for the interaction.
    /// </summary>
    public void DisplayInteractText();

    /// <summary>
    /// Does the interaction with this object.
    /// </summary>
    public void Interact();
}

/*****************************************************************************
// File Name :         Interactable.cs
// Author :            Jacob Welch
// Creation Date :     13 November 2021
//
// Brief Description : A default implementation of interactable objects.
*****************************************************************************/
public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField]
    [Tooltip("The text that will be displayed when this object can be interacted with")]
    protected string displayText = "Press F to pickup Hand";

    /// <summary>
    /// The pick up text in the scene.
    /// </summary>
    protected TextMeshProUGUI text;

    protected PlayerController pc;

    [SerializeField]
    protected PlayerController.activeController[] displayTextControllers;

    protected virtual void Awake()
    {
        text = GameObject.Find("Pickup Text").GetComponent<TextMeshProUGUI>();

        
        GameObject player = GameObject.Find("Player");

        if(player != null)
        pc = player.GetComponent<PlayerController>();

        //pc = FindObjectOfType<PlayerController>();
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