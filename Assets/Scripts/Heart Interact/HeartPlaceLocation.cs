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

    private AudioSource heartbeat;

    protected override void Awake()
    {
        base.Awake();
        heartbeat.enabled = false;

        if (startOnAwake)
        {
            HeartMesh.transform.position = heartPlaceLocation.position;
            HeartMesh.SetActive(true);
            hc = HeartMesh.GetComponent<HeartController>();
            hc.HPC = this;
            heartbeat.enabled = true;
            hc.StartCoroutine(hc.Switch());
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
            hc.HPC = this;
            hc.StartCoroutine(hc.Switch());
            DisplayInteractText();
            heartbeat.enabled = true;
        }
        else
        {
            hc = null;
            pc.HeartMesh.SetActive(false);
            DisplayInteractText();
            heartbeat.enabled = false;
        }
    }

    public void Activate()
    {
        activation.Invoke();
    }
}
