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

    public float maxSizeLow;
    public float maxSizeHigh;
    public float minSizeLow;
    public float minSizeHigh;
    public float decSizeLow;
    public float decSizeHigh;
    public float incSizeLow;
    public float incSizeHigh;

    // Start is called before the first frame update
    void Start()
    {
        mainLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

        maxSize = Random.Range(maxSizeLow, maxSizeHigh);
        minSize = Random.Range(minSizeLow, minSizeHigh);
        decSize = Random.Range(decSizeLow, decSizeHigh); // 50 75
        incSize = Random.Range(incSizeLow, incSizeHigh); // 40 60

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
