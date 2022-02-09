using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowRadialMenu : MonoBehaviour
{
    PlayerController pc;

    [SerializeField]
    private bool allow = false;

    // Start is called before the first frame update
    void Awake()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pc.CanOpenRadial = allow;
        }
    }
}
