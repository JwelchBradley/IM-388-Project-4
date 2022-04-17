using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeHiddenController : MonoBehaviour
{
    private List<GameObject> eyeHiddenImages = new List<GameObject>();

    public List<GameObject> EyeHiddenImages
    {
        get => eyeHiddenImages;
    }

    private PlayerController pc;

    private bool currentlyEye = false;

    private void Awake()
    {
        GameObject playerobj = GameObject.Find("Player");

        if(playerobj != null)
        pc = playerobj.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool currentActive = pc!=null && pc.CurrentActive.Equals(PlayerController.activeController.EYE);

        if (currentActive && !currentlyEye)
        {
            currentlyEye = true;

            foreach(GameObject eyeHiddenImage in eyeHiddenImages)
            {
                eyeHiddenImage.SetActive(false);
            }
        }
        else if(!currentActive && currentlyEye)
        {
            currentlyEye = false;

            foreach (GameObject eyeHiddenImage in eyeHiddenImages)
            {
                eyeHiddenImage.SetActive(true);
            }
        }
    }
}
