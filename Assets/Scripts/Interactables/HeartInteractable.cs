using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartInteractable : Interactable
{
    [Tooltip("Parent of the heart object when held")]
    public GameObject heartParent;
    [Tooltip("Position of the heart object when held")]
    public Vector3 heartPosition;
    [Tooltip("Scale of heart when held")]
    public Vector3 heartScale;
    [Tooltip("Parent of the heart when in receptacle")]
    public GameObject receptacleParent;
    [Tooltip("Offset of the heart from a receptacle")]
    public Vector3 heartOffset;
    [Tooltip("Scale of heart in receptacle")]
    public Vector3 receptacleScale;
    [Tooltip("The heart controller")]
    public HeartController hc;

    /// <summary>
    /// The player controller
    /// </summary>
    private PlayerController pc;

    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
    }

    public override void Interact()
    {
        if (hc.enabled && (pc.CurrentActive == PlayerController.activeController.PERSON || pc.CurrentActive == PlayerController.activeController.HEART))
        {
            hc.enabled = false;
            hc.gameObject.transform.SetParent(heartParent.transform, true);
            hc.gameObject.transform.localPosition = heartPosition;
            hc.gameObject.transform.localScale = heartScale;
            pc.ActivateHeartStateFromBody();
        }
        else if (pc.HeartMesh.activeSelf && (pc.CurrentActive == PlayerController.activeController.PERSON || pc.CurrentActive == PlayerController.activeController.HEART))
        {
            hc.enabled = true;
            hc.gameObject.transform.SetParent(receptacleParent.transform, true);
            hc.gameObject.transform.localPosition = heartOffset;
            hc.gameObject.transform.localScale = receptacleScale;
        }
    }
}
