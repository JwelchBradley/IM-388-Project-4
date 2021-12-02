using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeHiderHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("EyeHiddenController").GetComponent<EyeHiddenController>().EyeHiddenImages.Add(gameObject);
    }
}
