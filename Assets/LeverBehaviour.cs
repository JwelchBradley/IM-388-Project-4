using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverBehaviour : Interactable
{
    [SerializeField]
    [Tooltip("The door that this pressure plate opens")]
    private DoorBehaviour[] door;

    [SerializeField]
    private GameObject lever;

    private bool activated = false;

    private Vector3 startPos;
    private Quaternion startRotation;

    private Vector3 pushedPos;
    private Quaternion pushedRotation;

    private float timeToMove = 0.5f;

    float t = 0;

    [SerializeField]
    [Tooltip("Set to -1 if the second state is the correct one")]
    int startingIter = 1;

    private void Start()
    {
        startPos = lever.transform.localPosition;
        startRotation = lever.transform.localRotation;

        pushedPos = new Vector3(-startPos.x, startPos.y, 0);
        pushedRotation = lever.transform.localRotation * Quaternion.Euler(new Vector3(0, 0, -40));

        if(startingIter == -1)
        {
            foreach (DoorBehaviour db in door)
            {
                db.ChangeState(1);
            }
        }
    }

    public override void Interact()
    {
        activated = !activated;

        //anim.SetBool("Active", activated);

        int change = -1;

        if (activated)
        {
            change = 1;
        }

        StopAllCoroutines();
        StartCoroutine(ChangeState(change));
        foreach(DoorBehaviour db in door)
        {
            db.ChangeState(change*startingIter);
        }
    }

    private IEnumerator ChangeState(int mod)
    {
        Quaternion target = pushedRotation;

        if(mod == -1)
        {
            target = startRotation;
        }

        while (transform.localRotation != target)
        {
            t += Time.deltaTime*mod/timeToMove;

            t = Mathf.Clamp(t, 0, 1);

            lever.transform.localPosition = Vector3.Lerp(startPos, pushedPos, t);
            lever.transform.localRotation = Quaternion.Lerp(startRotation, pushedRotation, t);

            yield return new WaitForEndOfFrame();
        }
    }
}
