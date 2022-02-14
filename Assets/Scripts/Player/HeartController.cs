using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    [Tooltip("Switches constantly between true and false. Objects polling this will change when this switch is changed.")]
    [HideInInspector] public bool heartbeatSwitch = false;
    [Tooltip("Time it takes for the heart to beat.")]
    public float heartbeatTime = 1f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Switch");
    }

    IEnumerator Switch()
    {
        while (true)
        {
            heartbeatSwitch = !heartbeatSwitch;
            yield return new WaitForSeconds(heartbeatTime);
        }
    }
}
