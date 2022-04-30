using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFloors : MonoBehaviour
{
    [SerializeField]
    private GameObject[] floorsToDisable;

    [SerializeField]
    private GameObject[] floorstToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach(GameObject obj in floorsToDisable)
            {
                obj.SetActive(false);
            }

            foreach(GameObject obj in floorstToEnable)
            {
                obj.SetActive(true);

            }
        }
    }
}
