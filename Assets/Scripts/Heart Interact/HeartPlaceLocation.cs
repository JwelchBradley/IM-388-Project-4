using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPlaceLocation : MonoBehaviour
{
    IHeartActivatable[] heartActivatables = new IHeartActivatable[0];

    public void UpdateHeartInteractables()
    {
        foreach(IHeartActivatable heartActivatable in heartActivatables)
        {
            heartActivatable.ChangeObjectState();
        }
    }
}
