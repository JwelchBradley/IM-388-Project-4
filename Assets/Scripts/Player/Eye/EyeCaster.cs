using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCaster : MonoBehaviour
{
    private GameObject eyeLandIndicator;
    private float eyeSizeMod = 1.5f;
    private float maxDist = 30;
    private Transform cam;
    private RaycastHit hit;

    [SerializeField]
    private LayerMask landableMask;

    [SerializeField]
    private LayerMask castableMask;

    private MeshRenderer mr;

    [SerializeField]
    private Material canCastMat;

    [SerializeField]
    private Material cannotCastMat;

    private bool isCasting = false;

    private bool canCast = false;

    public bool CanCast
    {
        get => canCast;
    }

    public bool IsCasting
    {
        get => isCasting;

        set
        {
            isCasting = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main.transform;
        eyeLandIndicator = GameObject.Find("EyeLand");
        mr = eyeLandIndicator.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCasting)
        {
            DisplayLandLocation();
        }
        else
        {
            if (mr.enabled)
            {
                mr.enabled = false;
            }
        }
    }

    private void DisplayLandLocation()
    {
        canCast = Physics.Raycast(cam.position, cam.forward, out hit, Mathf.Infinity, castableMask);

        if (canCast)
        {
            if (!mr.enabled)
            {
                mr.enabled = true;
            }


            // Can Land
            if (landableMask == (landableMask | (1 << hit.transform.gameObject.layer)) && Vector3.Distance(hit.point, cam.position) < maxDist)
            {
                mr.material = canCastMat;
            }

            // Cannont Land
            else
            {
                mr.material = cannotCastMat;
                canCast = false;
            }

            eyeLandIndicator.transform.position = hit.point;
            eyeLandIndicator.transform.localScale = Vector3.one * Vector3.Distance(hit.point, cam.position) * (eyeSizeMod / 100);
            eyeLandIndicator.transform.rotation = SpawnRotation();

        }
    }

    private void MoveIndicatorTowardsLocation() { }

    public GameObject SpawnObject(GameObject obj)
    {
        //GameObject eye = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Eye/Eye", typeof(GameObject)), hit.point, SpawnRotation());
        GameObject newObject = Instantiate(obj, hit.point, SpawnRotation());
        newObject.transform.position = SpawnSpot(newObject);
        mr.enabled = false;
        return newObject;
    }

    public GameObject SpawnEye()
    {
        GameObject eye = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Eye/Eye", typeof(GameObject)), hit.point, SpawnRotation());
        eye.transform.position = SpawnSpot(eye);
        mr.enabled = false;
        return eye;
    }

    private Vector3 SpawnSpot(GameObject eye)
    {
        Vector3 spot = eye.transform.position + eye.transform.forward.normalized*0.5f;
        
        return spot;
    }

    private Quaternion SpawnRotation()
    {
        float zMod = 1;
        if(hit.normal.z > -0.1f && hit.normal.z < 0.1f)
        {
            zMod = 0;
        }

        Quaternion rotation = Quaternion.Euler(new Vector3(hit.normal.y*-90, hit.normal.x*90 + hit.normal.z*-90 + 90*zMod, 0));

        return rotation;
    }
}
