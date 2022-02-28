using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpikes : MonoBehaviour, Activatable
{
    [Tooltip("The time it takes for the spikes to move into a new position")]
    public float spikeMoveTime;
    [Tooltip("The delay before the spikes come up")]
    public float delayedMoveTime;
    [Tooltip("The location where spikes are shown to the player")]
    public GameObject upperLocation;
    [Tooltip("The location where the spikes are hidden")]
    public GameObject lowerLocation;
    [Tooltip("Whether the spikes start in their upper location")]
    public bool startUp;

    /// <summary>
    /// The current location target
    /// </summary>
    private Vector3 targetLocation;

    /// <summary>
    /// The time the spike target last changed
    /// </summary>
    private float changeTime = 0f;

    private void Start()
    {
        if(startUp)
        {
            targetLocation = upperLocation.transform.position;
            transform.position = targetLocation;
        }
        else
        {
            targetLocation = lowerLocation.transform.position;
            transform.position = targetLocation;
        }
    }

    public void ChangeObjectState()
    {
        if(targetLocation == upperLocation.transform.position)
        {
            targetLocation = lowerLocation.transform.position;
        }
        else
        {
            targetLocation = upperLocation.transform.position;
        }
        changeTime = Time.unscaledTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.unscaledTime - changeTime < spikeMoveTime + (targetLocation == upperLocation.transform.position ? delayedMoveTime : 0f))
        {
            if(targetLocation == upperLocation.transform.position)
            {
                transform.position = Vector3.Lerp(lowerLocation.transform.position, targetLocation, (Time.unscaledTime - changeTime) / (spikeMoveTime + delayedMoveTime));
            }
            else
            {
                transform.position = Vector3.Lerp(upperLocation.transform.position, targetLocation, (Time.unscaledTime - changeTime) / spikeMoveTime);
            }
            
        }
        else
        {
            transform.position = targetLocation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().KillPlayer();
        }
    }
}
