using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController controller;

    private Transform cam;

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

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
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
        //Vector2 inputVec = input.Get<Vector2>();

        moveVec = new Vector3(input.x, 0, input.y).normalized;
    }
    #endregion

    #region Calculations
    private void FixedUpdate()
    {
        IsGrounded();

        GravityCalculation();

        if (moveVec.magnitude >= 0.1f)
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

    private void RotatePlayer()
    {
        float angle = Mathf.Atan2(moveVec.x, moveVec.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVeloctiy, turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

        MovePlayer(angle);
    }

    private void MovePlayer(float targetAngle)
    {
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveSpeed * moveDir.normalized * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Applyes gravity to the player.
    /// </summary>
    private void GravityCalculation()
    {
        if (!isGrounded)
        {
            jumpVel += gravity * Time.deltaTime;

            //controller.Move(moveVec * Time.fixedDeltaTime);
        }

        controller.Move(new Vector3(0, jumpVel, 0) * Time.fixedDeltaTime);
    }
    #endregion
}
