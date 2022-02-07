using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [SerializeField]
    private float yDegreesOfFreedom = 45;

    [SerializeField]
    private float xDegreesOfFreedom = 45;

    private float startingXRotation = 0;
    private float startingYRotation = 0;

    private float mouseSensitivity = 2.5f;
    float xRotation = 0f;
    float yRotation = 0f;

    private bool isGroundCeiling = false;

    [SerializeField]
    private GameObject eye;

    private Outline outline;

    public Outline OutlineScript
    {
        get => outline;
    }

    public GameObject Eye
    {
        get => gameObject;
    }

    private void Awake()
    {
        outline = GetComponentInChildren<Outline>();
        InitializeAngle();
    }

    public void InitializeAngle()
    {
        //eye.transform.rotation = transform.rotation;
        //startingXRotation = transform.eulerAngles.x;
        //startingYRotation = transform.eulerAngles.y;
        //xRotation = eye.transform.eulerAngles.y;
        //yRotation = eye.transform.eulerAngles.x;

        SetStartValues();

        if (transform.eulerAngles.x > 20 || transform.eulerAngles.x < -20)
        {
            isGroundCeiling = true;
            eye.transform.Rotate(new Vector3(-45, 0, 0));
            yRotation = -45;
        }
    }

    private void SetStartValues()
    {
        startingXRotation = eye.transform.localEulerAngles.x;
        startingYRotation = eye.transform.localEulerAngles.y;
        xRotation = eye.transform.localEulerAngles.y;
        yRotation = eye.transform.localEulerAngles.x;
    }

    public void Look(Vector2 input)
    {
        if (isGroundCeiling)
        {
            float mod = 1;

            if(yRotation > startingXRotation)
            {
                //mod = -1;
            }

            if(input.y > 0)
            {
                //transform.RotateAround(eye.transform.position, Vector3.down, input.y * 1);
            }
            else
            {
                //transform.RotateAround(eye.transform.position, Vector3.down, -1);
            }

            transform.RotateAround(eye.transform.position, Vector3.down, -input.x*mouseSensitivity*Time.deltaTime);
            //xRotation += input.x * mouseSensitivity * Time.deltaTime * mod;
            yRotation -= input.y * mouseSensitivity * Time.deltaTime;

            yRotation = Mathf.Clamp(yRotation, -90, 0);
        }
        else
        {
            xRotation += input.x * mouseSensitivity * Time.deltaTime;
            yRotation -= input.y * mouseSensitivity * Time.deltaTime;
        }

        xRotation = Mathf.Clamp(xRotation, -xDegreesOfFreedom+startingYRotation, xDegreesOfFreedom+startingYRotation);
        yRotation = Mathf.Clamp(yRotation, -yDegreesOfFreedom+startingXRotation, yDegreesOfFreedom+startingXRotation);

        //rotates the player and the camera.
        //eye.transform.rotation = Quaternion.Euler(yRotation, xRotation, 0f);
        eye.transform.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }

    private void OnBecameVisible()
    {
        outline.enabled = false;
    }

    private void OnBecameInvisible()
    {
        outline.enabled = true;
    }
}
