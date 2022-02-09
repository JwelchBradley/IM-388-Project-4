using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateGroupController : MonoBehaviour
{
    [HideInInspector]
    public List<PressurePlateBehaviour> pressurePlate = new List<PressurePlateBehaviour>();

    [SerializeField]
    private LeverBehaviour lever;

    private int numActivated = 0;

    private bool hasActivated = false;

    public void UpdateActivated(int mod)
    {
        numActivated += mod;

        if(numActivated == pressurePlate.Count)
        {
            AllowLever(true);
            hasActivated = true;
        }
        else if (hasActivated)
        {
            AllowLever(false);
        }
    }

    private void AllowLever(bool allow)
    {
        lever.CanActivate = allow;

        foreach(PressurePlateBehaviour ppb in pressurePlate)
        {
            ppb.AllowEmission(allow);
        }
    }
}
