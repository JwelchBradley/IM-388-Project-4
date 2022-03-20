/*****************************************************************************
// File Name :         EyeInteractable.cs
// Author :            Jacob Welch
// Creation Date :     13 November 2021
//
// Brief Description : A default implementation of the eye interactable
*****************************************************************************/

public class EyeInteractable : Interactable
{
    /// <summary>
    /// Handles the picking up of the eye.
    /// </summary>
    public override void Interact()
    {
        if(pc.CurrentActive.Equals(PlayerController.activeController.PERSON))
        {
            pc.ResetEye();
        }
    }
}
