using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    public Text dialogueText;
    public GameObject dialoguePanel; 

    Camera camPos;

    public DialogueSounds DialogueSnds;

    public AudioClip letterSnd;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();

        dialoguePanel.SetActive(false);

        camPos = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count <= 0)
        {
            dialoguePanel.SetActive(false);

            return;
        }

        string sentence = sentences.Dequeue();
        //dialogueText.text = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            //LetterSoundSelection(letter);

            dialogueText.text += letter;
            AudioSource.PlayClipAtPoint(LetterSoundSelection(letter), camPos.transform.position);
            yield return new WaitForFixedUpdate();
        }
    }

    public AudioClip LetterSoundSelection(char test)
    {
        if (test != '.' && test != ' ' && test != '?' && test != ',' && test != '\'' && test != '!' && test != '\'')
        {
            char name = test;
            //char c = 'b'; you may use lower case character.
            int index = char.ToUpper(name) - 64;//index == 1
            //Debug.Log(index);
            return DialogueSnds.Sounds[index];
        }
        else return DialogueSnds.Sounds[26];
    }
}
