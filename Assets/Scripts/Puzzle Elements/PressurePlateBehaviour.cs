using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class PressurePlateBehaviour : MonoBehaviour
{
    BoxCollider bc;

    AudioSource aud;

    [SerializeField]
    private PressurePlateGroupController ppgc;

    [Header("Hold Down Items")]
    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private string allowedTag = "";

    private RaycastHit hit;

    private bool heldDown = false;

    Vector3 startPos;
    Vector3 pushedPos;

    [Header("Activatables")]
    [SerializeField]
    [Tooltip("The door that this pressure plate opens")]
    private DoorBehaviour door;

    [Header("Movement Values")]
    [SerializeField]
    float moveSpeed = 3;

    [SerializeField]
    float downDist = 1f;

    [SerializeField]
    private bool allowHand = false;
    //
    [Header("Wires")]
    [SerializeField]
    private bool canShowWithoutEye = true;

    [SerializeField]
    private GameObject[] Wire;
    List<Material> OriginalColor = new List<Material>();

    bool isChanging = false;

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

    // Start is called before the first frame update
    void Awake()
    {
        if(ppgc != null)
        {
            ppgc.pressurePlate.Add(gameObject.GetComponent<PressurePlateBehaviour>());
        }

        aud = GetComponent<AudioSource>();

        startPos = transform.position;
        bc = GetComponent<BoxCollider>();
        pushedPos = bc.bounds.extents.y * downDist * -transform.up + transform.position;
        //
        foreach(GameObject wire in Wire)
        {
            OriginalColor.Add(wire.GetComponent<Renderer>().material);
        }

        foreach(Material mat in OriginalColor)
        {
            mat.EnableKeyword("_EMISSION");
        }

        //OriginalColor.EnableKeyword("_EMISSION");
        foreach (Material mat in OriginalColor)
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool shouldHoldDown = Physics.BoxCast(transform.position - transform.up.normalized, bc.bounds.extents, transform.up, out hit, Quaternion.identity, 4, playerMask);

        if(!allowHand && shouldHoldDown && hit.transform.gameObject.CompareTag("Hand"))
        {
            shouldHoldDown = false;
        }

        if(allowedTag != "" && !heldDown && shouldHoldDown && !hit.transform.gameObject.CompareTag(allowedTag))
        {
            
            shouldHoldDown = false;
        }

        if (shouldHoldDown && !heldDown)
        {
            heldDown = true;
            StopAllCoroutines();
            StartCoroutine(ChangeState(true));
            if(door != null)
            door.ChangeState(1);

            if(ppgc != null)
            ppgc.UpdateActivated(1);
            //
            SetEmission(Color.yellow);
        }
        else if(!shouldHoldDown && heldDown)
        {
            heldDown = false;
            StopAllCoroutines();
            StartCoroutine(ChangeState(false));
            if(door != null)
            door.ChangeState(-1);

            if(ppgc != null)
            ppgc.UpdateActivated(-1);
            //
            SetEmission(Color.black);
        }
        else if(!heldDown && transform.position != pushedPos && !isChanging)
        {
            StartCoroutine(ChangeState(false));
        }
    }

    private IEnumerator ChangeState(bool shouldChange)
    {
        isChanging = true;
        Vector3 target = startPos;
        if (shouldChange)
        {
            target = pushedPos;

            aud.Play();
        }

        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
        isChanging = false;
    }
    //
    void SetEmission(Color hue)
    {
        if (canShowWithoutEye)
        {
            foreach (Material mat in OriginalColor)
            {
                mat.SetColor("_EmissionColor", hue);
            }
        }
    }

    public void AllowEmission(bool allow)
    {
        if (allow)
        {
            foreach (Material mat in OriginalColor)
            {
                mat.SetColor("_EmissionColor", Color.yellow);
            }
        }
        else
        {
            foreach (Material mat in OriginalColor)
            {
                mat.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
