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

    /// <summary>
    /// The cinemachine brain on the main camera.
    /// </summary>
    private CinemachineBrain mainCamBrain;
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

        mainCamBrain = Camera.main.GetComponent<CinemachineBrain>();

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
        switch (currentActive)
        {
            case activeController.PERSON:
                pm.Crouch();
                break;

            case activeController.HAND:
                break;

            case activeController.EYE:
                break;
        }
    }

    /// <summary>
    /// Makes the player jump.
    /// </summary>
    public void OnJump()
    {
        switch (currentActive)
        {
            case activeController.PERSON:
                pm.Jump();
                break;

            case activeController.HAND:
                tpm.Jump();
                break;

            case activeController.EYE:
                break;
        }
    }

    /// <summary>
    /// Calls for the player to be moved.
    /// </summary>
    /// <param name="input">A vector 2 input direction.</param>
    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();

        if (!mainCamBrain.IsBlending)
        {
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
    }

    public void OnHand()
    {
        switch (currentActive)
        {
            case activeController.PERSON:
                UpdateCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, activeController.HAND);
                pm.MovePlayer(Vector2.zero);
                break;

            case activeController.HAND:
                UpdateCam(0, CinemachineBrain.UpdateMethod.SmartUpdate, activeController.PERSON);
                tpm.MovePlayer(Vector2.zero);
                break;

            case activeController.EYE:
                break;
        }
    }

    private void UpdateCam(int priority, CinemachineBrain.UpdateMethod camUpdateMethod, activeController newActive)
    {
        handCam.Priority = priority;
        mainCamBrain.m_UpdateMethod = camUpdateMethod;
        currentActive = newActive;
    }

    public void OnClick()
    {
        Debug.Log("Click");
    }
    #endregion
    #endregion
}
