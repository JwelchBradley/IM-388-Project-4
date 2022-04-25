using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfLightInRange : MonoBehaviour
{
    private static Transform mainCam;
    private Vector3 pos;
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        light = GetComponentInChildren<Light>();

        if(mainCam == null)
        {
            mainCam = Camera.main.transform;
        }

        StartCoroutine(CheckIfInRange());
    }

    private IEnumerator CheckIfInRange()
    {
        while (true)
        {
            if((mainCam.position - pos).sqrMagnitude < 30000)
            {
                light.enabled = true;
            }
            else
            {
                light.enabled = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
