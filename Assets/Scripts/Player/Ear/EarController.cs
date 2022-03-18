using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarController : MonoBehaviour
{
    [Tooltip("The audio that plays for the ear")]
    public AudioSource earAudio;
    [Tooltip("The object with the audio listener")]
    public Transform listeningObject;
    [Tooltip("The upgrade type so that trigger areas can change the clip depending on the upgrade")]
    public Transform type = 0;

    /// <summary>
    /// Thev initial parent of the listeningObject
    /// </summary>
    private Transform listeningObjectParent;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject ear = GameObject.FindGameObjectWithTag("Ear");
        earAudio = ear.GetComponent<AudioSource>();
        listeningObject = ear.GetComponent<Transform>();

        earAudio.enabled = true;
        listeningObjectParent = listeningObject.parent;
        listeningObject.SetParent(transform);
        listeningObject.localPosition = Vector3.zero;
    }

    private void OnDestroy()
    {
        earAudio.enabled = false;
        listeningObject.SetParent(listeningObjectParent);
        listeningObject.localPosition = Vector3.zero;
    }
}
