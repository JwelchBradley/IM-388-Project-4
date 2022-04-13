using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartTest : MonoBehaviour,Activatable
{
    [Tooltip("The heart controller")]
    public HeartController hc;

    /// <summary>
    /// The mesh renderer for this object
    /// </summary>
    private MeshRenderer mesh;

    public void ChangeObjectState()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }
}
