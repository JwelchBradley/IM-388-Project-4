using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumpadController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI numText;

    private NumpadChecker currentNumpad;

    public NumpadChecker CurrentNumpad
    {
        set
        {
            currentNumpad = value;
            InitializeNumpad();
        }
    }

    private void InitializeNumpad()
    {
        numText.text = currentNumpad.CurrentEntered;
    }

    public void EnterNumber(int num)
    {
        numText.text = currentNumpad.EnterNumber(num);
    }
}
