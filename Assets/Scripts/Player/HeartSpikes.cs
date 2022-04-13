using System.Collections;
using UnityEngine;

public class HeartSpikes : MonoBehaviour, Activatable
{
    [Header("Speed")]
    [Tooltip("The time it takes for the spikes to move into a new position")]
    public float spikeMoveTime;
    [Tooltip("The delay before the spikes come up")]
    public float delayedMoveTime;

    [Header("Object References")]
    [Tooltip("The location where spikes are shown to the player")]
    public GameObject upperLocation;
    [Tooltip("The location where the spikes are hidden")]
    public GameObject lowerLocation;
    [SerializeField]
    [Tooltip("The spike object to move")]
    private Transform spike;

    [Header("Scene Settings")]
    [Tooltip("Whether the spikes start in their upper location")]
    public bool startUp;

    [SerializeField]
    private GameObject heartLocation;

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
        heartLocation.GetComponent<HeartPlaceLocation>().activation.AddListener(ChangeObjectState);

        if(startUp)
        {
            targetLocation = upperLocation.transform.position;
            spike.position = targetLocation;
        }
        else
        {
            targetLocation = lowerLocation.transform.position;
            spike.position = targetLocation;
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
        changeTime = Time.time;

        StartCoroutine(MoveSpike());
    }

    private IEnumerator MoveSpike()
    {
        bool shouldMove = true;
        while (shouldMove)
        {
            if (Time.time - changeTime < spikeMoveTime + (targetLocation == upperLocation.transform.position ? delayedMoveTime : 0f))
            {
                if (targetLocation == upperLocation.transform.position)
                {
                    spike.position = Vector3.Lerp(lowerLocation.transform.position, targetLocation, (Time.time - changeTime) / (spikeMoveTime + delayedMoveTime));
                }
                else
                {
                    spike.position = Vector3.Lerp(upperLocation.transform.position, targetLocation, (Time.time - changeTime) / spikeMoveTime);
                }

            }
            else
            {
                shouldMove = false;
                spike.position = targetLocation;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /*
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Time.unscaledTime - changeTime < spikeMoveTime + (targetLocation == upperLocation.transform.position ? delayedMoveTime : 0f))
        {
            if(targetLocation == upperLocation.transform.position)
            {
                spike.position = Vector3.Lerp(lowerLocation.transform.position, targetLocation, (Time.unscaledTime - changeTime) / (spikeMoveTime + delayedMoveTime));
            }
            else
            {
                spike.position = Vector3.Lerp(upperLocation.transform.position, targetLocation, (Time.unscaledTime - changeTime) / spikeMoveTime);
            }
            
        }
        else
        {
            spike.position = targetLocation;
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().KillPlayer();
        }
        else if(other.tag == "Hand") 
        {
            other.GetComponent<ThirdPersonMovement>().KillPlayer();
        }
    }
}
