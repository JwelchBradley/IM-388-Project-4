/*****************************************************************************
// File Name :         PanelDisabler.cs
// Author :            Jacob Welch
// Creation Date :     28 August 2021
//
// Brief Description : Disables this panel if the player presses escape.
*****************************************************************************/
using UnityEngine;

public class PanelDisabler : MonoBehaviour
{
    PauseMenuBehavior pauseMenu;

    private void Awake()
    {
        GameObject pause = GameObject.Find("Pause Menu Templates Canvas");

        if (pause != null)
        {
            pauseMenu = pause.GetComponent<PauseMenuBehavior>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseMenu != null)
            {
                pauseMenu.CanClosePauseMenu(true);
            }

            if (pauseMenu != null && pauseMenu.KeyPad.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            gameObject.SetActive(false);
        }
    }
}
