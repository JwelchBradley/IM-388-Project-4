using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumpadChecker : MonoBehaviour, IInteractable
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

    NumpadChecker nc;

    [SerializeField]
    private TextMeshProUGUI numText;

    private bool Solved = false;

    private void Awake()
    {
        pmb = GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>();
        nc = pmb.KeyPad.GetComponent<NumpadChecker>();

        foreach(GameObject gm in activatableObjects)
        {
            activatables.Add(gm.GetComponentInChildren<Activatable>());
        }
    }

    public void DisplayInteractText()
    {
        pmb.PickUpText.text = interactMessage;
    }

    public void EnterNumber(int num)
    {
        if (Solved)
        {
            return;
        }

        numText.text += num;
        currentIndex++;

        if (currentIndex == code.Length)
        {
            Entered();
        }
    }

    private void Entered()
    {
        string entered = "";
        foreach(int num in code)
        {
            entered += num.ToString();
        }

        if(entered == numText.text)
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
        numText.text = "";
        currentIndex = 0;
    }

    public void Interact()
    {
        nc.Code = code;
        nc.Activatables = activatables;

        if (!pmb.KeyPad.activeInHierarchy || !Input.GetKeyDown(KeyCode.Mouse0))
        {
            pmb.KeyPad.SetActive(!pmb.KeyPad.activeInHierarchy);
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void ActivateObjects()
    {
        numText.text = "Correct";

        foreach(Activatable activatable in activatables)
        {
            activatable.ChangeObjectState();
        }

        Invoke("Close", 1f);
    }

    private void Close()
    {
        pmb.KeyPad.SetActive(false) ;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
