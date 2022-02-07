using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinBehaviour : MonoBehaviour
{
    private PauseMenuBehavior pmb;

    [SerializeField]
    private GameObject winMenu;

    private void Awake()
    {
        pmb = GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pmb.CanPause = false;
            Time.timeScale = 0;
            winMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
