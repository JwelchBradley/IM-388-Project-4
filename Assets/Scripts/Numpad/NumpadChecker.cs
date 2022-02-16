using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumpadChecker : Interactable
{
    [SerializeField] private int[] code;

    public int[] Code
    {
        set
        {
            code = value;
        }
    }

    private int currentIndex = 0;

    [SerializeField]
    private GameObject[] activatableObjects;

    private List<Activatable> activatables = new List<Activatable>();

    public List<Activatable> Activatables
    {
        set
        {
            activatables = value;
        }
    }

    private PauseMenuBehavior pmb;

    [SerializeField]
    private string interactMessage = "Enter Keycode";

    [SerializeField]
    private string solvedInteractMessage = "This has already been solved";

    private bool Solved = false;

    private string currentEntered = "";

    public string CurrentEntered
    {
        get => currentEntered;
    }

    private NumpadController nc;

    private void Start()
    {
        pmb = GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>();
        nc = pmb.KeyPad.GetComponent<NumpadController>();

        foreach(GameObject gm in activatableObjects)
        {
            activatables.Add(gm.GetComponentInChildren<Activatable>());
        }
    }

    public override void DisplayInteractText()
    {
        base.DisplayInteractText();

        if (Solved)
        {
            text.text = solvedInteractMessage;
        }
    }

    public string EnterNumber(int num)
    {
        if (Solved)
        {
            return "Correct";
        }

        //numText.text += num;
        currentEntered += num;
        currentIndex++;

        if (currentIndex == code.Length)
        {
            Entered();
        }

        return currentEntered;
    }

    private void Entered()
    {
        string entered = "";
        foreach(int num in code)
        {
            entered += num.ToString();
        }

        if(entered == currentEntered)
        {
            ActivateObjects();
        }
        else
        {
            Reset();
        }
    }

    private void Reset()
    {
        currentEntered = "";
        currentIndex = 0;
    }

    public override void Interact()
    {
        if (Solved)
        {
            return;
        }

        bool canInteract = false;

        foreach (PlayerController.activeController ac in displayTextControllers)
        {
            if (ac.Equals(pc.CurrentActive))
            {
                canInteract = true;
                break;
            }
        }
        if (!canInteract || Time.timeScale == 0)
            return;

        if (!pmb.KeyPad.activeInHierarchy || !Input.GetKeyDown(KeyCode.Mouse0))
        {
            nc.CurrentNumpad = GetComponent<NumpadChecker>();
            pmb.KeyPad.SetActive(!pmb.KeyPad.activeInHierarchy);
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void ActivateObjects()
    {
        currentEntered = "Correct";
        Solved = true;

        foreach(Activatable activatable in activatables)
        {
            activatable.ChangeObjectState();
        }

        Invoke("Close", 0.5f);
    }

    private void Close()
    {
        pmb.KeyPad.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
