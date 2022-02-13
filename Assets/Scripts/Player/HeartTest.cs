using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartTest : MonoBehaviour
{
    [Tooltip("The heart controller")]
    public HeartController hc;

    /// <summary>
    /// The mesh renderer for this object
    /// </summary>
    private MeshRenderer mesh;

    /// <summary>
    /// The state the heart switch was in last frame
    /// </summary>
    private bool previousState = false;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hc.enabled)
        {
            if(previousState != hc.heartbeatSwitch)
            {
                mesh.enabled = !mesh.enabled;
                previousState = hc.heartbeatSwitch;
            }
        }
        else
        {
            mesh.enabled = true;
        }
    }
}
