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

    private float mouseSensitivity = 5;
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

        startingXRotation = eye.transform.localEulerAngles.x;
        startingYRotation = eye.transform.localEulerAngles.y;
        xRotation = eye.transform.localEulerAngles.y;
        yRotation = eye.transform.localEulerAngles.x;

        if (yRotation > 20 || yRotation < -20)
        {
            isGroundCeiling = true;
        }
    }

    public void Look(Vector2 input)
    {
        if (isGroundCeiling)
        {
            float mod = 1;

            if(yRotation > startingXRotation)
            {
                mod = -1;
            }

            xRotation += input.x * mouseSensitivity * Time.deltaTime * mod;
            yRotation -= input.y * mouseSensitivity * Time.deltaTime;
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
