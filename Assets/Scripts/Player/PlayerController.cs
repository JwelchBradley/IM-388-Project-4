/*****************************************************************************
// File Name :         PlayerController.cs
// Author :            Jacob Welch
// Creation Date :     3 November 2021
//
// Brief Description : Handles the inputs of the player and changing between
                       different controllers.
*****************************************************************************/
using Cinemachine;
using TMPro;
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
    public enum activeController { HAND, PERSON, EYE };

    /// <summary>
    /// The currently active controller;
    /// </summary>
    private activeController currentActive = activeController.PERSON;

    public activeController CurrentActive
    {
        get => currentActive;
    }

    #region Controllers
    /// <summary>
    /// The player movement script on this player.
    /// </summary>
    private PlayerMovement pm;

    /// <summary>
    /// The movement for the third person controller.
    /// </summary>
    private ThirdPersonMovement tpm;

    public ThirdPersonMovement TPM
    {
        get => tpm;
        set
        {
            tpm = value;
        }
    }

    /// <summary>
    /// The current active eyecontroller.
    /// </summary>
    private EyeController ec;

    public EyeController EC
    {
        get => ec;
        set
        {
            ec = value;
        }
    }

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

    public CinemachineFreeLook HandCam
    {
        set
        {
            handCam = value;
        }
    }

    /// <summary>
    /// The virtual camera for the eye.
    /// </summary>
    private CinemachineVirtualCamera eyeCam;

    public CinemachineVirtualCamera EyeCam
    {
        set
        {
            eyeCam = value;
        }
    }

    /// <summary>
    /// The cinemachine brain on the main camera.
    /// </summary>
    private CinemachineBrain mainCamBrain;

    /// <summary>
    /// The main camera in the scene.
    /// </summary>
    private Camera mainCam;
    #endregion

    #region FPS Visuals
    [Header("FPS Visuals")]
    [Tooltip("The mesh of the fps player")]
    [SerializeField]
    private GameObject fpsMesh;
    #endregion

    #region Radial Menu
    private GameObject radialMenuPanel;

    private GameObject radialMenu;


    private bool currentRadial = false;

    private bool canOpenRadial = false;

    public bool CanOpenRadial
    {
        set
        {
            canOpenRadial = value;
        }
    }

    public bool Current
    {
        get => currentRadial;
    }
    #endregion

    #region Pickup
    [Header("Pickup")]
    [SerializeField]
    private float maxDist = 10;
    [SerializeField]
    private LayerMask pickUpSurface;
    [SerializeField]
    private LayerMask interactableMask;
    private TextMeshProUGUI pickUpText;
    [SerializeField]
    private LayerMask wallCheckMask;
    private bool canPickUp = false;
    private IInteractable interactable;
    public IInteractable InteractableObject
    {
        get => interactable;
    }

    private RaycastHit hit;
    #endregion

    #region Crosshair
    private GameObject crosshair;
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
        mainCam = Camera.main;
        mainCamBrain = mainCam.GetComponent<CinemachineBrain>();

        // Gets the hand if the scenes starts with it
        GameObject hand = GameObject.Find("Third Person Player");
        InitializeHand(hand);

        // Sets the cursor state
        Invoke("InitializeCursor", 0.1f);
        Cursor.visible = false;

        InitializeRadialMenu();

        pickUpText = GameObject.Find("Pickup Text").GetComponent<TextMeshProUGUI>();

        crosshair = GameObject.Find("Crosshair");
    }

    private void InitializeRadialMenu()
    {
        radialMenuPanel = GameObject.Find("Radial Menu Panel");
        radialMenu = GameObject.Find("Radial Menu");
        radialMenuPanel.SetActive(false);
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
        fpsMesh.SetActive(true);
        pm.MovePlayer(Vector2.zero, false);
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

        switch (currentActive)
        {
            case activeController.PERSON:
                pm.MovePlayer(inputVec, true);
                break;

            case activeController.HAND:
                tpm.MovePlayer(inputVec);
                break;

            case activeController.EYE:
                break;
        }
    }
    #endregion
    #endregion

    #region Radial Menu
    public void OnOpenMenu()
    {
        if (canOpenRadial && Time.timeScale != 0)
        {
            currentRadial = !radialMenuPanel.activeInHierarchy;
            radialMenuPanel.SetActive(currentRadial);
            Cursor.visible = currentRadial;

            if (currentRadial)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    #endregion

    #region Pickup
    private void FixedUpdate()
    {
        DisplayPickupText();
    }

    private void DisplayPickupText()
    {
        if (!mainCamBrain.IsBlending)
        {
            bool changed = canPickUp;
            GameObject past = null;
            if(hit.transform != null)
            {
                past = hit.transform.gameObject;
            }
            LayerMask currentMask = pickUpSurface;

            if(currentActive != activeController.PERSON)
            {
                currentMask = interactableMask;
            }

            canPickUp = Physics.BoxCast(mainCam.transform.position, Vector3.one * 3, mainCam.transform.forward, out hit, mainCam.transform.rotation, maxDist, currentMask);

            if (!canPickUp)
            {
                canPickUp = Physics.BoxCast(mainCam.transform.position, Vector3.one * 1f, mainCam.transform.forward, out hit, mainCam.transform.rotation, maxDist, currentMask);
            }

            if (!canPickUp)
            {
                canPickUp = Physics.BoxCast(mainCam.transform.position, Vector3.one * 0.5f, mainCam.transform.forward, out hit, mainCam.transform.rotation, maxDist, currentMask);
            }

            if (canPickUp)
            {
                RaycastHit hitTemp;
                Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hitTemp, maxDist, wallCheckMask);

                if(Vector3.Distance(hitTemp.point, mainCam.transform.position) < Vector3.Distance(hit.point, mainCam.transform.position))
                {
                    canPickUp = false;
                }
            }

            if (canPickUp)
            {
                if (hit.transform != null && past != null && past.Equals(hit.transform.gameObject))
                {
                    changed = false;
                }
            }

            if (canPickUp)
            {
                if (!changed && hit.transform.gameObject.name!="Player")
                {
                    interactable = hit.transform.gameObject.GetComponent<IInteractable>();
                    interactable.DisplayInteractText();
                }
                //pickUpText.text = "Press F to pickup " + hit.transform.gameObject.tag;
            }
            else
            {
                interactable = null;
                pickUpText.text = "";
            }
        }
        else
        {
            interactable = null;
            canPickUp = false;
            pickUpText.text = "";
        }
    }

    public void OnResetEye()
    {
        if(interactable != null && !radialMenu.activeInHierarchy)
        {
            interactable.Interact();
        }
        /*
        if (canPickUp && ec != null)
        {
            Destroy(ec.Eye);
            ec = null;
            eyeCam = null;
        }*/
    }

    public void OnResetHand()
    {
        /*
        if(canPickUp && tpm != null)
        {
            Destroy(tpm.Hand);
            tpm = null;
            handCam = null;
        }*/
    }
    #endregion

    #region Body
    private void OnBody()
    {
        crosshair.SetActive(true);
        switch (currentActive)
        {
            case activeController.HAND:
                UpdateHandCam(0, CinemachineBrain.UpdateMethod.LateUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.PERSON);
                fpsMesh.SetActive(false);
                tpm.MovePlayer(Vector2.zero);
                break;
            case activeController.EYE:
                eyeCam.Priority = 0;
                currentActive = activeController.PERSON;
                mainCamBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
                mainCamBrain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
                fpsMesh.SetActive(false);
                break;
        }
    }
    #endregion

    #region Hand
    /// <summary>
    /// Handles changes to and from the hand.
    /// </summary>
    private void OnHand()
    {
        switch (currentActive)
        {
            // To the hand from the person
            case activeController.PERSON:
                ToHandFromPerson();
                break;

                /*
            // To the person from the hand
            case activeController.HAND:
                UpdateHandCam(0, CinemachineBrain.UpdateMethod.LateUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.PERSON);
                fpsMesh.SetActive(false);
                tpm.MovePlayer(Vector2.zero);
                break;
                */
            // To the hand from the eye
            case activeController.EYE:
                if (tpm != null)
                {

                        UpdateHandCam(100, CinemachineBrain.UpdateMethod.LateUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
                    //UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
                    eyeCam.Priority = 0;
                    pm.MovePlayer(Vector2.zero, false);
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

        fpsMesh.SetActive(true);

        crosshair.SetActive(false);

        UpdateHandCam(100, CinemachineBrain.UpdateMethod.LateUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
        //UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
        pm.MovePlayer(Vector2.zero, false);
    }

    private void UpdateHandCam(int priority, CinemachineBrain.UpdateMethod camUpdateMethod, CinemachineBrain.BrainUpdateMethod camBlendUpdate, activeController newActive)
    {
        handCam.Priority = priority;
        mainCamBrain.m_UpdateMethod = camUpdateMethod;
        mainCamBrain.m_BlendUpdateMethod = camBlendUpdate;
        currentActive = newActive;
    }
    #endregion

    #region Eye
    /// <summary>
    /// Changes to and from the eye.
    /// </summary>
    private void OnEye()
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
                    crosshair.SetActive(false);
                    fpsMesh.SetActive(true);
                    pm.MovePlayer(Vector2.zero, false);
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
                /*
            // Changes from the eye to the player
            case activeController.EYE:
                eyeCam.Priority = 0;
                currentActive = activeController.PERSON;
                mainCamBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
                mainCamBrain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
                fpsMesh.SetActive(false);
                break;*/
        }
    }

    /// <summary>
    /// Stops the casting funciton for the eye.
    /// </summary>
    private void NoLongerCastingEye()
    {
        eCaster.IsCasting = !eCaster.IsCasting;
        crosshair.SetActive(!crosshair.activeInHierarchy);
    }

    /// <summary>
    /// Handles the looking of the eye.
    /// </summary>
    /// <param name="input"></param>
    public void OnMouseLook(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();

        if (currentActive.Equals(activeController.EYE) && !mainCamBrain.IsBlending && !radialMenu.activeInHierarchy)
        {
            ec.Look(inputVec);
        }
    }

    /// <summary>
    /// Handles the spawning of the eye when the mouse is clicked and the playing is casting.
    /// </summary>
    public void OnClick()
    {
        if (eCaster.IsCasting && eCaster.CanCast && Time.timeScale != 0 && !radialMenuPanel.activeInHierarchy)
        {
            eCaster.IsCasting = false;
            GameObject eye = eCaster.SpawnEye();
            crosshair.SetActive(false);
            InitializeEye(eye);
        }
        else if (radialMenuPanel.activeInHierarchy && Time.timeScale != 0)
        {
            switch (radialMenu.GetComponent<RadialMenuController>().Im.sprite.name)
            {
                case "RadialMenuNewAtlas_5":
                    OnBody();
                    break;
                case "RadialMenuNewAtlas_6":
                    OnEye();
                    break;
                case "RadialMenuNewAtlas_7":
                    OnHand();
                    break;
            }

            Invoke("OnOpenMenu", 0.05f);
            //OnOpenMenu();
        }
    }
    #endregion
    #endregion
    #endregion
}
