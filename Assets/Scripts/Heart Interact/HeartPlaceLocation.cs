using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeartPlaceLocation : Interactable
{
    /*
    [SerializeField]
    private GameObject[] activatableObjects;
    */
    public UnityEvent activation;

    //private List<Activatable> activatables = new List<Activatable>();

    [SerializeField]
    private Transform heartPlaceLocation;

    private HeartController hc;

    [SerializeField]
    private string heartPlacedHereMessage = "Press F to pick up the heart";

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
            hc.HPC = this;
            hc.StartCoroutine(hc.Switch());
        }
        else
        {
            hc = null;
            pc.HeartMesh.SetActive(false);
        }
    }

    public void Activate()
    {
        activation.Invoke();
    }
}
