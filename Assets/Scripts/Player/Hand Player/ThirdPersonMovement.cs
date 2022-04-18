using Cinemachine;
using System.Collections;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    #region Variables
    [SerializeField]
    [Tooltip("The upgrade type for this object")]
    private int type = 0;

    public int Type
    {
        get => type;
    }

    #region Camera
    #region Normal Camera
    /// <summary>
    /// The main camera of this scene.
    /// </summary>
    private Camera mainCam;

    [Tooltip("The target that the camera looks at")]
    [SerializeField] private GameObject cinemachineCameraTarget;

    /// <summary>
    /// The given mouse delta input
    /// </summary>
    private Vector2 input;

    #region Senstitivity Variables
    /// <summary>
    /// The x sensitivity of the player.
    /// </summary>
    private float xSens = 1;

    /// <summary>
    /// The x sensitivity of the player.
    /// </summary>
    public float XSens
    {
        set
        {
            xSens = value;
        }
    }

    /// <summary>
    /// The y sensitivity of the player.
    /// </summary>
    private float ySens = 1;

    /// <summary>
    /// The y sensitivity of the player.
    /// </summary>
    public float YSens
    {
        set
        {
            ySens = value;
        }
    }
    #endregion

    #region Target Rotation Variables
    /// <summary>
    /// The current target x rotation of the camera.
    /// </summary>
    float cinemachineTargetXRot = 0;

    /// <summary>
    /// The current target y rotation of the camera.
    /// </summary>
    float cinemachineTargetYRot = 0;
    #endregion

    #region Clamp Variables
    [Header("Camera")]
    [Tooltip("The bottom clamp for the camera")]
    [Range(-30, 0)]
    [SerializeField] private float bottomClamp = -0;
    [Tooltip("The top clamp for the camera")]
    [Range(45, 90)]
    [SerializeField] private float topClamp = 90;
    #endregion
    #endregion

    #region wall
    [Tooltip("The target that the camera looks at when the hand is on a wall")]
    [SerializeField] private Transform cinemachineCameraWallTarget;

    private float climbTopClamp = 30;

    private float cinemachineTargetXRotWall = 0;
    private float cinemachineTargetYRotWall = 0;
    private float cinemachineWallRightClamp = 90;
    private float cinemachineWallLeftClamp = -90;
    #endregion
    #endregion

    #region Movement
    private float currentSpeed = 0;
    private float speedChangeRate = 5;
    private float targetRotation = 0;
    private float rotationVelocity = 0;
    [SerializeField] private float rotationSmoothTime = 0.075f;
    private string moveAnimationString = "Crawling";
    private int moveAnimationHash;
    private string jumpAnimationString = "isJumping";
    private int jumpAnimationHash;
    #endregion

    private CharacterController controller;
    private float startingStepOffset;

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

    #region Climbing
    [Header("Climb")]
    [SerializeField]
    [Tooltip("How far away the player will they start climbing")]
    [Range(0, 2)]
    private float climbDist = 1;

    [SerializeField]
    private LayerMask wallMask;

    private bool jumpOffWall = false;

    [SerializeField]
    private Animator anim;
    #endregion

    /// <summary>
    /// The cinemachine brain on the main camera.
    /// </summary>
    private CinemachineBrain mainCamBrain;

    [SerializeField]
    private CinemachineVirtualCamera cineCam;

    //CinemachinePOV cineCamPOV;

    [SerializeField]
    private CinemachineFreeLook climbCineCam;

    [SerializeField]
    private CinemachineVirtualCamera newClimbCineCam;

    public GameObject Hand
    {
        get => gameObject;
    }

    /// <summary>
    /// The outline script for the hands sillouette.
    /// </summary>
    private Outline outline;

    /// <summary>
    /// The outline script for the hands sillouette.
    /// </summary>
    public Outline OutlineScript
    {
        get => outline;
    }

    [SerializeField]
    private GameObject visuals;
    private float xRotChange = 0;
    private int climbTurnRate = 450;
    private bool isClimbing = false;
    float climbTransitionSpeed = 1;
    private bool isClimbTransitioning = false;
    private float startingCCRadius = 0.4f;
    #endregion

    #region Initilization
    private void Awake()
    {
        InitializeComponents();
        SetAnimationHashes();
        SetCameraSens();
    }

    private void InitializeComponents()
    {
        controller = GetComponent<CharacterController>();
        startingCCRadius = controller.radius;
        startingStepOffset = controller.stepOffset;
        mainCam = Camera.main;
        mainCamBrain = mainCam.GetComponent<CinemachineBrain>();
        outline = GetComponentInChildren<Outline>();
    }

    private void SetAnimationHashes()
    {
        moveAnimationHash = Animator.StringToHash(moveAnimationString);
        jumpAnimationHash = Animator.StringToHash(jumpAnimationString);
    }

    private void SetCameraSens()
    {
        if (!PlayerPrefs.HasKey("X Sens Hand"))
        {
            PlayerPrefs.SetFloat("X Sens Hand", .5f);
            PlayerPrefs.SetFloat("Y Sens Hand", .5f);
        }

        if (PlayerPrefs.GetFloat("X Sens Hand") != 0 && PlayerPrefs.GetFloat("Y Sens Hand") != 0)
        {
            XSens = PlayerPrefs.GetFloat("X Sens Hand");
            YSens = PlayerPrefs.GetFloat("Y Sens Hand");
        }
    }

    private int curClimb = 0;
    private int curNorm = 100;
    public void SwitchCameras()
    {
        if(curNorm != cineCam.m_Priority)
        {
            cineCam.m_Priority = curNorm;
            //climbCineCam.m_Priority = curClimb;
            newClimbCineCam.Priority = curClimb;
        }
        else
        {
            cineCam.m_Priority = -1;

            if(curClimb > 0)
            {
                newClimbCineCam.Priority = 1;

                //climbCineCam.m_Priority = 1;
            }
            else
            {
                newClimbCineCam.Priority = -2;
            }
        }
    }
    #endregion

    #region Input Calls
    /// <summary>
    /// The player jumps if they are on the ground.
    /// </summary>
    public void Jump()
    {
        if (isClimbing && !isClimbTransitioning)
        {
            Physics.Raycast(visuals.transform.position, Vector3.down, out hit, 100f, groundMask);
            StartCoroutine(ClimbTransition());
            /*
            jumpVel = Mathf.Sqrt(jumpHeight * -8f * gravity);
            jumpOffWall = true;
            isClimbing = false;
            jumpOffWallUp = visuals.transform.up;
            Invoke("NotJumpOffWall", 1);*/
        }
        else if (isGrounded)
        {
            controller.stepOffset = 0;
            jumpVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    Vector3 jumpOffWallUp;
    private void NotJumpOffWall()
    {
        jumpOffWall = false;
    }

    public void MovePlayer(Vector2 input)
    {
        moveVec = new Vector3(input.x, 0, input.y).normalized;
    }
    #endregion

    #region Calculations
    private void FixedUpdate()
    {
        WhileActiveChecks();
        IsGrounded();
        GravityCalculation();
    }

    private void WhileActiveChecks()
    {
        if (!mainCamBrain.IsBlending)
        {
            if (!isClimbTransitioning)
            {
                if (isClimbing)
                {
                    CameraRotation(ref cinemachineTargetXRotWall, ref cinemachineTargetYRotWall, cinemachineCameraWallTarget, bottomClamp, climbTopClamp, true);
                }
                else
                {
                    CameraRotation(ref cinemachineTargetXRot, ref cinemachineTargetYRot, cinemachineCameraTarget.transform, bottomClamp, topClamp, false);
                }
            }


            PlayerMoveAnimation(moveVec);

            if (moveVec.magnitude >= 0.1f)
            {
                if (!isClimbTransitioning)
                {
                    if (!isClimbing)
                    {
                        NewMove();
                    }
                    else
                    {
                        ClimbRotate();
                        MovePlayerOverride(1.5f);
                    }
                }
            }
        }
    }

    #region Movement
    public void NewMove()
    {
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        float targetSpeed = moveVec == Vector3.zero ? 0.0f : moveSpeed;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = moveVec.magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            currentSpeed = Mathf.Lerp(currentHorizontalSpeed, moveSpeed * inputMagnitude, Time.fixedDeltaTime * speedChangeRate);

            // round speed to 3 decimal places
            currentSpeed = Mathf.Round(currentSpeed * 1000f) / 1000f;
        }
        else
        {
            currentSpeed = targetSpeed;
        }

        NewRotatePlayer();

        // move the player in target direction
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        controller.Move(targetDirection.normalized * (currentSpeed * Time.fixedDeltaTime) + new Vector3(0, jumpVel, 0) * Time.fixedDeltaTime);
        ClimbCheck();
    }

    #region Visuals
    private void NewRotatePlayer()
    {
        // normalise input direction
        moveVec = moveVec.normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (moveVec != Vector3.zero)
        {
            targetRotation = Mathf.Atan2(moveVec.x, moveVec.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(visuals.transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            // rotate to face input direction relative to camera position
            visuals.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    private void PlayerMoveAnimation(Vector3 current)
    {
        if (isClimbTransitioning || (current != Vector3.zero && isGrounded && jumpVel <= 0))
        {
            anim.SetBool(moveAnimationHash, true);
        }
        else
        {
            anim.SetBool(moveAnimationHash, false);
        }
    }
    #endregion
    #endregion

    #region Gravity
    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    private void IsGrounded()
    {
        bool climbOrTransition = isClimbing || isClimbTransitioning;

        LayerMask currentMask = climbOrTransition ? wallMask : groundMask;
        float currentCheckDist = climbOrTransition ? 1f : groundCheckDist;
        isGrounded = Physics.CheckSphere(groundCheckPos.transform.position, currentCheckDist, currentMask);

        if (isGrounded && !isClimbing && !isClimbTransitioning)
        {
            controller.stepOffset = startingStepOffset;
        }

        
        if (!isGrounded && !isClimbTransitioning && isClimbing)
        {
            isClimbing = false;
            jumpOffWall = true;
            Physics.Raycast(transform.position, Vector3.down, out hit, 100, wallMask);
            StartCoroutine(ClimbTransition());
        }
    }

    /// <summary>
    /// Applyes gravity to the player.
    /// </summary>
    private void GravityCalculation()
    {
        if (jumpOffWall)
        {
            jumpVel += gravity * Time.fixedDeltaTime;
            controller.Move(new Vector3(0, jumpVel, 0) * Time.fixedDeltaTime);
            anim.SetBool(jumpAnimationHash, true);
        }
        else if (isClimbing)
        {
            jumpVel = 0;
            controller.Move(-visuals.transform.up * Time.fixedDeltaTime);
            anim.SetBool(jumpAnimationHash, false);
        }
        else if (!isGrounded)
        {
            jumpVel += gravity * Time.fixedDeltaTime;
            anim.SetBool(jumpAnimationHash, true);
        }
        else if(jumpVel <= 0)
        {
            jumpVel = 0;
            anim.SetBool(jumpAnimationHash, false);
        }

        if ((moveVec == Vector3.zero || mainCamBrain.IsBlending) && (!isClimbing && !jumpOffWall))
            controller.Move(new Vector3(0, jumpVel, 0) * Time.fixedDeltaTime);
    }
    #endregion

    #region Climbing
    public void ClimbCheck()
    {
        if (!isClimbTransitioning)
        {
            bool canClimb = Physics.Raycast(transform.position, visuals.transform.forward, out hit, climbDist, wallMask);

            if (canClimb)
            {
                currentNormal = hit.normal;
                controller.stepOffset = 0;
                StartCoroutine(ClimbTransition());
            }
        }
    }

    private void SetClimbValues()
    {
        if (hit.normal.y >= 0.5f || hit.normal.y <= -0.5f)
        {
            cinemachineTargetXRot = cinemachineTargetXRotWall;
            cinemachineTargetYRot = 0;
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetYRot, cinemachineTargetXRot, 0.0f);

            controller.stepOffset = startingStepOffset;
            isClimbing = false;
            climbCineCam.Priority = 0;
            newClimbCineCam.Priority = 0;
            curClimb = 0;
            xRotChange = 0;
            controller.radius = startingCCRadius;
        }
        else
        {
            isClimbing = true;
            curClimb = 150;
            climbCineCam.Priority = 150;
            newClimbCineCam.Priority = 150;

            controller.radius = 0.2f;
        }
    }

    Vector3 currentNormal = Vector3.zero;
    private IEnumerator ClimbTransition()
    {
        Vector3 newRotation = hit.normal;
        newRotation = new Vector3(Mathf.Round(newRotation.x), Mathf.Round(newRotation.y), Mathf.Round(newRotation.z));

        // Finds rotation values
        Quaternion oldRotation = visuals.transform.localRotation;
        Quaternion targetRotation = Quaternion.LookRotation(-newRotation) * Quaternion.Euler(new Vector3(-90, 0, 0));
        PlayerMoveAnimation(Vector3.one);

        float oldCamValue = GetClimbCamOld();
        float newCamValue = GetClimbCamNew();

        float camLerpCheck = oldCamValue - newCamValue;

        if (isClimbing)
        {
            Physics.Raycast(visuals.transform.position, -visuals.transform.up, out RaycastHit tempHit, 2f, wallMask);
            if (currentNormal.x > 0.8f && tempHit.normal.z < -0.8f)
            {
                cinemachineTargetXRotWall += 360;
                oldCamValue = cinemachineTargetXRotWall;
            }
            else if (currentNormal.z < -0.8f && tempHit.normal.x > 0.8f)
            {
                cinemachineTargetXRotWall -= 360;
                oldCamValue = cinemachineTargetXRotWall;
            }

            if (hit.normal.y > 0.5f || hit.normal.y < -0.5f)
            {
                if (tempHit.normal.x > 0.5f)
                {
                    targetRotation *= Quaternion.Euler(0, 90, 0);
                }
                else if (tempHit.normal.x < -0.5f)
                {
                    targetRotation *= Quaternion.Euler(0, -90, 0);
                }
                else
                {
                    if (tempHit.normal.z > 0.8f)
                    {
                        targetRotation *= Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        targetRotation *= Quaternion.Euler(0, 180, 0);
                    }
                }
            }
            else
            {
                int mod = -90;
                xRotChange = 270;

                if (moveVec.x > 0)
                {
                    mod = -270;
                    xRotChange = 90;
                }

                targetRotation *= Quaternion.AngleAxis(mod, mainCam.transform.up);
            }
        }
        else if(currentNormal != hit.normal)
        {
            if (currentNormal.x > 0.5f)
            {
                targetRotation *= Quaternion.Euler(0, -90, 0);
            }
            else if (currentNormal.x < -0.5f)
            {
                targetRotation *= Quaternion.Euler(0, 90, 0);
            }
            else
            {
                if (currentNormal.z > 0.8f)
                {
                    targetRotation *= Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    targetRotation *= Quaternion.Euler(0, 0, 0);
                }
            }
        }

        if (camLerpCheck < 0)
        {
            camLerpCheck = -camLerpCheck;
        }

        /*
        if (camLerpCheck > 190)
        {
            newCamValue -= 360;
        }*/

        if(hit.normal.y > 0.1f || hit.normal.y < -0.1f)
        {
            cinemachineTargetXRot = oldCamValue;
        }

        isClimbTransitioning = true;
        SetClimbValues();

        targetRotation = Quaternion.Euler(new Vector3(Mathf.Round((targetRotation.eulerAngles.x/90))*90, Mathf.Round((targetRotation.eulerAngles.y / 90)) * 90, Mathf.Round(targetRotation.eulerAngles.z / 90) * 90));

        float t = 0;
        while (visuals.transform.localRotation != targetRotation)
        {
            // Rotates player model to new rotation
            t += Time.fixedDeltaTime * climbTransitionSpeed;
            t = Mathf.Clamp(t, 0, 1);
            visuals.transform.localRotation = Quaternion.Lerp(oldRotation, targetRotation, t);

            if (hit.normal.y >= -0.5f && hit.normal.y <= 0.5f)
            {
                cinemachineTargetXRotWall = Mathf.Lerp(oldCamValue, newCamValue, t);
                // Updates cinemachine follow target rotation (essentially rotates the camera)
                //cinemachineCameraWallTarget.rotation = Quaternion.Euler(cinemachineTargetYRotWall, cinemachineTargetXRotWall, 0.0f);
                CameraRotation(ref cinemachineTargetXRotWall, ref cinemachineTargetYRotWall, cinemachineCameraWallTarget, bottomClamp, climbTopClamp, false);
                climbCineCam.m_XAxis.Value = Mathf.Lerp(oldCamValue, newCamValue, t);
            }

            yield return new WaitForFixedUpdate();

            if (t == 1)
            {
                visuals.transform.localRotation = targetRotation;
            }

            MovePlayerOverride(4);
        }

        jumpOffWall = false;
        isClimbTransitioning = false;
    }

    #region Climb Cam Value Getters
    private float GetClimbCamOld()
    {
        if (isClimbing)
        {
            return cinemachineTargetXRotWall;
            //return climbCineCam.m_XAxis.Value;
        }
        else if (currentNormal.x < -0.8f)
        {
            return 90;
        }
        else if (currentNormal.x > 0.8f)
        {
            return 270;
        }
        else if (currentNormal.z > 0.8f)
        {
            return 180;
        }
        else
        {
            return 0;
        }
    }

    private float GetClimbCamNew()
    {
        if (currentNormal.x < -0.8f)
        {
            cinemachineWallLeftClamp = 10;
            cinemachineWallRightClamp = 170;
            return 90;
        }
        else if (currentNormal.x > 0.8f)
        {
            cinemachineWallLeftClamp = 190;
            cinemachineWallRightClamp = 350;
            return 270;
        }
        else if (currentNormal.z > 0.8f)
        {
            cinemachineWallLeftClamp = 100;
            cinemachineWallRightClamp = 260;
            return 180;
        }
        else
        {
            cinemachineWallLeftClamp = -80;
            cinemachineWallRightClamp = 80;
            return 0;
        }
    }
    #endregion

    #region Climb Player Movement Rotation
    private void ClimbRotate()
    {
        float targetRot = 0;
        #region Turn Singles
        if (moveVec.x == 1)  // 90
        {
            targetRot = 90;

        }
        else if (moveVec.x == -1) // 270
        {
            targetRot = 270;
        }
        else if (moveVec.z == -1)  // 180
        {
            targetRot = 180;
        }
        else if (moveVec.z == 1)  // 0
        {
            targetRot = 0;

            if ((int)(xRotChange / 180) % 2 == 1)
            {
                targetRot = 360;
            }
        }
        #endregion

        #region Turn Two Keys
        if (moveVec.x >= 0.5f && moveVec.z >= 0.5f)
        {
            targetRot = 45;
        }
        else if (moveVec.x >= 0.5f && moveVec.z <= -0.5f)
        {
            targetRot = 135;
        }
        else if (moveVec.x <= -0.5f && moveVec.z >= 0.5f)
        {
            targetRot = 315;
        }
        else if (moveVec.x <= -0.5f && moveVec.z <= -0.5f)
        {
            targetRot = 225;
        }
        #endregion

        if ((int)(xRotChange / 360) != 0)
            targetRot += (int)(xRotChange / 360) * 360;

        int turnDir = 1;

        float norm = 0;
        float offset = 0;

        if (targetRot > xRotChange)
        {
            norm = targetRot - xRotChange;
            offset = targetRot - (xRotChange + 360);
        }
        else
        {
            norm = xRotChange - (targetRot + 360);
            offset = xRotChange - targetRot;
        }

        if (norm < 0)
        {
            norm = -norm;
        }
        if (offset < 0)
        {
            offset = -offset;
        }

        if (norm > offset)
        {
            if (targetRot != xRotChange)
            {
                xRotChange += 360;
            }
            turnDir = -1;
        }

        if (xRotChange != targetRot)
        {
            float change = turnDir * climbTurnRate * Time.fixedDeltaTime;

            xRotChange += change;
            visuals.transform.RotateAround(visuals.transform.position, visuals.transform.up, change);
        }
    }
    #endregion

    #region Climb Movement
    private void MovePlayerOverride(float speedMod)
    {
        Vector3 moveDir = visuals.transform.forward;
        controller.Move(moveSpeed / speedMod * moveDir.normalized * Time.fixedDeltaTime);

        if (!isClimbTransitioning)
        {
            ClimbCheck();
        }
    }
    #endregion
    #endregion
    #endregion

    #region Cam Limits
    #region Camera Rotation
    public void UpdateCameraCall(Vector2 inputVec)
    {
        input += inputVec;
    }

    /// <summary>
    /// Rotates the objects that the cinemachine camera is following.
    /// </summary>
    public void CameraRotation(ref float xTargetRot, ref float yTargetRot, Transform target, float bottomClamp, float topClamp, bool shouldSideClamp)
    {
        // Updates the target look values
        if (input.sqrMagnitude >= 0.01f && !isClimbTransitioning)
        {
            xTargetRot += input.x * Time.fixedDeltaTime * xSens;
            yTargetRot += -input.y * Time.fixedDeltaTime * ySens;
        }

        if (shouldSideClamp)
        {
            xTargetRot = ClampAngle(xTargetRot, cinemachineWallLeftClamp, cinemachineWallRightClamp);
        }
        else
        {
            // clamp our rotations so our values are limited 360 degrees
            xTargetRot = ClampAngle(xTargetRot, float.MinValue, float.MaxValue);
        }

        yTargetRot = ClampAngle(yTargetRot, bottomClamp, topClamp);

        // Updates cinemachine follow target rotation (essentially rotates the camera)
        target.rotation = Quaternion.Euler(yTargetRot, xTargetRot, 0.0f);

        //mainCam.transform.rotation = Quaternion.Euler(cinemachineTargetYRot, cinemachineTargetXRot, 0.0f);
        input = Vector2.zero;
    }

    /// <summary>
    /// Clamps the camera angle between given values.
    /// </summary>
    /// <param name="targetAngle">The current targeted angle.</param>
    /// <param name="angleMin">The current minimum angle for this axis.</param>
    /// <param name="angleMax">The current maximum angle for this axis.</param>
    /// <returns></returns>
    private float ClampAngle(float targetAngle, float angleMin, float angleMax)
    {
        float newAngle = Mathf.Clamp(targetAngle, angleMin, angleMax);
        if (newAngle < -360f) newAngle += 360f;
        if (newAngle > 360f) newAngle -= 360f;
        return newAngle;
    }
    #endregion

    public void SetCameras(float value)
    {
        cinemachineTargetXRot = value;
        cinemachineTargetYRot = 0;


        Vector2 rotationVec = new Vector2(0, 1);
        rotationVec.Normalize();

        targetRotation = Mathf.Atan2(rotationVec.x, rotationVec.y) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;

        // rotate to face input direction relative to camera position
        visuals.transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
    }
    #endregion

    public void KillPlayer()
    {
        PlayerController pc = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
        pc.UpdateBodyPart(PlayerController.activeController.PERSON);
        pc.ResetHand();
    }
}
