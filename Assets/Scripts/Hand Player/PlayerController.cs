using Cinemachine;
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
    /// List of controllers.
    /// </summary>
    private enum activeController { HAND, PERSON, EYE };

    /// <summary>
    /// The currently active controller;
    /// </summary>
    private activeController currentActive = activeController.PERSON;

    /// <summary>
    /// Holds true if the player can crouch.
    /// </summary>
    private bool canCrouch = true;

    /// <summary>
    /// The virtual camera for when the player is standing.
    /// </summary>
    private CinemachineFreeLook handCam;
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

        GameObject hand = GameObject.Find("Third Person Player");

        if(hand != null)
        {
            tpm = hand.GetComponentInChildren<ThirdPersonMovement>();
            handCam = hand.GetComponentInChildren<CinemachineFreeLook>();
            
            InitializeCamera();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializeCamera()
    {
        //handCam = hand.GetComponent<CinemachineVirtualCamera>();
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
        if (currentActive.Equals(activeController.PERSON))
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

        switch (currentActive)
        {
            case activeController.PERSON:
                pm.MovePlayer(inputVec);
                break;

            case activeController.HAND:
                tpm.MovePlayer(inputVec);
                break;

            case activeController.EYE:
                break;
        }
    }

    public void OnHand()
    {
        switch (currentActive)
        {
            case activeController.PERSON:
                currentActive = activeController.HAND;
                handCam.Priority = 100;
                pm.MovePlayer(Vector2.zero);
                break;

            case activeController.HAND:
                currentActive = activeController.PERSON;
                handCam.Priority = 0;
                tpm.MovePlayer(Vector2.zero);
                break;

            case activeController.EYE:
                break;
        }
    }

    public void OnClick()
    {
        Debug.Log("Click");
    }
    #endregion
    #endregion
}
