using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController controller;
    private float startingStepOffset;

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
    private CinemachineFreeLook cineCam;

    [SerializeField]
    private CinemachineFreeLook climbCineCam;

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

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        startingStepOffset = controller.stepOffset;
        cam = Camera.main.transform;
        mainCamBrain = Camera.main.GetComponent<CinemachineBrain>();
        //cineCam = transform.parent.gameObject.GetComponentInChildren<CinemachineFreeLook>();
        outline = GetComponentInChildren<Outline>();

        SetCameraSens();
    }

    private void SetCameraSens()
    {
        if (!PlayerPrefs.HasKey("X Sens Hand"))
        {
            PlayerPrefs.SetFloat("X Sens Hand", 200);
            PlayerPrefs.SetFloat("Y Sens Hand", 2);
        }

        if (PlayerPrefs.GetFloat("X Sens Hand") != 0 && PlayerPrefs.GetFloat("Y Sens Hand") != 0)
        {
            cineCam.m_XAxis.m_MaxSpeed = PlayerPrefs.GetFloat("X Sens Hand");
            cineCam.m_YAxis.m_MaxSpeed = PlayerPrefs.GetFloat("Y Sens Hand");
        }
    }

    private int curClimb = 0;
    private int curNorm = 100;
    public void SwitchCameras()
    {
        if(curNorm != cineCam.m_Priority)
        {
            cineCam.m_Priority = curNorm;
            climbCineCam.m_Priority = curClimb;
        }
        else
        {
            cineCam.m_Priority = 0;

            if(curClimb > 0)
            {
                climbCineCam.m_Priority = 1;
            }
            else
            {
                climbCineCam.m_Priority = -1;
            }
        }
    }

    #region Input Calls
    /// <summary>
    /// The player jumps if they are on the ground.
    /// </summary>
    public void Jump()
    {
        if (isClimbing)
        {
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
        IsGrounded();

        GravityCalculation();

        if (moveVec == Vector3.zero && !isClimbTransitioning)
        {
            anim.SetBool("Crawling", false);
        }

        if (moveVec.magnitude >= 0.1f && !mainCamBrain.IsBlending && !isClimbTransitioning)
        {
            RotatePlayer(moveVec);
        }
    }

    bool canFallOffWall = false;
    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    private void IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckPos.transform.position, groundCheckDist, groundMask);

        if(isGrounded && !isClimbing)
        {
            controller.stepOffset = startingStepOffset;
        }

        if(!isGrounded && !isClimbTransitioning && isClimbing && canFallOffWall)
        {
            jumpOffWall = true;
            isClimbing = false;
            jumpOffWallUp = Vector3.zero;
            StartCoroutine(NewFallOffWall());
            xMod = 0;
            xAngle = 0;
            Invoke("NotJumpOffWall", 1);
        }
    }

    [SerializeField]
    private GameObject visuals;
    private float xRotChange = 0;
    private int climbTurnRate = 450;
    private void RotatePlayer(Vector3 moveVec)
    {
        if (!isClimbing)
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
        else
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
            else if(moveVec.z == -1)  // 180
            {
                targetRot = 180;
            }
            else if (moveVec.z == 1)  // 0
            {
                targetRot = 0;

                if((int)(xRotChange / 180) % 2 == 1)
                {
                    targetRot = 360;
                }
            }
            #endregion

            #region Turn Two Keys
            if(moveVec.x >= 0.5f && moveVec.z >= 0.5f)
            {
                targetRot = 45;
            }
            else if(moveVec.x >= 0.5f && moveVec.z <= -0.5f)
            {
                targetRot = 135;
            }
            else if(moveVec.x <= -0.5f && moveVec.z >= 0.5f)
            {
                targetRot = 315;
            }
            else if(moveVec.x <= -0.5f && moveVec.z <= -0.5f)
            {
                targetRot = 225;
            }
            #endregion

            if ((int)(xRotChange / 360) != 0)
            targetRot += (int)(xRotChange / 360)*360;

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
                norm = xRotChange - (targetRot+360);
                offset = xRotChange - targetRot;
            }

            if(norm < 0)
            {
                norm = -norm;
            }
            if(offset < 0)
            {
                offset = -offset;
            }

            if (norm > offset)
            {
                if(targetRot != xRotChange)
                {
                    xRotChange += 360;
                }
                turnDir = -1;
            }

            if(xRotChange != targetRot)
            {
                float change = turnDir * climbTurnRate * Time.fixedDeltaTime;

                xRotChange += change;
                visuals.transform.RotateAround(visuals.transform.position, visuals.transform.up, change);
            }

            MovePlayer();
        }
    }

    private float xAngle = 0;

    private void MovePlayer()
    {
        Vector3 moveDir = visuals.transform.forward;
        Vector3 move = moveSpeed * moveDir.normalized * Time.fixedDeltaTime;

        controller.Move(move);

        if(move != Vector3.zero)
        {
            anim.SetBool("Crawling", true);
        }

        ClimbObject();
    }

    private void MovePlayerOverride()
    {
        Vector3 moveDir = visuals.transform.forward;
        controller.Move(moveSpeed/2 * moveDir.normalized * Time.fixedDeltaTime);
        controller.Move(-visuals.transform.up *5*Time.fixedDeltaTime);
    }

    #region Climbing
    private bool isClimbing = false;
    private bool hasClimbed = false;
    float xMod = 1;
    private float yAngle = 0;
    private Vector3 currentSurface = Vector3.zero;
    private void ClimbObject()
    {
        bool canClimb = Physics.Raycast(transform.position, visuals.transform.forward, out hit, climbDist, wallMask);

        if (canClimb && !jumpOffWall)
        {
            isClimbing = true;
            controller.stepOffset = 0;
            currentSurface = hit.normal;
            if (hit.normal.x > 0.1f || hit.normal.x < -0.1f || hit.normal.z > 0.1f || hit.normal.z < -0.1f)
                xAngle = -90;
            if (true)
            {

                if(hit.normal.x > 0.1f)
                {
                    xMod = 1.5f;
                }
                else if(hit.normal.x < -0.1f)
                {
                    xMod = -1.5f;
                }
                else
                {
                    if (hit.normal.z >= 0.5f)
                        xMod = hit.normal.z;
                    else
                    {
                        xMod = 0;
                    }
                }

                float camAngle = 0;

                if(hit.normal.y >= 0.5f || hit.normal.y <= -0.5f)
                {
                    controller.stepOffset = startingStepOffset;
                    xAngle = 0;
                    yAngle = 0;
                    camAngle = cineCam.m_XAxis.Value;
                    isClimbing = false;
                    climbCineCam.Priority = 0;
                    curClimb = 0;
                }
                else
                {
                    curClimb = 150;
                    climbCineCam.Priority = 150;
                }
                
                StartCoroutine(NewClimbTransition());
            }
        }
    }

    #region Climb Transition
    float climbTransitionSpeed = 1;
    private bool isClimbTransitioning = false;
    private IEnumerator NewClimbTransition()
    {
        isClimbTransitioning = true;

        anim.SetBool("Crawling", true);
        anim.SetBool("isJumping", false);
        xRotChange = 0;
        float t = 0;
        Quaternion newParentRot = Quaternion.Euler(xAngle, 0, 180 * xMod);
        Quaternion oldParentRot = visuals.transform.localRotation;

        float oldCamValue = climbCineCam.m_XAxis.Value;
        float newCamValue = 0;

        if (hit.normal.x < -0.8f)
        {
            newCamValue = 90;
            //climbCineCam.m_XAxis.Value = 90;
        }
        else if (hit.normal.x > 0.8f)
        {
            newCamValue = 270;
            //climbCineCam.m_XAxis.Value = 270;
        }
        else if (hit.normal.z > 0.8f)
        {
            newCamValue = 180;
            //climbCineCam.m_XAxis.Value = 180;
        }
        else
        {
            //climbCineCam.m_XAxis.Value = 0;
        }

        float camLerpCheck = oldCamValue - newCamValue;

        if(camLerpCheck < 0)
        {
            camLerpCheck = -camLerpCheck;
        }

        if(camLerpCheck > 190)
        {
            newCamValue -= 360;
        }

        RaycastHit tempHit;
        Physics.Raycast(visuals.transform.position, -visuals.transform.up, out tempHit, climbDist, groundMask);

        if (hit.normal.y == 1 || hit.normal.y == -1)
        {
            switch (tempHit.normal.x)
            {
                case 1:
                    cineCam.m_XAxis.Value = -90;
                    newParentRot *= Quaternion.Euler(0, 90, 0);
                    break;
                case -1:
                    cineCam.m_XAxis.Value = 90;
                    newParentRot *= Quaternion.Euler(0, -90, 0);
                    break;
                case 0:
                    if (tempHit.normal.z == -1)
                    {
                        cineCam.m_XAxis.Value = 0;
                        newParentRot *= Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        cineCam.m_XAxis.Value = 180;
                        newParentRot *= Quaternion.Euler(0, 180, 0);
                    }
                    break;
                default:
                    if (tempHit.normal.z > 0.8f)
                    {
                        cineCam.m_XAxis.Value = 180;
                        newParentRot *= Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        cineCam.m_XAxis.Value = 0;
                        newParentRot *= Quaternion.Euler(0, 180, 0);
                    }
                    break;
            }

            hasClimbed = false;
        }
        else
        {
            if (!hasClimbed)
            {
                oldCamValue = newCamValue;
                climbCineCam.m_XAxis.Value = newCamValue;
                hasClimbed = true;
            }
            else
            {
                if(visuals.transform.localEulerAngles.z >= 269)
                {
                    newParentRot = Quaternion.Euler(0, visuals.transform.localEulerAngles.y + 90, visuals.transform.localEulerAngles.z);
                    xRotChange += 90;
                }
                else
                {
                    newParentRot = Quaternion.Euler(0, visuals.transform.localEulerAngles.y - 90, visuals.transform.localEulerAngles.z);
                    xRotChange -= 90;
                }
            }
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        while (visuals.transform.localRotation != newParentRot)
        {
            t += Time.fixedDeltaTime * climbTransitionSpeed;
            t = Mathf.Clamp(t, 0, 1);
            visuals.transform.localRotation = Quaternion.Lerp(oldParentRot, newParentRot, t);

            if(hit.normal.y > -0.5f && hit.normal.y < 0.5f)
            {
                climbCineCam.m_XAxis.Value = Mathf.Lerp(oldCamValue, newCamValue, t);
            }

            yield return new WaitForFixedUpdate();

            if (t == 1)
            {
                visuals.transform.localRotation = newParentRot;
            }

            MovePlayerOverride();
        }

        canFallOffWall = true;
        isClimbTransitioning = false;
    }
    #endregion

    private IEnumerator NewFallOffWall()
    {
        isClimbTransitioning = true;
        anim.SetBool("Crawling", true);
        anim.SetBool("isJumping", false);
        xRotChange = 0;
        float t = 0;

        Quaternion newParentRot = Quaternion.Euler(0, 180, 0);
        Quaternion oldParentRot = visuals.transform.localRotation;
        curClimb = 0;
        climbCineCam.Priority = 0;
        controller.stepOffset = startingStepOffset;

        RaycastHit tempHit;
        Physics.Raycast(visuals.transform.position, -visuals.transform.up, out tempHit, climbDist, groundMask);
        if(currentSurface.x > 0.5f)
        {
            cineCam.m_XAxis.Value = -90;
            newParentRot = Quaternion.Euler(0, -90, 0);
        }
        else if(currentSurface.x < -0.5f)
        {
            cineCam.m_XAxis.Value = 90;
            newParentRot = Quaternion.Euler(0, 90, 0);
        }
        else if (currentSurface.z < -0.5f)
        {
            cineCam.m_XAxis.Value = 0;
            newParentRot = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            cineCam.m_XAxis.Value = 180;
        }

        hasClimbed = false;

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        while (visuals.transform.localRotation != newParentRot || mainCamBrain.IsBlending)
        {
            t += Time.fixedDeltaTime * 1;
            t = Mathf.Clamp(t, 0, 1);
            visuals.transform.localRotation = Quaternion.Lerp(oldParentRot, newParentRot, t);
            yield return new WaitForFixedUpdate();

            if (t == 1)
            {
                visuals.transform.localRotation = newParentRot;
            }

            MovePlayerOverride();
        }

        canFallOffWall = true;
        isClimbTransitioning = false;
        hasClimbed = false;
    }

    #region Old Climb
    /*
    bool isClimbTransitioning = false;
    float climbTransitionSpeed = 1;
    private IEnumerator ClimbTransition()
    {
        canFallOffWall = false;
        isClimbTransitioning = true;
        Vector3 pos = transform.position;
        transform.parent.position = pos;
        transform.position = pos;

        float t = 0;
        Quaternion newParentRot = Quaternion.Euler(xAngle, 0, 180 * xMod);
        Quaternion oldParentRot = transform.parent.rotation;
        Vector3 localUp = visuals.transform.up;
        localUp.x = xAngle / 90;
        Vector3 oldforward = visuals.transform.forward;
        float oldCineCamXValue = cineCam.m_XAxis.Value;
        RaycastHit tempHit;
        Physics.Raycast(visuals.transform.position, -visuals.transform.up, out tempHit, climbDist, groundMask);
        float newCineCamXValue = 0;

        if (hit.normal.y == 1 || hit.normal.y == -1)
        {
            switch (tempHit.normal.x)
            {
                case 1:
                    newCineCamXValue = 90;
                    break;
                case -1:
                    newCineCamXValue = -90;
                    break;
                case 0:
                    if(tempHit.normal.z == -1)
                    {
                        newCineCamXValue = 180;
                    }
                    break;
            }
        }
        else
        {
            Invoke("CanFallOffWall", 1.5f);
        }

            while (transform.parent.rotation != newParentRot)
        {
            t += Time.fixedDeltaTime*climbTransitionSpeed;
            t = Mathf.Clamp(t, 0, 1);
            transform.parent.rotation = Quaternion.Lerp(oldParentRot, newParentRot, t);
            RotatePlayer(Vector3.zero);
            cineCam.m_XAxis.Value = Mathf.Lerp(oldCineCamXValue, newCineCamXValue, t);
            yield return new WaitForFixedUpdate();

            if(t == 1)
            {
                transform.parent.rotation = Quaternion.Euler(xAngle, 0, 180 * xMod);
            }

            MovePlayerOverride();
        }

        isClimbTransitioning = false;
    }

    private IEnumerator ClimbJumpTransition()
    {
        xAngle = 0;
        xMod = 0;

        //isClimbTransitioning = true;
        Vector3 pos = transform.position;
        transform.parent.position = pos;
        transform.position = pos;

        float t = 0;
        Quaternion newParentRot = Quaternion.Euler(xAngle, 0, 180 * xMod);
        Quaternion oldParentRot = transform.parent.rotation;
        Vector3 localUp = visuals.transform.up;
        localUp.x = xAngle / 90;
        Vector3 oldforward = visuals.transform.forward;
        float oldCineCamXValue = cineCam.m_XAxis.Value;
        RaycastHit tempHit;
        Physics.Raycast(visuals.transform.position, -visuals.transform.up, out tempHit, climbDist, groundMask);
        float newCineCamXValue = 0;

        if (true)
        {    
            if(currentSurface.x > 0.1f)
            {
                jumpVel *= 1.05f;
                newCineCamXValue = 90;

                if (jumpOffWallUp == Vector3.zero)
                {
                    newCineCamXValue = -90;
                }
            }
            else if(currentSurface.x < -0.1f)
            {
                jumpVel *= 1.05f;
                newCineCamXValue = -90;

                if (jumpOffWallUp == Vector3.zero)
                {
                    newCineCamXValue = 90;
                }
            }
            else
            {
                if (currentSurface.z > -0.1f)
                {
                    jumpVel *= 1.05f;
                    newCineCamXValue = 180;

                    if (jumpOffWallUp == Vector3.zero)
                    {
                        newCineCamXValue = -180;
                    }
                }
                else if (jumpOffWallUp == Vector3.zero)
                {
                    newCineCamXValue = 0;
                }
            }

            /*
            switch (currentSurface.x)
            {
                case 1:
                    jumpVel *= 1.05f;
                    newCineCamXValue = 90;

                    if(jumpOffWallUp == Vector3.zero) {
                        newCineCamXValue = -90;
                    }
                    break;
                case -1:
                    jumpVel *= 1.05f;
                    newCineCamXValue = -90;

                    if (jumpOffWallUp == Vector3.zero)
                    {
                        newCineCamXValue = -90;
                    }
                    break;
                case 0:
                    if (currentSurface.z == -1)
                    {
                        jumpVel *= 1.05f;
                        newCineCamXValue = 180;

                        if (jumpOffWallUp == Vector3.zero)
                        {
                            newCineCamXValue = 0;
                        }
                    }
                    else if (jumpOffWallUp == Vector3.zero)
                    {
                        newCineCamXValue = 180;
                    }
                    break;
            }*/
    /*
        }

        while (transform.parent.rotation != newParentRot)
        {
            t += Time.fixedDeltaTime * climbTransitionSpeed;
            t = Mathf.Clamp(t, 0, 1);
            transform.parent.rotation = Quaternion.Lerp(oldParentRot, newParentRot, t);
            RotatePlayer(Vector3.zero);
            cineCam.m_XAxis.Value = Mathf.Lerp(oldCineCamXValue, newCineCamXValue, t);
            yield return new WaitForFixedUpdate();

            if (t == 1)
            {
                transform.parent.rotation = Quaternion.Euler(xAngle, 0, 180 * xMod);
            }

            //controller.Move(transform.up * 10 *Time.fixedDeltaTime);

            //MovePlayerOverride();
        }

        isClimbTransitioning = false;
    }

    private void CanFallOffWall()
    {
        canFallOffWall = true;
    }
    */
    #endregion
    #endregion

    /// <summary>
    /// Applyes gravity to the player.
    /// </summary>
    private void GravityCalculation()
    {
        if (jumpOffWall)
        {
            jumpVel += gravity * Time.fixedDeltaTime;
            controller.Move(jumpOffWallUp * Time.fixedDeltaTime * jumpVel);
            anim.SetBool("isJumping", true);
        }
        else if (isClimbing)
        {
            jumpVel = 0;
            controller.Move(-visuals.transform.up * Time.fixedDeltaTime);
        }
        else if (!isGrounded)
        {
            jumpVel += gravity * Time.fixedDeltaTime;
            anim.SetBool("isJumping", true);
        }
        else
        { 
            anim.SetBool("isJumping", false);
        }

        controller.Move(new Vector3(0, jumpVel, 0) * Time.fixedDeltaTime);
    }
    #endregion

    #region Cam Limits
    private void Update()
    {
        LimitCamera();
    }

    private void LimitCamera()
    {
        if (mainCamBrain.IsBlending || GameObject.Find("Player").GetComponent<PlayerController>().Current)
        {
            cineCam.m_XAxis.m_MaxSpeed = 0;
            cineCam.m_YAxis.m_MaxSpeed = 0;
        }
        else
        {
            cineCam.m_XAxis.m_MaxSpeed = PlayerPrefs.GetFloat("X Sens Hand");
            cineCam.m_YAxis.m_MaxSpeed = PlayerPrefs.GetFloat("Y Sens Hand");
        }
    }
    #endregion
}
