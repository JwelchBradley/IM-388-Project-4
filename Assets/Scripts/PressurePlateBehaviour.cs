using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateBehaviour : MonoBehaviour
{
    BoxCollider bc;
    [SerializeField]
    private LayerMask playerMask;

    private RaycastHit hit;

    private bool heldDown = false;

    Vector3 startPos;
    Vector3 pushedPos;

    [SerializeField]
    [Tooltip("The door that this pressure plate opens")]
    private DoorBehaviour door;

    [SerializeField]
    float moveSpeed = 3;

    [SerializeField]
    float downDist = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.position;
        bc = GetComponent<BoxCollider>();
        pushedPos = bc.bounds.extents.y * downDist * -transform.up + transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool shouldHoldDown = Physics.BoxCast(transform.position - transform.up.normalized, bc.bounds.extents, transform.up, out hit, Quaternion.identity, 4, playerMask);

        if (shouldHoldDown && !hit.transform.gameObject.CompareTag("Hand") && !heldDown)
        {
            heldDown = true;
            StopAllCoroutines();
            StartCoroutine(ChangeState(true));
            door.ChangeState(1);
        }
        else if(!shouldHoldDown && heldDown)
        {
            heldDown = false;
            StopAllCoroutines();
            StartCoroutine(ChangeState(false));
            door.ChangeState(-1);
        }
    }

    private IEnumerator ChangeState(bool shouldChange)
    {
        Vector3 target = startPos;
        if (shouldChange)
        {
            target = pushedPos;
        }

        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }
}
