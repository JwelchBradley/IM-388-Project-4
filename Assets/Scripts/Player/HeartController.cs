using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    [Tooltip("Switches constantly between true and false. Objects polling this will change when this switch is changed.")]
    [HideInInspector] public bool heartbeatSwitch = false;
    [Tooltip("Time it takes for the heart to beat.")]
    public float heartbeatTime = 1f;

    private HeartPlaceLocation hpc;

    private AudioSource heartBeat;

    public HeartPlaceLocation HPC
    {
        set
        {
            hpc = value;
        }
    }

    private void Awake()
    {
        heartBeat = GetComponent<AudioSource>();
    }

    public IEnumerator Switch()
    {
        while (true)
        {
            yield return new WaitForSeconds(heartbeatTime);
            heartBeat.PlayOneShot(heartBeat.clip);
            UpdateHeartInteractables();
            //heartbeatSwitch = !heartbeatSwitch;
        }
    }

    public void UpdateHeartInteractables()
    {
        hpc.Activate();
        /*
        foreach (Activatable heartActivatable in heartActivatables)
        {
            heartActivatable.ChangeObjectState();
        }*/
    }
}
