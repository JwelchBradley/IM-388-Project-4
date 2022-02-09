/*****************************************************************************
// File Name :         PauseMenuBehavior.cs
// Author :            Jacob Welch
// Creation Date :     28 August 2021
//
// Brief Description : Handles the pause menu and allows players to pause the game.
*****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenuBehavior : MenuBehavior
{
    #region Variables
    /// <summary>
    /// Holds true if the game is currently paused.
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// Enables and disables the pause feature.
    /// </summary>
    private bool canPause = false;

    private bool canClosePauseMenu = true;

    /// <summary>
    /// 
    /// </summary>
    public bool CanPause
    {
        get => canPause;
        set
        {
            canPause = value;
        }
    }

    [Space]
    [SerializeField]
    [Tooltip("The pause menu gameobject")]
    private GameObject pauseMenu = null;

    [Tooltip("The radial menu")]
    [SerializeField] private GameObject radialMenuPanel;

    public GameObject RadialMenuPanel
    {
        get => radialMenuPanel;
    }

    [Tooltip("The radial menu controller")]
    [SerializeField] RadialMenuController rmc;

    /// <summary>
    /// The radial menu controller.
    /// </summary>
    public RadialMenuController RMC
    {
        get => rmc;
    }

    [Tooltip("The pickup text")]
    [SerializeField] private TextMeshProUGUI pickUpText;

    /// <summary>
    /// The pickup text.
    /// </summary>
    public TextMeshProUGUI PickUpText
    {
        get => pickUpText;
    }

    [Tooltip("The UI for notes")]
    [SerializeField] private GameObject note;

    /// <summary>
    /// The UI for notes.
    /// </summary>
    public GameObject Note
    {
        get => note;
    }
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components.
    /// </summary>
    private void Awake()
    {
        StartCoroutine(WaitFadeIn());
    }

    private IEnumerator WaitFadeIn()
    {
        yield return new WaitForSeconds(crossfadeAnim.GetCurrentAnimatorStateInfo(0).length);

        canPause = true;
    }

    bool wasActiveBefore = false;
    /// <summary>
    /// If the player hits the pause game key, the game is paused.
    /// </summary>
    public void PauseGame()
    {
        // Opens pause menu and pauses the game
        if (canPause && canClosePauseMenu && !note.activeInHierarchy)
        {
            if (isPaused)
            {
                ResetPauseMenu();
            }

            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            AudioListener.pause = isPaused;
            Time.timeScale = Convert.ToInt32(!isPaused);

            if (isPaused)
            {
                if(Cursor.lockState == CursorLockMode.Confined)
                {
                    wasActiveBefore = true;
                }
                else
                {
                    wasActiveBefore = false;
                }
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else if(!wasActiveBefore)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void CanClosePauseMenu(bool canClose)
    {
        canClosePauseMenu = canClose;
    }

    /// <summary>
    /// Disables any panels of the pause menu that may be open.
    /// </summary>
    private void ResetPauseMenu()
    {
        /*
        foreach(GameObject panel in menuPanels)
        {
            panel.SetActive(false);
        }*/
    }

    /// <summary>
    /// Restarts the current level from the beginning.
    /// </summary>
    public void RestartLevel()
    {
        canPause = false;

        if (!hasLoadScreen)
        {
            StartCoroutine(LoadSceneHelper(SceneManager.GetActiveScene().name));
        }
    }
    #endregion
}