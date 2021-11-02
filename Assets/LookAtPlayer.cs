using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    private void FixedUpdate()
    {
        transform.LookAt(player.transform.position);
    }
}
