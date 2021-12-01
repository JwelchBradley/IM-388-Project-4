using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MoveSoundBehaviour : MonoBehaviour
{
    public Rigidbody rb;

    private bool canPlaySound;

    private AudioSource moveAud;

    // Start is called before the first frame update
    void Start()
    {
        canPlaySound = true;

        rb = GetComponent<Rigidbody>();

        moveAud.mute = true;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        AudioMixer mixer = Resources.Load("AudioMaster") as AudioMixer;
        moveAud = GetComponent<AudioSource>();
        moveAud.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        moveAud.playOnAwake = false;
        moveAud.spatialBlend = 1;
        moveAud.rolloffMode = AudioRolloffMode.Custom;
        moveAud.minDistance = 50;
        moveAud.maxDistance = 200;
    }
#endif

    private void Awake()
    {
        moveAud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = rb.velocity;

        if(velocity.magnitude > 0.01)
        {
            moveAud.mute = false;

            PlayMove();
        }

        if(velocity.magnitude <= 0.01)
        {
            moveAud.mute = true;

            moveAud.Stop();

            canPlaySound = true;
        }
    }

    public void PlayMove()
    {
        if (canPlaySound)
        {
            moveAud.Play();

            canPlaySound = false;
        }
    }
}
