using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    Light mainLight;

    bool swap;

    float minSize;
    float maxSize;
    float decSize;
    float incSize;

    public float waitTime;
    public int chanceMin;
    public int chanceMax;

    // Start is called before the first frame update
    void Start()
    {
        mainLight = GetComponent<Light>();
        StartCoroutine(Timer());
    }

    int flicker;

    // Update is called once per frame
    void Update()
    {


        if(flicker != 1)
        {
            mainLight.range = 30;
        }
        else
        {
            mainLight.range = 0;
        }



    }


    private IEnumerator Timer()
    {
        while (true)
        {
            // light

            flicker = Random.Range(chanceMin, chanceMax);

            yield return new WaitForSeconds(waitTime);
        }
    }
}
