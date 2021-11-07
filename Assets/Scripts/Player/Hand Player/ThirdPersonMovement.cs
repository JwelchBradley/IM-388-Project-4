using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController controller;

    private Transform cam;
    private RaycastHit hit;

    private Vector3 moveVec;

    private float jumpVel;

    private bool isGrounded = true;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVeloctiy;

    [Header("Jump")]
    [SerializeField]
    [Tooltip("How much velocity the player has when jumping")]
    [Range(0, 5)]
    private float jumpHeight = 3;

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

    [Header("Climb")]
    [SerializeField]
    [Tooltip("How far away the player will they start climbing")]
    [Range(0, 2)]
    private float climbDist = 1;

    [SerializeField]
    private LayerMask wallMask;

    /// <summary>
    /// The cinemachine brain on the main camera.
    /// </summary>
    private CinemachineBrain mainCamBrain;

    private CinemachineFreeLook cineCam;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        mainCamBrain = Camera.main.GetComponent<CinemachineBrain>();
        cineCam = transform.parent.gameObject.GetComponentInChildren<CinemachineFreeLook>();
    }

    #region Input Calls
    /// <summary>
    /// The player jumps if they are on the ground.
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            jumpVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void MovePlayer(Vector2 input)
    {
        moveVec = new Vector3(input.x, 0, input.y).normalized;
    }
    #endregion

    #region Calculations
    private void FixedUpdate()
    {
        IsGrounded();

        GravityCalculation();

        if (moveVec.magnitude >= 0.1f && !mainCamBrain.IsBlending)
        {
            RotatePlayer();
        }
    }

    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    private void IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckPos.transform.position, groundCheckDist, groundMask);
    }

    [SerializeField]
    private GameObject visuals;
    private void RotatePlayer()
    {
        float xValue = cineCam.m_XAxis.Value;
        if (xValue < 0)
        {
            xValue += 360;
        }
        //float angle = Mathf.Atan2(moveVec.x, moveVec.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.Atan2(moveVec.x, moveVec.z) * Mathf.Rad2Deg + xValue;
        float smoothAngle = Mathf.SmoothDampAngle(visuals.transform.localRotation.eulerAngles.y, angle, ref turnSmoothVeloctiy, turnSmoothTime);
        visuals.transform.localRotation = Quaternion.Euler(0, smoothAngle, 0f);

        MovePlayer();
    }

    private float xAngle = 0;

    private void MovePlayer()
    {
        Vector3 moveDir = visuals.transform.forward;
        controller.Move(moveSpeed * moveDir.normalized * Time.fixedDeltaTime);

        ClimbObject();
    }

    private bool isClimbing = false;
    private bool hasClimbed = false;
    float xMod = 1;
    private void ClimbObject()
    {
        bool canClimb = Physics.Raycast(transform.position, visuals.transform.forward, out hit, climbDist, wallMask);

        if (canClimb)
        {
            isClimbing = true;
            if (hit.normal.x != 0 || hit.normal.z != 0)
                xAngle = -90;
            mainCamBrain.m_WorldUpOverride = transform.parent;
            if (!hasClimbed)
            {
                if(hit.normal.x > 0)
                {
                    xMod = 1.5f;
                    Debug.Log(true);
                }
                else if(hit.normal.x < 0)
                {
                    xMod = -1.5f;
                }
                else
                {
                    if (hit.normal.z == 1)
                        xMod = hit.normal.z;
                    else
                        xMod = 0;
                }
                Vector3 pos = transform.position;
                transform.parent.position = pos;
                transform.position = pos;
                transform.parent.rotation = Quaternion.Euler(xAngle, 0, 180 * xMod);
                transform.position += transform.up*-0.4f;
                hasClimbed = true;
            }

        }
    }

    /// <summary>
    /// Applyes gravity to the player.
    /// </summary>
    private void GravityCalculation()
    {
        if (isClimbing)
        {
            jumpVel = 0;
        }
        else if (!isGrounded)
        {
            jumpVel += gravity * Time.fixedDeltaTime;
        }

        controller.Move(new Vector3(0, jumpVel, 0) * Time.fixedDeltaTime);
    }
    #endregion
}
