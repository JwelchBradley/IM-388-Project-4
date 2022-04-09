using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
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

    private AudioSource aud;

    [SerializeField]
    private AudioClip clickSound;

    [SerializeField]
    private AudioClip correctSound;

    [SerializeField]
    private AudioClip incorrectSound;

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

    [SerializeField]
    private GameObject[] Wire;
    List<Material> OriginalColor = new List<Material>();

    #region Level 3
    [HideInInspector]
    public Level3LeverController l3lc;

    private LeverBehaviour lb;

    private Light greenYellowLight;

    [SerializeField]
    private bool yellowCheck = true;
    #endregion

#if UNITY_EDITOR
    private void Reset()
    {
        AudioMixer mixer = Resources.Load("AudioMaster") as AudioMixer;
        aud = GetComponent<AudioSource>();
        aud.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        aud.playOnAwake = false;
        aud.spatialBlend = 1;
        aud.rolloffMode = AudioRolloffMode.Custom;
        aud.minDistance = 50;
        aud.maxDistance = 200;
    }
#endif

    private void Start()
    {
        foreach (GameObject wire in Wire)
        {
            OriginalColor.Add(wire.GetComponent<Renderer>().material);
        }

        foreach (Material mat in OriginalColor)
        {
            mat.EnableKeyword("_EMISSION");
        }

        //OriginalColor.EnableKeyword("_EMISSION");
        foreach (Material mat in OriginalColor)
        {
            mat.SetColor("_EmissionColor", Color.black);
        }

        startPos = lever.transform.localPosition;
        startRotation = lever.transform.localRotation;

        pushedPos = new Vector3(-startPos.x, startPos.y, 0);
        pushedRotation = lever.transform.localRotation * Quaternion.Euler(new Vector3(0, 0, -40));

        lb = GetComponent<LeverBehaviour>();

        greenYellowLight = onLight.GetComponentInChildren<Light>();

        aud = GetComponent<AudioSource>();

        if (startingIter == -1)
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

        //aud.Play();

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

            if (activated && yellowCheck)
            {
                greenYellowLight.color = Color.yellow;

                Invoke("TurnGreen", 1.01f);

                aud.PlayOneShot(clickSound);
            }
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

    private void TurnGreen()
    {
        greenYellowLight.color = Color.green;

        SetEmission(Color.yellow);

        if (activated)
        {
            aud.PlayOneShot(correctSound);
        }
    }

    public IEnumerator L3LCUnActivate(bool current)
    {
        yield return new WaitForSeconds(1);
        StopAllCoroutines();

        if(current)
        aud.PlayOneShot(incorrectSound);

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

    void SetEmission(Color hue)
    {
            foreach (Material mat in OriginalColor)
            {
                mat.SetColor("_EmissionColor", hue);
            }
    }
}
