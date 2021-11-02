using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController controller;

    private Transform cam;

    private Vector3 moveVec;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVeloctiy;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if(moveVec.magnitude >= 0.1f)
        {
            RotatePlayer();
        }   
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
        controller.Move(speed * moveDir.normalized * Time.fixedDeltaTime);
    }

    public void OnJump()
    {
        if (controller.isGrounded)
        {

        }
    }

    public void MovePlayer(Vector2 input)
    {
        //Vector2 inputVec = input.Get<Vector2>();

        moveVec = new Vector3(input.x, 0, input.y).normalized;
    }
}
