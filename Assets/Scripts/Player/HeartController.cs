using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    [Tooltip("Switches constantly between true and false. Objects polling this will change when this switch is changed.")]
    [HideInInspector] public bool heartbeatSwitch = false;
    [Tooltip("Time it takes for the heart to beat.")]
    public float heartbeatTime = 1f;

    [HideInInspector]
    public List<Activatable> heartActivatables = new List<Activatable>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator Switch()
    {
        while (true)
        {
            yield return new WaitForSeconds(heartbeatTime);
            UpdateHeartInteractables();
            //heartbeatSwitch = !heartbeatSwitch;
        }
    }

    public void UpdateHeartInteractables()
    {
        foreach (Activatable heartActivatable in heartActivatables)
        {
            heartActivatable.ChangeObjectState();
        }
    }
}
