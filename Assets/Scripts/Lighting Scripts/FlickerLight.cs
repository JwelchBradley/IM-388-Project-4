using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light[] flickeringLights;

    public int minMiniFlickerNumber = 2;
    public int maxMiniFlickerNumber = 6;
    public float miniFlickerTime = 0.15f;
    public float minFlickerTime = 0.5f;
    public float maxFlickerTime = 2.5f;

    public bool mainState = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            int randMiniFlickerNumber = Random.Range(minMiniFlickerNumber, maxMiniFlickerNumber);
            for (int i=0; i<randMiniFlickerNumber; i++)
            {
                foreach (Light light in flickeringLights)
                {
                    light.enabled = !light.enabled;
                }
                yield return new WaitForSeconds(miniFlickerTime);
            }
            foreach(Light light in flickeringLights)
            {
                light.enabled = mainState;
            }

            float randWait = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(randWait);
        }
    }
}
