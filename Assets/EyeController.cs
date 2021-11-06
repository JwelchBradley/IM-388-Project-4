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

    private float mouseSensitivity = 10;
    float xRotation = 0f;
    float yRotation = 0f;

    private bool isGroundCeiling = false;

    private void Awake()
    {
        InitializeAngle(transform.rotation);
    }

    public void InitializeAngle(Quaternion rotation)
    {
        if(transform.eulerAngles.x > 20 || transform.eulerAngles.x < -20)
        {
            isGroundCeiling = true;
            Debug.Log(transform.eulerAngles.x);
        }
        transform.rotation = rotation;
        startingXRotation = transform.eulerAngles.x;
        startingYRotation = transform.eulerAngles.y;
    }

    public void Look(Vector2 input)
    {
        if (isGroundCeiling)
        {
            float mod = 1;

            if(yRotation < startingXRotation)
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
        transform.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }


}
