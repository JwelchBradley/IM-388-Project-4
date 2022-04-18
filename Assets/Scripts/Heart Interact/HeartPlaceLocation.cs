using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeartPlaceLocation : Interactable
{
    public UnityEvent activation;

    [SerializeField]
    private Transform heartPlaceLocation;

    private HeartController hc;

    [SerializeField]
    private string heartPlacedHereMessage = "Press F to pick up the heart";

    [SerializeField]
    private bool startOnAwake = false;

    [SerializeField]
    private GameObject HeartMesh;

    protected override void Awake()
    {
        base.Awake();

        if (startOnAwake)
        {
            HeartMesh.transform.position = heartPlaceLocation.position;
            HeartMesh.SetActive(true);
            hc = HeartMesh.GetComponent<HeartController>();
            hc.HPC = this;
            hc.StartCoroutine(hc.Switch());
        }
    }

    public override void DisplayInteractText()
    {
        base.DisplayInteractText();

        foreach (PlayerController.activeController ac in displayTextControllers)
        {
            if (ac.Equals(pc.CurrentActive))
            {
                if (hc != null)
                {
                    text.text = heartPlacedHereMessage;
                }
            }
        }
    }

    public override void Interact()
    {
        foreach (PlayerController.activeController ac in displayTextControllers)
        {
            if (ac.Equals(pc.CurrentActive))
            {
                if (!pc.HeartMesh.activeInHierarchy)
                {
                    pc.HeartMesh.transform.position = heartPlaceLocation.position;
                    pc.HeartMesh.SetActive(true);
                    hc = pc.HeartMesh.GetComponent<HeartController>();
                    hc.HPC = this;
                    hc.StartCoroutine(hc.Switch());
                    DisplayInteractText();
                }
                else
                {
                    hc = null;
                    pc.HeartMesh.SetActive(false);
                    DisplayInteractText();
                }
            }
        }
    }

    public void Activate()
    {
        activation.Invoke();
    }
}
