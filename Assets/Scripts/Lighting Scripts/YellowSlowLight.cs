using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSlowLight : MonoBehaviour
{

    Light mainLight;

    bool swap;

    float minSize;
    float maxSize;
    float decSize;
    float incSize;

    // Start is called before the first frame update
    void Start()
    {
        mainLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

        maxSize = Random.Range(20f, 30f);
        minSize = Random.Range(14f, 18f);
        decSize = Random.Range(50f, 75f);
        incSize = Random.Range(14f, 18f);

        if(swap)
        {
            mainLight.range -= decSize * Time.deltaTime;
        }
        else
        {
            mainLight.range += incSize * Time.deltaTime;
        }

        if(mainLight.range <= minSize)
        {
            swap = false;
        }
        else if (mainLight.range >= maxSize)
        {
            swap = true;
        }

    }
}
