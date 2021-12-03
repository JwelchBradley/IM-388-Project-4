using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    BoxCollider bc;

    Vector3 startPos;
    Vector3 openPos;

    public AudioClip doorSnd;

    [SerializeField]
    float moveSpeed = 3;

    [SerializeField]
    [Tooltip("The amount of objectives needed to be completed before this door opens")]
    [Range(1, 10)]
    private int objectivesNeeded = 1;

    [HideInInspector]
    public int currentObjectTally = 0;

    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.position;
        bc = GetComponent<BoxCollider>();
        openPos = bc.bounds.extents.y * 2 * transform.up + transform.position;
    }

    public void ChangeState(int increment)
    {
        currentObjectTally += increment;
        if (currentObjectTally >= objectivesNeeded)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeStateHelper(true));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ChangeStateHelper(false));
        }
    }

    private IEnumerator ChangeStateHelper(bool open)
    {
        Vector3 target = startPos;
        if (open)
        {
            target = openPos;

            AudioSource.PlayClipAtPoint(doorSnd, target, 1000);
        }

        while(transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }
}
