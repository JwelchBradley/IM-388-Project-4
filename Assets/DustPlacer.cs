using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject dust;

    [SerializeField]
    private float endDistHorizontal = 0;

    [SerializeField]
    private float endDistVertical = 0;

    [SerializeField]
    private float distBetweenVertically = 20;

    [SerializeField]
    private float distBetweenHorizontally = 20;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < (int)((transform.position.x - endDistHorizontal)/distBetweenHorizontally); i++)
        {
            for(int j = 0; j < (int)((transform.position.z - endDistVertical)/distBetweenVertically); j++)
            {
                Instantiate(dust, new Vector3(transform.position.x - distBetweenHorizontally*i, transform.position.y, transform.position.z - distBetweenVertically*j), Quaternion.identity, transform);
            }
        }
    }
}
