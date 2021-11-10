/*****************************************************************************
// File Name :         PlayerMovement.cs
// Author :            Jacob Welch
// Creation Date :     28 August 2021
//
// Brief Description : Handles the movement of the player.
*****************************************************************************/
using Cinemachine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Handles gravity and 
    ///                             // TODO smooth player velocity
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// Animator of the FPS Controller.
    /// </summary>
    private Animator anim;

    private bool active = true;

    #region Movement
    [Header("Move Speed")]
    [SerializeField]
    [Tooltip("The speed the player moves while standing")]
    [Range(0, 40)]
    private float walkSpeed = 10;

    [SerializeField]
    [Tooltip("The speed the player moves while crouching")]
    [Range(0, 40)]
    private float crouchWalkSpeed = 6;

    /// <summary>
    /// Holds the current movement speed of the player.
    /// </summary>
    private float currentSpeed = 10;

    /// <summary>
    /// Holds the input player movement.
    /// </summary>
    private Vector3 move = Vector3.zero;

    /// <summary>
    /// The Character Controller of the player. (used to move the player)
    /// </summary>
    private CharacterController controller;
    #endregion

    #region Air Movement
    [Header("Jump")]
    [SerializeField]
    [Tooltip("How much velocity the player has when jumping")]
    [Range(0, 5)]
    private float jumpHeight = 3;

    [SerializeField]
    [Tooltip("How much velocity the player has when jumping")]
    [Range(0, 5)]
    private float crouchJumpHeight = 3;

    /// <summary>
    /// The height the player currently jumps to.
    /// </summary>
    private float currentJumpHeight = 3;

    [SerializeField]
    [Tooltip("The rate at which gravity scales")]
    [Range(-50, 0)]
    private float gravity = -9.8f;

    [Space]
    [SerializeField]
    [Tooltip("How far away the player must be from the ground to be grounded")]
    [Range(0, 2)]
    private float groundCheckDist = 0.5f;

    [SerializeField]
    [Tooltip("Position on the player to check ground from")]
    private GameObject groundCheckPos;

    [SerializeField]
    [Tooltip("The layer mask of the ground")]
    private LayerMask groundMask;

    [SerializeField]
    [Tooltip("The layer mask of interactables")]
    private LayerMask interactableMask;

    /// <summary>
    /// Holds true if the player is on the ground.
    /// </summary>
    private bool isGrounded = false;

    /// <summary>
    /// Holds reference to interactable objects that the player is on.
    /// </summary>
    public static Collider[] playerIsOn;
    #endregion

    #region Cameras
    /// <summary>
    /// The transform of the camera used for movement direction calculations.
    /// </summary>
    private Transform cameraTransform;

    /// <summary>
    /// The cinemachine brain on the main camera.
    /// </summary>
    private CinemachineBrain mainCamBrain;

    /// <summary>
    /// The virtual camera for when the player is standing.
    /// </summary>
    private CinemachineVirtualCamera walkCam;

    /// <summary>
    /// The CinemachinePOV of the walk camera.
    /// </summary>
    private CinemachinePOV walkCamPOV;
    #endregion

    #region Visuals
    [SerializeField]
    private GameObject mesh;
    #endregion
    #endregion

    #region Functions
    #region Initilization
    /// <summary>
    /// Initializes the player movement components.
    /// </summary>
    private void Awake()
    {
        currentSpeed = walkSpeed;
        currentJumpHeight = jumpHeight;

        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        GetCameras();
    }

    /// <summary>
    /// Gets all player cameras and components.
    /// </summary>
    private void GetCameras()
    {
        cameraTransform = Camera.main.transform;
        mainCamBrain = Camera.main.GetComponent<CinemachineBrain>();
        walkCam = GameObject.Find("Walk vcam").GetComponent<CinemachineVirtualCamera>();
        walkCamPOV = walkCam.GetCinemachineComponent<CinemachinePOV>();
        CinemachineCore.GetInputAxis = GetAxisCustom;

        SetCameraSens();
    }

    private void SetCameraSens()
    {
        if (!PlayerPrefs.HasKey("X Sens"))
        {
            PlayerPrefs.SetFloat("X Sens", 600);
            PlayerPrefs.SetFloat("Y Sens", 400);
        }

        if (PlayerPrefs.GetFloat("X Sens") != 0 && PlayerPrefs.GetFloat("Y Sens") != 0)
        {
            walkCamPOV.m_HorizontalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("X Sens");
            walkCamPOV.m_VerticalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("Y Sens");
        }
    }
    #endregion

    #region Input Calls
    /// <summary>
    /// Recieves player input as a Vector2.
    /// </summary>
    /// <param name="move">The Vector2 movement for the player.</param>
    public void MovePlayer(Vector2 move, bool active)
    {
        this.active = active;
        this.move = new Vector3(move.x, 0, move.y);
    }

    /// <summary>
    /// The player jumps if they are on the ground.
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(currentJumpHeight * -2f * gravity);
        }
    }

    /// <summary>
    /// Crouches or uncrouches the player.
    /// </summary>
    public void Crouch()
    {
        bool newState = !anim.GetBool("isCrouched");

        if (!isGrounded)
        {
            StartCoroutine(CrouchInAir(newState));
        }
        else 
        {
            anim.SetBool("isCrouched", newState);

            if (newState)
            {
                CrouchHelper(crouchWalkSpeed, crouchJumpHeight);
            }
            else
            {
                CrouchHelper(walkSpeed, jumpHeight);
            }
        }
    }

    private IEnumerator CrouchInAir(bool state)
    {
        while (!isGrounded)
        {
            yield return new WaitForFixedUpdate();
        }

        Crouch();
    }

    /// <summary>
    /// Sets the current camera and changes the speed.
    /// </summary>
    /// <param name="camPriority">Sets the cam priority of the crouch cam to be higher or lower than the walk cam.</param>
    /// <param name="setTo">The cam that is having its value changed.</param>
    /// <param name="setFrom">The cam that is passing along its values.</param>
    /// <param name="speed">The new speed of the player.</param>
    private void CrouchHelper(float speed, float jumpHeight)
    {
        // Sets the player move speed of the player
        currentSpeed = speed;
    }
    #endregion

    #region Calculations
    private void FixedUpdate()
    {
        if (!mainCamBrain.IsBlending)
            MoveCalculation();

        GravityCalculation();

        IsGrounded();

        if (active)
        {
            RotateMesh();
        }
    }

    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    private void IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckPos.transform.position, groundCheckDist, groundMask);

        if (isGrounded)
        {
            controller.stepOffset = 1;
        }
        else
        {
            controller.stepOffset = 0;
        }
    }

    /// <summary>
    /// Moves the player.
    /// </summary>
    private void MoveCalculation()
    {
        Vector3 currentMove = cameraTransform.right * move.x + cameraTransform.forward * move.z;
        currentMove.y = 0f;
        currentMove.Normalize();

        controller.Move(currentMove * currentSpeed * Time.fixedDeltaTime);
    }

    
    private void RotateMesh()
    {
        Quaternion camRot = cameraTransform.rotation;
        camRot.eulerAngles = new Vector3(0, camRot.eulerAngles.y+90, 0);
        mesh.transform.rotation = camRot;
    }

    /// <summary>
    /// Applyes gravity to the player.
    /// </summary>
    private void GravityCalculation()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;

            controller.Move(velocity * Time.fixedDeltaTime);
        }
        else
        {
            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Collisions
    [SerializeField]
    private float pushPower = 2.0f;
    [SerializeField]
    private LayerMask pushableMask;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(pushableMask == (pushableMask | (1 << hit.gameObject.layer)))
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            // no rigidbody
            if (body == null || body.isKinematic) { return; }
            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3)
            {
                return;
            }
            // Calculate push direction from move direction,
            // we only push objects to the sides never up and down
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            // If you know how fast your character is trying to move,
            // then you can also multiply the push velocity by that.

            // Apply the push
            body.velocity = pushDir * pushPower;
        }
    }
    #endregion

    #region Camera Limitation
    private float GetAxisCustom(string axisName)
    {
        if (mainCamBrain.IsBlending)
            return 0;
        return Input.GetAxis(axisName);
    }
    #endregion
    #endregion
}
