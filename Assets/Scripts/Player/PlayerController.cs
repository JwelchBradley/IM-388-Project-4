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

    #region Character Controllers
    /// <summary>
    /// List of controllers.
    /// </summary>
    private enum activeController { HAND, PERSON, EYE };

    /// <summary>
    /// The currently active controller;
    /// </summary>
    private activeController currentActive = activeController.PERSON;

    #region Controllers
    /// <summary>
    /// The player movement script on this player.
    /// </summary>
    private PlayerMovement pm;

    /// <summary>
    /// The movement for the third person controller.
    /// </summary>
    private ThirdPersonMovement tpm;

    /// <summary>
    /// The current active eyecontroller.
    /// </summary>
    private EyeController ec;

    /// <summary>
    /// The eye casting component.
    /// </summary>
    private EyeCaster eCaster;
    #endregion
    #endregion

    #region Cameras
    /// <summary>
    /// The virtual camera for when the hand.
    /// </summary>
    private CinemachineFreeLook handCam;

    /// <summary>
    /// The virtual camera for the eye.
    /// </summary>
    private CinemachineVirtualCamera eyeCam;

    /// <summary>
    /// The cinemachine brain on the main camera.
    /// </summary>
    private CinemachineBrain mainCamBrain;
    #endregion
    #endregion

    #region Funcitons
    #region Initialize
    /// <summary>
    /// Initializes components.
    /// </summary>
    void Awake()
    {
        // Gets components in the scene
        pmb = GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>();
        pm = GetComponent<PlayerMovement>();
        eCaster = GetComponent<EyeCaster>();
        mainCamBrain = Camera.main.GetComponent<CinemachineBrain>();

        // Gets the hand if the scenes starts with it
        GameObject hand = GameObject.Find("Third Person Player");
        InitializeHand(hand);

        // Sets the cursor state
        Invoke("InitializeCursor", 0.1f);
        Cursor.visible = false;
    }

    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Gets everything necessary for the hand to function.
    /// </summary>
    /// <param name="hand"></param>
    private void InitializeHand(GameObject hand)
    {
        if (hand != null)
        {
            tpm = hand.GetComponentInChildren<ThirdPersonMovement>();
            handCam = hand.GetComponentInChildren<CinemachineFreeLook>();

            InitializeCamera();
        }
    }

    private void InitializeEye(GameObject eye)
    {

    }

    private void InitializeCamera()
    {
        //handCam = hand.GetComponent<CinemachineVirtualCamera>();
    }
    #endregion

    #region Input Calls
    #region Basic Calls
    #region Pause
    public void OnPause()
    {
        pmb.PauseGame();
    }
    #endregion

    #region Basic Actions
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
    #endregion
    #endregion

    #region Hand
    public void OnHand()
    {
        switch (currentActive)
        {
            case activeController.PERSON:
                if(tpm == null)
                {
                    GameObject hand = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Third Person Player/Third Person Player", typeof(GameObject)), transform.position + Camera.main.transform.forward*2, transform.rotation);
                    InitializeHand(hand);
                }

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
    #endregion

    #region Eye
    public void OnEye()
    {
        switch (currentActive)
        {
            case activeController.PERSON:
                if(ec == null)
                {
                    eCaster.IsCasting = !eCaster.IsCasting;
                }
                else
                {
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                }
                break;

            case activeController.HAND:
                break;

            case activeController.EYE:
                eyeCam.Priority = 0;
                currentActive = activeController.PERSON;
                break;
        }
    }
    #endregion

    private void UpdateCam(int priority, CinemachineBrain.UpdateMethod camUpdateMethod, activeController newActive)
    {
        handCam.Priority = priority;
        mainCamBrain.m_UpdateMethod = camUpdateMethod;
        currentActive = newActive;
    }

    public void OnMouseLook(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();

        if (currentActive.Equals(activeController.EYE))
        {
            ec.Look(inputVec);
        }
    }

    public void OnClick()
    {
        if (eCaster.IsCasting && eCaster.CanCast)
        {
            eCaster.IsCasting = false;
            GameObject eye = eCaster.SpawnEye();
            ec = eye.GetComponentInChildren<EyeController>();
            eyeCam = eye.GetComponentInChildren<CinemachineVirtualCamera>();
            eyeCam.Priority = 100;
            currentActive = activeController.EYE;
        }
    }
    #endregion
    #endregion
}
