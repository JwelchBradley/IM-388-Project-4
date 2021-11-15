using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteBehaviour : Interactable
{
    PauseMenuBehavior pm;
    TextMeshProUGUI noteText;

    [SerializeField]
    string noteString = "";

    private AudioSource aud;

    // Start is called before the first frame update
    void Start()
    {
        //pm.Note = GameObject.Find("Note");
        pm = GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>();
        noteText = pm.Note.GetComponentInChildren<TextMeshProUGUI>();

        aud = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        foreach (PlayerController.activeController ac in displayTextControllers)
        {
            if (ac.Equals(pc.CurrentActive))
            {
                if (!pm.Note.activeInHierarchy && Time.timeScale != 0)
                {
                    pm.Note.SetActive(true);
                    noteText.text = noteString;

                    if (!aud.isPlaying)
                        aud.Play();
                }
                else
                {
                    aud.Stop();
                    pm.Note.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            aud.Stop();
            pm.Note.SetActive(false);
        }
    }
}
