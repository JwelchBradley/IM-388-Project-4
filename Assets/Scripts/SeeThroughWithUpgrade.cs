using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughWithUpgrade : MonoBehaviour
{
    private Renderer render;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if(playerObj != null)
        player = playerObj.GetComponentInChildren<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player != null && player.eyeType == 1 && player.CurrentActive == PlayerController.activeController.EYE)
        {
            render.enabled = false;
        }
        else
        {
            render.enabled = true;
        }
    }
}
