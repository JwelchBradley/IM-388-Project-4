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
    private string displayText = "Press F to pickup Hand";

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GameObject.Find("Pickup Text").GetComponent<TextMeshProUGUI>();
    }

    public void DisplayInteractText()
    {
        text.text = displayText;
    }

    public virtual void Interact()
    {
        throw new System.NotImplementedException();
    }
}