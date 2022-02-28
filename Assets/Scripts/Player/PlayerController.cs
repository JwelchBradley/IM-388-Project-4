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
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    #region Pause Menu
    /// <summary>
    /// The pause menu script.
    /// </summary>
    private PauseMenuBehavior pmb;

    public PauseMenuBehavior PMB
    {
        get => pmb;
    }
    #endregion

    #region Character Controllers
    /// <summary>
    /// List of controllers.
    /// </summary>
    public enum activeController { HAND, PERSON, EYE, INTESTINES, MOUTH };

    /// <summary>
    /// The currently active controller;
    /// </summary>
    private activeController currentActive = activeController.PERSON;

    /// <summary>
    /// Gets the currently active character controller.
    /// </summary>
    public activeController CurrentActive
    {
        get => currentActive;
    }

    private List<GameObject> keys = new List<GameObject>();

    public List<GameObject> Keys
    {
        get => keys;
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

    /// <summary>
    /// The movement script for the hand controller.
    /// </summary>
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

    /// <summary>
    /// The current active eyecontroller.
    /// </summary>
    public EyeController EC
    {
        get => ec;
        set
        {
            ec = value;
        }
    }

    private EarController earCon;

    public EarController EarCon
    {
        get => earCon;
        set
        {
            earCon = value;
        }
    }

    /// <summary>
    /// The eye casting component.
    /// </summary>
    private EyeCaster eCaster;

    /// <summary>
    /// The eye casing component.
    /// </summary>
    public EyeCaster ECaster
    {
        get => eCaster;
    }

    [SerializeField]
    private GameObject castableEye;

    [SerializeField]
    private GameObject castableEar;

    private GameObject currentCastableObject;

    [SerializeField]
    private HeartController hc;

    public HeartController HC
    {
        get => hc;
    }

    [SerializeField]
    private GameObject heartMesh;

    public GameObject HeartMesh
    {
        get => heartMesh;
    }
    #endregion
    #endregion

    #region Cameras
    /// <summary>
    /// The virtual camera for when the player is standing.
    /// </summary>
    private CinemachineVirtualCamera walkCam;

    /// <summary>
    /// The virtual camera for the eye.
    /// </summary>
    private CinemachineVirtualCamera eyeCam;

    /// <summary>
    /// The camera used by the eye.
    /// </summary>
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

    #region Renderering Masks
    [Tooltip("The layermask of the eye image renderer")]
    [SerializeField] private LayerMask eyeImageRendererMask;

    /// <summary>
    /// The starting layermask renderer.
    /// </summary>
    private LayerMask startingRendererMask;
    #endregion
    #endregion

    #region FPS Visuals
    [Header("FPS Visuals")]
    [Tooltip("The mesh of the fps player")]
    [SerializeField]
    private GameObject fpsMesh;

    [Tooltip("The mesh of the fps player eye")]
    [SerializeField]
    private GameObject eyeMesh;

    /// <summary>
    /// The mesh of the fps player eye.
    /// </summary>
    public GameObject EyeMesh
    {
        get => eyeMesh;
    }

    [Tooltip("The mesh of the fps player hand")]
    [SerializeField]
    private GameObject handMesh;

    /// <summary>
    /// The mesh of the fps player hand.
    /// </summary>
    public GameObject HandMesh
    {
        get => handMesh;
    }

    [Tooltip("The mesh of the players arms")]
    [SerializeField]
    private GameObject armMesh;

    [Tooltip("The right hand of the zombies FPS view")]
    [SerializeField] private GameObject rightHandArmMesh;

    /// <summary>
    /// The right hand of the zombies FPS view.
    /// </summary>
    public GameObject RightHandArmMesh
    {
        get => rightHandArmMesh;
    }

    [Tooltip("The animator of the players arms")]
    [SerializeField] private Animator[] armAnim;

    /// <summary>
    /// The animator of the players arms.
    /// </summary>
    public Animator[] ArmAnim
    {
        get => armAnim;
    }
    #endregion

    #region Radial Menu
    private bool canOpenRadial = true;

    public bool CanOpenRadial
    {
        set
        {
            canOpenRadial = value;
        }
    }

    private bool currentRadial = false;

    public bool CurrentRadial
    {
        get => currentRadial;
    }
    #endregion

    #region Pickup
    [Header("Pickup")]
    [Tooltip("The max distance for normal interactions")]
    [Range(0, 50)]
    [SerializeField] private float maxDist = 10;

    [Range(0, 50)]
    [Tooltip("The max distance for interactions as the hand")]
    [SerializeField] private float maxHandDist = 20;

    #region Interaction Masks
    [Tooltip("The layermask for pick up objects")]
    [SerializeField] private LayerMask pickUpSurface;

    [Tooltip("The layermask for interactable objects")]
    [SerializeField] private LayerMask interactableMask;

    [Tooltip("The layer mask used to check if a wall is obscuring an object")]
    [SerializeField] private LayerMask wallCheckMask;
    #endregion

    /// <summary>
    /// Holds true if the player is able to pick something up.
    /// </summary>
    private bool canPickUp = false;

    /// <summary>
    /// The current interactable object being looked at.
    /// </summary>
    private IInteractable interactable;

    /// <summary>
    /// The current interactable object being looked at.
    /// </summary>
    public IInteractable InteractableObject
    {
        get => interactable;
    }

    /// <summary>
    /// Holds reference to the last raycast data.
    /// </summary>
    private RaycastHit hit;
    #endregion

    #region Crosshair
    /// <summary>
    /// The crosshair for the zombie.
    /// </summary>
    private GameObject crosshair;
    #endregion

    #region Checkpoints
    [Tooltip("The current checkpoint. Set this variable to create a default checkpoint.")]
    public GameObject checkpoint;
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

        // Initializes the main camera
        mainCam = Camera.main;
        mainCamBrain = mainCam.GetComponent<CinemachineBrain>();
        startingRendererMask = mainCam.cullingMask;

        // Sets the cursor state
        Invoke("InitializeCursor", 0.1f);
        Cursor.visible = false;

        crosshair = GameObject.Find("Crosshair");

        GameObject hand = GameObject.Find("Hand Player");
        if (hand != null)
        {
            // Initializes hand if it isn't created yet
            InitializeHand(hand);

            // Sets all the values for it going to the hand
            pm.MovePlayer(Vector2.zero, false);
            ChangeMeshState(true, false, false);
            ToHand();
        }
    }

    /// <summary>
    /// Sets the cursor to be locked to the center of the screen.
    /// </summary>
    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        }
    }

    /// <summary>
    /// Makes the player jump.
    /// </summary>
    public void OnJump()
    {
        switch (currentActive)
        {
            case activeController.HAND:
                tpm.Jump();
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
        }
    }
    #endregion
    #endregion

    #region Radial Menu
    public void OnOpenMenu()
    {
        if (canOpenRadial && Time.timeScale != 0 && !pmb.Note.activeInHierarchy)
        {
            currentRadial = !pmb.RadialMenuPanel.activeInHierarchy;
            pmb.RadialMenuPanel.SetActive(currentRadial);
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

            CheckForInteractable(ref hit);

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

                    if(interactable != null)
                    interactable.DisplayInteractText();
                }
            }
            else
            {
                interactable = null;
                pmb.PickUpText.text = "";
            }
        }
        else if (eCaster.IsCasting)
        {
            pmb.PickUpText.text = "Left click to cast eye";
        }
        else
        {
            interactable = null;
            canPickUp = false;
            pmb.PickUpText.text = "";
        }
    }

    /// <summary>
    /// Checks if an allowed interactable is in view.
    /// </summary>
    /// <param name="hit"></param>
    private void CheckForInteractable(ref RaycastHit hit)
    {
        LayerMask currentMask = pickUpSurface;
        float currentDist = maxDist;

        if (currentActive != activeController.PERSON)
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

            if (Vector3.Distance(hitTemp.point, mainCam.transform.position) < Vector3.Distance(hit.point, mainCam.transform.position))
            {
                canPickUp = false;
            }
        }
    }

    public void OnResetEye()
    {
        if(interactable != null && !pmb.RadialMenuPanel.activeInHierarchy)
        {
            interactable.Interact();
        }
    }
    #endregion

    public void UpdateBodyPart(activeController newActive)
    {
        eCaster.IsCasting = false;

        if (currentActive.Equals(newActive))
        {
            return;
        }

        switch (currentActive)
        {
            case activeController.PERSON:
                DeactivatePerson();
                break;
            case activeController.HAND:
                DeactivateHand();
                break;
            case activeController.EYE:
                DeactivateEye();
                break;
            case activeController.INTESTINES:
                DeactivateIntestines();
                break;
            case activeController.MOUTH:
                DeactivateMouth();
                break;
        }

        switch (newActive)
        {
            case activeController.PERSON:
                ActivatePerson();
                break;
            case activeController.HAND:
                ActivateHand();
                break;
            case activeController.EYE:
                ActivateEye();
                break;
            case activeController.INTESTINES:
                ActivateIntestines();
                break;
            case activeController.MOUTH:
                ActivateMouth();
                break;
        }

        currentActive = newActive;
    }

    #region Body
    #region Input Call
    private void OnBody()
    {
        if(!currentActive.Equals(activeController.PERSON))
        UpdateBodyPart(activeController.PERSON);
    }
    #endregion

    #region Activation and Deactivation
    private void ActivatePerson()
    {
        crosshair.SetActive(true);

        ToZombie();

        StartCoroutine(EnableArms());
    }

    private void DeactivatePerson()
    {
        armMesh.SetActive(false);
        // Sets all the values for it going to the hand
        pm.MovePlayer(Vector2.zero, false);
        // If player is trying to cast eye then stop it
        if (ec == null && eCaster.IsCasting)
        {
            CastingStateChange(null);
        }
    }

    #region Change Functions
    /// <summary>
    /// Switches the state to being the zombie.
    /// </summary>
    private void ToZombie()
    {
        if (tpm != null)
        {
            tpm.OutlineScript.enabled = true;
        }

        if (ec != null)
        {
            ec.OutlineScript.enabled = true;
        }

        UpdateCamera(walkCam);

        currentActive = activeController.PERSON;
        fpsMesh.SetActive(false);
    }

    /// <summary>
    /// Enables the arms.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableArms()
    {
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
    #endregion
    #endregion

    #region Hand
    /// <summary>
    /// Gets everything necessary for the hand to function.
    /// </summary>
    /// <param name="hand">The hand gameobject.</param>
    private void InitializeHand(GameObject hand)
    {
        handMesh.SetActive(false);
        rightHandArmMesh.SetActive(false);
        tpm = hand.GetComponentInChildren<ThirdPersonMovement>();
    }

    #region Input Call
    /// <summary>
    /// Handles changes to and from the hand.
    /// </summary>
    public void OnHand()
    {
        if(!currentActive.Equals(activeController.HAND))
        UpdateBodyPart(activeController.HAND);
    }
    #endregion

    #region Activation and Deactivation
    private void ActivateHand()
    {
        // Initializes hand if it isn't created yet
        IsHandActiveCheck();

        ChangeMeshState(true, false, false);
        ToHand();
    }

    private void DeactivateHand()
    {
        tpm.SwitchCameras();
        tpm.MovePlayer(Vector2.zero);
    }

    #region Change Functions
    /// <summary>
    /// Initializes the hand if it hasn't been created yet.
    /// </summary>
    private void IsHandActiveCheck()
    {
        if (tpm == null)
        {
            GameObject hand = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Third Person Player/Third Person Player", typeof(GameObject)), transform.position + Camera.main.transform.forward * 2, transform.rotation);
            InitializeHand(hand);
        }
    }

    /// <summary>
    /// Handles the event of changing to the hand from the person.
    /// </summary>
    private void ToHand()
    {
        currentActive = activeController.HAND;
        if (tpm.OutlineScript != null)
            tpm.OutlineScript.enabled = false;
        tpm.SwitchCameras();
    }
    #endregion
    #endregion
    #endregion

    #region Eye
    /// <summary>
    /// Initializes the eye when it is first placed.
    /// </summary>
    /// <param name="eye"></param>
    private void InitializeEye(GameObject eye)
    {
        // Hides eye on player and shows zombie
        eyeMesh.SetActive(false);
        pm.MovePlayer(Vector2.zero, false);

        // Gets component references
        ec = eye.GetComponentInChildren<EyeController>();
        eyeCam = eye.GetComponentInChildren<CinemachineVirtualCamera>();

        ChangeToEye();
    }

    #region Input Call
    /// <summary>
    /// Changes to and from the eye.
    /// </summary>
    private void OnEye()
    {
        if (ec != null)
        {
            UpdateBodyPart(activeController.EYE);
        }
        else
        {
            CastingStateChange(castableEye);
        }
        #region Old
        /*
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
                    pm.MovePlayer(Vector2.zero, false);
                    ChangeToEye();
                }
                break;

                
            // Changes to the eye from the hand
            case activeController.HAND:
                if(ec != null)
                {
                    tpm.SwitchCameras();
                    tpm.MovePlayer(Vector2.zero);

                    ChangeToEye();
                }
                break;
        }*/
        #endregion
    }
    #endregion

    #region Activate and Deactivate
    private void ActivateEye()
    {
            ChangeToEye();
    }

    private void DeactivateEye()
    {
        mainCam.cullingMask = startingRendererMask;
    }
    #endregion

    #region Change Functions
    /// <summary>
    /// Changes the player to be using the eye.
    /// </summary>
    private void ChangeToEye()
    {
        // Sets the player state
        currentActive = activeController.EYE;
        ec.OutlineScript.enabled = false;

        ChangeMeshState(true, false, false);
        UpdateCamera(eyeCam);
        StartCoroutine(SetEyeImageRenderer());
    }

    /// <summary>
    /// Sets the image renderer for the eye to see hidden images.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetEyeImageRenderer()
    {
        // Garuntees a wait for the camera to blend
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        while (mainCamBrain.IsBlending)
        {
            yield return new WaitForFixedUpdate();
        }

        if (currentActive.Equals(activeController.EYE))
        {
            mainCam.cullingMask = eyeImageRendererMask;
        }
    }

    /// <summary>
    /// Stops the casting funciton for the eye.
    /// </summary>
    private void CastingStateChange(GameObject newObj)
    {
        currentCastableObject = newObj;

        if (currentActive.Equals(activeController.PERSON))
        {
            eCaster.IsCasting = !eCaster.IsCasting;
            crosshair.SetActive(!crosshair.activeInHierarchy);
        }
    }
    #endregion

    /// <summary>
    /// Handles the looking of the eye.
    /// </summary>
    /// <param name="input">The input mouse delta.</param>
    public void OnMouseLook(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        if(!mainCamBrain.IsBlending && !pmb.RadialMenuPanel.activeInHierarchy && Time.timeScale != 0)
        {
            if (currentActive.Equals(activeController.EYE))
            {
                ec.Look(inputVec);
            }
            else if (currentActive.Equals(activeController.HAND))
            {
                tpm.UpdateCameraCall(inputVec);
            }
        }
    }
    #endregion

    #region Heart
    #region Input Call
    private void OnHeart()
    {
        //UpdateBodyPart(activeController.HEART);
    }
    #endregion

    #region Activation and Deactivation
    private void ActivateHeart()
    {
        heartMesh.SetActive(true);
    }

    private void DeactivateHeart()
    {
        heartMesh.SetActive(false);
    }

    #region Change Functions

    #endregion
    #endregion
    #endregion

    #region Intestines
    #region Input Call
    private void OnIntestine()
    {
        UpdateBodyPart(activeController.INTESTINES);
    }
    #endregion

    #region Activation and Deactivation
    private void DeactivateIntestines()
    {

    }

    private void ActivateIntestines()
    {

    }
    #endregion
    #endregion

    #region Mouth
    #region Input Call
    private void OnMouth()
    {
        UpdateBodyPart(activeController.MOUTH);
    }
    #endregion

    #region Activation and Deactivation
    private void ActivateMouth()
    {

    }
    private void DeactivateMouth()
    {

    }
    #endregion
    #endregion

    #region Ear
    private void InitializeEar(GameObject obj)
    {
        earCon = obj.GetComponent<EarController>();
        crosshair.SetActive(true);

    }

    private void OnEar()
    {
        if(earCon == null)
        CastingStateChange(castableEar);
    }
    #endregion

    #region Radial Menu
    /// <summary>
    /// Handles the spawning of the eye when the mouse is clicked and the playing is casting.
    /// </summary>
    public void OnClick()
    {
        if (eCaster.IsCasting && eCaster.CanCast && Time.timeScale != 0 && !pmb.RadialMenuPanel.activeInHierarchy)
        {
            eCaster.IsCasting = false;
            GameObject obj = eCaster.SpawnObject(currentCastableObject);

            if (currentCastableObject.Equals(castableEye))
            {
                InitializeEye(obj);
            }
            else if (currentCastableObject.Equals(castableEar))
            {
                InitializeEar(obj);
            }
        }
        else if (pmb.RadialMenuPanel.activeInHierarchy && Time.timeScale != 0)
        {
            if (pmb.RMC.currentHovered.Equals(activeController.EYE))
            {
                OnEye();
            }
            else
            {
                UpdateBodyPart(pmb.RMC.currentHovered);
            }

            Invoke("OnOpenMenu", 0.05f);
        }
    }
    #endregion
    #endregion

    #region Checkpoints
    public void KillPlayer()
    {
        transform.position = checkpoint.transform.position;
        transform.rotation = checkpoint.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Checkpoint")
        {
            checkpoint = other.gameObject;
        }
    }
    #endregion
    #endregion

    #region Change States
    /// <summary>
    /// Sets the camera to be the new camera.
    /// </summary>
    /// <param name="newCam">The camera to be set to.</param>
    private void UpdateCamera(CinemachineVirtualCamera newCam)
    {
        mainCamBrain.ActiveVirtualCamera.Priority = 0;
        newCam.Priority = 1;
    }

    /// <summary>
    /// Changes between different mesh states.
    /// </summary>
    /// <param name="shouldShowZombie">Holds true if the zombie should be shown.</param>
    /// <param name="shouldShowArms">Holds true if the arms should be shown.</param>
    /// <param name="shouldShowCrosshair">Holds true if the crosshair should be shown.</param>
    private void ChangeMeshState(bool shouldShowZombie, bool shouldShowArms, bool shouldShowCrosshair)
    {
        fpsMesh.SetActive(shouldShowZombie);
        armMesh.SetActive(shouldShowArms);
        crosshair.SetActive(shouldShowCrosshair);
    }
    #endregion
}
