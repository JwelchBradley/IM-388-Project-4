using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverBehaviour : Interactable
{
    [Header("Scene Settings")]
    [SerializeField]
    [Tooltip("The door that this pressure plate opens")]
    private DoorBehaviour[] door;

    [SerializeField]
    [Tooltip("Set to -1 if the second state is the correct one")]
    int startingIter = 1;

    [SerializeField]
    [Tooltip("True means the lever can be actiavted")]
    bool canActivate = true;

    public bool CanActivate
    {
        set
        {
            canActivate = value;
        }
    }

    [SerializeField]
    [Tooltip("Holds true if the lever can be switched back to off")]
    bool canUnactivate = false;

    [Header("Lever Objects")]
    [SerializeField]
    private GameObject lever;

    [SerializeField]
    private GameObject onLight;

    [SerializeField]
    private GameObject offLight;

    private bool activated = false;

    private Vector3 startPos;
    private Quaternion startRotation;

    private Vector3 pushedPos;
    private Quaternion pushedRotation;

    private float timeToMove = 0.5f;

    float t = 0;

    #region Level 3
    [HideInInspector]
    public Level3LeverController l3lc;

    private LeverBehaviour lb;
    #endregion

    private void Start()
    {
        startPos = lever.transform.localPosition;
        startRotation = lever.transform.localRotation;

        pushedPos = new Vector3(-startPos.x, startPos.y, 0);
        pushedRotation = lever.transform.localRotation * Quaternion.Euler(new Vector3(0, 0, -40));

        lb = GetComponent<LeverBehaviour>();

        if(startingIter == -1)
        {
            onLight.SetActive(true);
            offLight.SetActive(false);

            foreach (DoorBehaviour db in door)
            {
                db.ChangeState(1);
            }
        }
    }

    public override void DisplayInteractText()
    {
        bool displayText = true;

        if(activated && !canUnactivate)
        {
            displayText = false;
        }

        if(displayText && canActivate)
        base.DisplayInteractText();
    }

    public override void Interact()
    {
        bool allowInteract = true;

        if(activated && !canUnactivate)
        {
            allowInteract = false;
        }
        else if(!activated && !canUnactivate)
        {
            text.text = "";
        }

        if (allowInteract && canActivate)
        {
            offLight.SetActive(activated);

            activated = !activated;

            onLight.SetActive(activated);

            int change = -1;

            if (activated)
            {
                change = 1;
            }

            StopAllCoroutines();
            StartCoroutine(ChangeState(change));
            foreach (DoorBehaviour db in door)
            {
                db.ChangeState(change * startingIter);
            }

            if(l3lc != null)
            l3lc.CompareIndex(lb);
        }
    }

    public IEnumerator L3LCUnActivate()
    {
        yield return new WaitForSeconds(1);
        StopAllCoroutines();
        activated = false;
        offLight.SetActive(true);
        onLight.SetActive(false);
        StartCoroutine(ChangeState(-1));
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
