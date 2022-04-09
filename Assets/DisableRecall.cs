using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRecall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().CanRecall = false; ;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().CanRecall = true;
        }
    }
}
