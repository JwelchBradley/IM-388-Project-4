using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPlaceLocation : Interactable
{
    [SerializeField]
    private GameObject[] activatableObjects;

    private List<Activatable> activatables = new List<Activatable>();

    [SerializeField]
    private Transform heartPlaceLocation;

    private HeartController hc;

    [SerializeField]
    private string heartPlacedHereMessage = "Press F to pick up the heart";

    private void Start()
    {
        foreach(GameObject activatable in activatableObjects)
        {
            activatables.Add(activatable.GetComponent<Activatable>());
        }
    }

    public override void DisplayInteractText()
    {
        base.DisplayInteractText();

        if(hc != null)
        {
            text.text = heartPlacedHereMessage;
        }
    }

    public override void Interact()
    {
        if (!pc.HeartMesh.activeInHierarchy)
        {
            pc.HeartMesh.transform.position = heartPlaceLocation.position;
            pc.HeartMesh.SetActive(true);
            hc = pc.HeartMesh.GetComponent<HeartController>();
            hc.heartActivatables = activatables;
            hc.StartCoroutine(hc.Switch());
        }
        else
        {
            hc = null;
            pc.HeartMesh.SetActive(false);
        }
    }
}
