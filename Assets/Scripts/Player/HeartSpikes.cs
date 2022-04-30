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

    [SerializeField]
    private bool isHeart = true;

    [SerializeField]
    private int shouldPopUpIter = 0;

    private int startingPopIter = 0;

    [SerializeField]
    private EndSpikesController esc;

    private void Start()
    {
        if(isHeart)
        heartLocation.GetComponent<HeartPlaceLocation>().activation.AddListener(ChangeObjectState);
        else
        {
            startingPopIter = shouldPopUpIter;
            esc.spikeAction.AddListener(ResetEndSpikes);
        }

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

    private void ResetEndSpikes(bool shouldReset)
    {
        if (shouldReset)
        {
            if (shouldPopUpIter < 0)
            {
                shouldPopUpIter = 0;

                ChangeObjectState();
            }
        }
        else
        {
            ChangeObjectState();
        }

        if (shouldReset)
        {
            shouldPopUpIter = startingPopIter;
        }
    }

    public void ChangeObjectState()
    {
        if(shouldPopUpIter != 0)
        {
            shouldPopUpIter--;
            return;
        }
        else if (!isHeart && shouldPopUpIter == 0)
        {
            shouldPopUpIter--;
        }

        if (targetLocation == upperLocation.transform.position)
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
        float currentDelay = targetLocation == upperLocation.transform.position ? delayedMoveTime : 0f;
        yield return new WaitForSeconds(currentDelay);

        while (shouldMove)
        {
            if (Time.time - (changeTime + currentDelay) < spikeMoveTime)
            {
                if (targetLocation == upperLocation.transform.position)
                {
                    spike.position = Vector3.Lerp(lowerLocation.transform.position, targetLocation, (Time.time - (changeTime + currentDelay)) / spikeMoveTime);
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (isHeart)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<PlayerController>().KillPlayer();
            }
            else if (other.tag == "Hand")
            {
                other.GetComponent<ThirdPersonMovement>().KillPlayer();
            }
        }
        else
        {
            if (other.tag == "Hand")
            {
                other.GetComponent<ThirdPersonMovement>().KillPlayer();
            }

            if (other.tag == "Player" || other.tag == "Hand")
            {
                FindObjectOfType<PlayerController>().KillPlayer();

                StartCoroutine(ResetAllSpikes());
            }
        }
    }
    private IEnumerator ResetAllSpikes()
    {
        yield return new WaitForSeconds(1);
        esc.ResetSpikes();
    }
}
