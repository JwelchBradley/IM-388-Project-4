/*****************************************************************************
// File Name :         PlayerController.cs
// Author :            Jacob Welch
// Creation Date :     3 November 2021
//
// Brief Description : Handles the inputs of the player and changing between
                       different controllers.
*****************************************************************************/
using Cinemachine;
using System.Collections;
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
    public enum activeController { HAND, PERSON, EYE, HEART };

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

    public EyeCaster ECaster
    {
        get => eCaster;
    }

    /// <summary>
    /// The current active heartcontroller.
    /// </summary>
    /*private HeartController hc;

    public HeartController HC
    {
        get => hc;
        set
        {
            hc = value;
        }
    }*/
    #endregion
    #endregion

    #region Cameras
    /// <summary>
    /// The virtual camera for when the player is standing.
    /// </summary>
    private CinemachineVirtualCamera walkCam;

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

    [SerializeField]
    private GameObject eyeImageRenderer;
    [SerializeField]
    private LayerMask eyeImageRendererMask;
    private LayerMask startingRendererMask;
    #endregion

    #region FPS Visuals
    [Header("FPS Visuals")]
    [Tooltip("The mesh of the fps player")]
    [SerializeField]
    private GameObject fpsMesh;

    [Tooltip("The mesh of the fps player eye")]
    [SerializeField]
    private GameObject eyeMesh;

    public GameObject EyeMesh
    {
        get => eyeMesh;
    }

    [Tooltip("The mesh of the fps player hand")]
    [SerializeField]
    private GameObject handMesh;

    public GameObject HandMesh
    {
        get => handMesh;
    }

    [Tooltip("The mesh of the fps player heart")]
    [SerializeField]
    private GameObject heartMesh;

    public GameObject HeartMesh
    {
        get => heartMesh;
    }

    [Tooltip("The mesh of the players arms")]
    [SerializeField]
    private GameObject armMesh;

    [SerializeField]
    private GameObject rightHandArmMesh;

    public GameObject RightHandArmMesh
    {
        get => rightHandArmMesh;
    }

    [Tooltip("The animator of the players arms")]
    [SerializeField]
    private Animator[] armAnim;

    public Animator[] ArmAnim
    {
        get => armAnim;
    }
    #endregion

    #region Radial Menu
    private GameObject radialMenuPanel;

    private GameObject radialMenu;


    private bool currentRadial = false;

    private bool canOpenRadial = true;

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
    private float maxHandDist = 20;
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
        walkCam = GameObject.Find("Walk vcam").GetComponent<CinemachineVirtualCamera>();
        mainCam = Camera.main;
        mainCamBrain = mainCam.GetComponent<CinemachineBrain>();
        startingRendererMask = mainCam.cullingMask;

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
            handMesh.SetActive(false);
            rightHandArmMesh.SetActive(false);
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
        StartCoroutine(SetEyeImageRenderer());
        eyeMesh.SetActive(false);
        armMesh.SetActive(false);
        ec = eye.GetComponentInChildren<EyeController>();
        eyeCam = eye.GetComponentInChildren<CinemachineVirtualCamera>();
        fpsMesh.SetActive(true);
        pm.MovePlayer(Vector2.zero, false);
        eyeCam.Priority = 100;
        currentActive = activeController.EYE;
        pmb.PickUpBodyPartReminder.SetActive(false);
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
        if (canOpenRadial && Time.timeScale != 0 && !pmb.Note.activeInHierarchy)
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
        if (!mainCamBrain.IsBlending && !eCaster.IsCasting)
        {
            bool changed = canPickUp;
            GameObject past = null;
            if(hit.transform != null)
            {
                past = hit.transform.gameObject;
            }
            LayerMask currentMask = pickUpSurface;
            float currentDist = maxDist;

            if(currentActive != activeController.PERSON)
            {
                currentMask = interactableMask;
                currentDist = maxHandDist;
            }

            canPickUp = Physics.BoxCast(mainCam.transform.position, Vector3.one * 3, mainCam.transform.forward, out hit, mainCam.transform.rotation, currentDist, currentMask);

            if (!canPickUp)
            {
                canPickUp = Physics.BoxCast(mainCam.transform.position, Vector3.one * 1f, mainCam.transform.forward, out hit, mainCam.transform.rotation, currentDist, currentMask);
            }

            if (!canPickUp)
            {
                canPickUp = Physics.BoxCast(mainCam.transform.position, Vector3.one * 0.5f, mainCam.transform.forward, out hit, mainCam.transform.rotation, currentDist, currentMask);
            }

            if (canPickUp)
            {
                RaycastHit hitTemp;
                Physics.Raycast(mainCam.transform.position, hit.point - mainCam.transform.position, out hitTemp, currentDist, wallCheckMask);

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
        else if (eCaster.IsCasting)
        {
            pickUpText.text = "Left click to cast eye";
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

        if(tpm != null)
        {
            tpm.OutlineScript.enabled = true;
        }

        if(ec != null)
        {
            ec.OutlineScript.enabled = true;
        }

        switch (currentActive)
        {
            case activeController.PERSON:
                eCaster.IsCasting = false;
                break;
            case activeController.HAND:
                UpdateHandCam(-5, CinemachineBrain.UpdateMethod.LateUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.PERSON);
                fpsMesh.SetActive(false);
                tpm.MovePlayer(Vector2.zero);
                tpm.SwitchCameras();

                pmb.PickUpBodyPartReminder.SetActive(true);
                break;
            case activeController.EYE:
                //eyeImageRenderer.SetActive(false);
                mainCam.cullingMask = startingRendererMask;
                eyeCam.Priority = -1;
                currentActive = activeController.PERSON;
                mainCamBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
                mainCamBrain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
                fpsMesh.SetActive(false);

                pmb.PickUpBodyPartReminder.SetActive(true);
                break;
        }

        StartCoroutine(EnableArms());
    }

    public void RemovePickupBodyPartReminder()
    {
        if(ec == null && tpm == null)
        pmb.PickUpBodyPartReminder.SetActive(false);
    }

    private IEnumerator EnableArms()
    {
        if (currentActive.Equals(activeController.PERSON))
        {
            if (walkCam.Priority == 50)
            {
                walkCam.Priority = 51;
            }
            else
            {
                walkCam.Priority = 50;
            }
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        while (currentActive.Equals(activeController.PERSON) && mainCamBrain.IsBlending)
        {
            yield return new WaitForFixedUpdate();
        }

        if (currentActive.Equals(activeController.PERSON))
        {
            armMesh.SetActive(true);
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

                pmb.PickUpBodyPartReminder.SetActive(false);
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
                    //eyeImageRenderer.SetActive(false);
                    mainCam.cullingMask = startingRendererMask;
                    tpm.SwitchCameras();
                    UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
                    //UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
                    eyeCam.Priority = -1;
                    pm.MovePlayer(Vector2.zero, false);
                    tpm.OutlineScript.enabled = false;
                }
                break;
        }

        armMesh.SetActive(false);
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
        else
        {
            tpm.SwitchCameras();
        }

        if (ec == null && eCaster.IsCasting)
        {
            NoLongerCastingEye();
        }

        fpsMesh.SetActive(true);

        crosshair.SetActive(false);

        UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
        //UpdateHandCam(100, CinemachineBrain.UpdateMethod.FixedUpdate, CinemachineBrain.BrainUpdateMethod.FixedUpdate, activeController.HAND);
        pm.MovePlayer(Vector2.zero, false);
        tpm.OutlineScript.enabled = false;
    }

    private void UpdateHandCam(int priority, CinemachineBrain.UpdateMethod camUpdateMethod, CinemachineBrain.BrainUpdateMethod camBlendUpdate, activeController newActive)
    {
        //handCam.Priority = priority;
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
                    StartCoroutine(SetEyeImageRenderer());
                    crosshair.SetActive(false);
                    fpsMesh.SetActive(true);
                    pm.MovePlayer(Vector2.zero, false);
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                    ec.OutlineScript.enabled = false;
                    armMesh.SetActive(false);

                    pmb.PickUpBodyPartReminder.SetActive(false);
                }
                break;

                
            // Changes to the eye from the hand
            case activeController.HAND:
                if(ec != null)
                {
                    StartCoroutine(SetEyeImageRenderer());
                    tpm.MovePlayer(Vector2.zero);
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                    tpm.SwitchCameras();
                    armMesh.SetActive(false);
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

    private IEnumerator SetEyeImageRenderer()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        while (mainCamBrain.IsBlending)
        {
            yield return new WaitForFixedUpdate();
        }

        if (currentActive.Equals(activeController.EYE))
        {
            mainCam.cullingMask = eyeImageRendererMask;
            //eyeImageRenderer.SetActive(true);
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
    #endregion

    #region Heart
    /// <summary>
    /// Changes to and from heart
    /// </summary>
    private void OnHeart()
    {
        switch (currentActive)
        {
            /*// Pulls out the heart to be place or changes from the person to the heart
            case activeController.PERSON:

                if (hc == null)
                {
                    //NoLongerCastingEye();
                }
                else
                {
                    StartCoroutine(SetEyeImageRenderer());
                    crosshair.SetActive(false);
                    fpsMesh.SetActive(true);
                    pm.MovePlayer(Vector2.zero, false);
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                    ec.OutlineScript.enabled = false;
                    armMesh.SetActive(false);

                    pmb.PickUpBodyPartReminder.SetActive(false);
                }
                break;


            // Changes to the heart from the hand
            case activeController.HAND:
                if (hc != null)
                {
                    StartCoroutine(SetEyeImageRenderer());
                    tpm.MovePlayer(Vector2.zero);
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                    tpm.SwitchCameras();
                    armMesh.SetActive(false);
                }
                break;
            // Changes from the eye to the heart
            case activeController.EYE:
                if (hc != null)
                {
                    StartCoroutine(SetEyeImageRenderer());
                    tpm.MovePlayer(Vector2.zero);
                    eyeCam.Priority = 100;
                    currentActive = activeController.EYE;
                    tpm.SwitchCameras();
                    armMesh.SetActive(false);
                }
                break;*/
        }
    }
    #endregion

    #region Select Radial Menu Item
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
            ec.OutlineScript.enabled = false;
        }
        else if (radialMenuPanel.activeInHierarchy && Time.timeScale != 0)
        {
            switch (radialMenu.GetComponent<RadialMenuController>().Im.sprite.name)
            {
                case "RadialMenuNewAtlas_10":
                    OnBody();
                    break;
                case "RadialMenuNewAtlas_12":
                    OnEye();
                    break;
                case "RadialMenuNewAtlas_8":
                    OnHand();
                    break;
                case "RadialMenuNewAtlas_11":
                    OnHeart();
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
