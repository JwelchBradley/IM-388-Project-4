using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The pause menu script.
    /// </summary>
    private PauseMenuBehavior pmb;

    /// <summary>
    /// The player movement script on this player.
    /// </summary>
    private PlayerMovement pm;

    /// <summary>
    /// The movement for the third person controller.
    /// </summary>
    private ThirdPersonMovement tpm;

    /// <summary>
    /// Holds true if the hand is active.
    /// </summary>
    private bool handActive = false;

    /// <summary>
    /// Holds true if the player can crouch.
    /// </summary>
    private bool canCrouch = true;
    #endregion

    #region Funcitons
    #region Initialize
    /// <summary>
    /// Initializes components.
    /// </summary>
    void Awake()
    {
        pmb = GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>();
        pm = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Input Calls
    public void OnPause()
    {
        pmb.PauseGame();
    }

    /// <summary>
    /// Calls for the player to crouch or uncrouch.
    /// </summary>
    public void OnCrouch()
    {
        if (!handActive)
        {
            pm.Crouch();
        }
    }

    /// <summary>
    /// Makes the player jump.
    /// </summary>
    public void OnJump()
    {
        pm.Jump();
    }

    /// <summary>
    /// Calls for the player to be moved.
    /// </summary>
    /// <param name="input">A vector 2 input direction.</param>
    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();

        if (handActive)
        {
            tpm.MovePlayer(inputVec);
        }
        else
        {
            pm.MovePlayer(inputVec);
        }
    }

    public void OnClick()
    {
        Debug.Log("Click");
    }
    #endregion
    #endregion
}
