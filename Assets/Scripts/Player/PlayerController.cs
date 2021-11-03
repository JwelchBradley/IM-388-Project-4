/*****************************************************************************
// File Name :         PlayerController.cs
// Author :            Jacob Welch
// Creation Date :     3 November 2021
//
// Brief Description : Handles the inputs of the player and changing between
                       different controllers.
*****************************************************************************/
using Cinemachine;
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

    /// <summary>
    /// Sets the cursor to be locked to the center of the screen.
    /// </summary>
    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Gets everything necessary for the hand to function.
    /// </summary>
    /// <param name="hand">The hand gameobject.</param>
    private void InitializeHand(GameObject hand)
    {
        if (hand != null)
        {
            tpm = hand.GetComponentInChildren<ThirdPersonMovement>();
            handCam = hand.GetComponentInChildren<CinemachineFreeLook>();
        }
    }

    /// <summary>
    /// Initializes the eye when it is first placed.
    /// </summary>
    /// <param name="eye"></param>
    private void InitializeEye(GameObject eye)
    {
        ec = eye.GetComponentInChildren<EyeController>();
        eyeCam = eye.GetComponentInChildren<CinemachineVirtualCamera>();
        pm.MovePlayer(Vector2.zero);
        eyeCam.Priority = 100;
        currentActive = activeController.EYE;
    }
    #endregion

    #region Input Calls
    #region Basic Calls
    #region Pause
    /// <summary>
    /// Pauses the game.
    /// </summary>
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
    /// <summary>
    /// Handles changes to and from the hand.
    /// </summary>
    public void OnHand()
    {
        switch (currentActive)
        {
            // To the hand from the person
            case activeController.PERSON:
                ToHandFromPerson();
                break;

            // To the person from the hand
            case activeController.HAND:
                UpdateHandCam(0, CinemachineBrain.UpdateMethod.SmartUpdate, activeController.PERSON);
                tpm.MovePlayer(Vector2.zero);
                break;

            // To the hand from the eye
            case activeController.EYE:
                if (tpm != null)
                {
                    UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, activeController.HAND);
                    eyeCam.Priority = 0;
                    pm.MovePlayer(Vector2.zero);
                }
                break;
        }
    }

    /// <summary>
    /// Handles the event of changing to the hand from the person.
    /// </summary>
    private void ToHandFromPerson()
    {
        if (tpm == null)
        {
            GameObject hand = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Third Person Player/Third Person Player", typeof(GameObject)), transform.position + Camera.main.transform.forward * 2, transform.rotation);
            InitializeHand(hand);
        }

        if (ec == null && eCaster.IsCasting)
        {
            NoLongerCastingEye();
        }

        UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, activeController.HAND);
        pm.MovePlayer(Vector2.zero);
    }

    private void UpdateHandCam(int priority, CinemachineBrain.UpdateMethod camUpdateMethod, activeController newActive)
    {
        handCam.Priority = priority;
        mainCamBrain.m_UpdateMethod = camUpdateMethod;
        currentActive = newActive;
    }
    #endregion

    #region Eye
    /// <summary>
    /// Changes to and from the eye.
    /// </summary>
    public void OnEye()
    {
        switch (currentActive)
        {
            // Pulls out the eye to be place or changes from the person to the eye
            case activeController.PERSON:
                if(ec == null)
                {
                    NoLongerCastingEye();
                }
                else
                {
                    pm.MovePlayer(Vector2.zero);
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                }
                break;

            // Changes to the eye from the hand
            case activeController.HAND:
                if(ec != null)
                {
                    tpm.MovePlayer(Vector2.zero);
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                }
                break;

            // Changes from the eye to the player
            case activeController.EYE:
                eyeCam.Priority = 0;
                currentActive = activeController.PERSON;
                break;
        }
    }

    /// <summary>
    /// Stops the casting funciton for the eye.
    /// </summary>
    private void NoLongerCastingEye()
    {
        eCaster.IsCasting = !eCaster.IsCasting;
    }

    /// <summary>
    /// Handles the looking of the eye.
    /// </summary>
    /// <param name="input"></param>
    public void OnMouseLook(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();

        if (currentActive.Equals(activeController.EYE))
        {
            ec.Look(inputVec);
        }
    }

    /// <summary>
    /// Handles the spawning of the eye when the mouse is clicked and the playing is casting.
    /// </summary>
    public void OnClick()
    {
        if (eCaster.IsCasting && eCaster.CanCast && Time.timeScale != 0)
        {
            Debug.Log(true);
            eCaster.IsCasting = false;
            GameObject eye = eCaster.SpawnEye();
            InitializeEye(eye);
        }
    }
    #endregion
    #endregion
    #endregion
}
