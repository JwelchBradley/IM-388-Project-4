using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuController : MonoBehaviour
{
    Vector2 mousePos;
    Camera mainCam;

    Image im;

    public Image Im
    {
        get => im;
    }

    [SerializeField]
    Sprite normal;
    [SerializeField]
    Sprite person;
    [SerializeField]
    Sprite hand;
    [SerializeField]
    Sprite eye;
    [SerializeField]
    Sprite heart;
    [SerializeField]
    Sprite intestines;
    [SerializeField]
    Sprite mouth;
    [SerializeField]
    Sprite ear;

    [HideInInspector]
    public PlayerController.activeController currentHovered;

    // Start is called before the first frame update
    void Awake()
    {
        mainCam = Camera.main;
        im = GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angle = FindAngle();
        if (angle >= 45 && angle < 180)
        {
            currentHovered = PlayerController.activeController.EYE;
            im.sprite = eye;
        }
        else if (angle < 270 && angle > 180)
        {
            currentHovered = PlayerController.activeController.HAND;
            im.sprite = hand;
        }
        else
        {
            currentHovered = PlayerController.activeController.PERSON;
            im.sprite = person;
        }

        /*
        if (angle > 90 && angle < 180)
        {
            currentHovered = PlayerController.activeController.EYE;
            im.sprite = eye;
        }
        else if (angle >= 180 && angle < 270)
        {
            currentHovered = PlayerController.activeController.HAND;
            im.sprite = hand;
        }
        else if (angle >= 270 && angle < 360)
        {
            currentHovered = PlayerController.activeController.PERSON;
            im.sprite = person;
        }
        else
        {
            currentHovered = PlayerController.activeController.MOUTH;
            im.sprite = mouth;
        }*/
    }

    /// <summary>
    /// Finds the cursor is to the center of the circle.
    /// </summary>
    /// <returns>Angle of cursor to center of circle.</returns>
    private float FindAngle()
    {
        // Gets mouse position
        mousePos = Input.mousePosition;
        mousePos = mainCam.ScreenToViewportPoint(mousePos);

        // Sets position to be relative to the menu
        mousePos -= new Vector2(0.5f, 0.5f);

        // Sets vector to have magnitude of one
        mousePos.Normalize();

        // Returns value in angle form
        float angle = Mathf.Atan2(mousePos.y, -mousePos.x) * 180 / Mathf.PI;

        // Makes angles between 0-360 instead of -180 - 180
        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }
}
