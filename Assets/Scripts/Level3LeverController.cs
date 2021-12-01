using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3LeverController : MonoBehaviour
{
    [SerializeField]
    private List<LeverBehaviour> levers = new List<LeverBehaviour>();

    [SerializeField]
    private DoorBehaviour[] door;

    Level3LeverController l3lc;

    private int currentIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        l3lc = GetComponent<Level3LeverController>();

        foreach(LeverBehaviour lb in levers)
        {
            lb.l3lc = l3lc;
        }
    }

    public void CompareIndex(LeverBehaviour lb)
    {
        if (lb.Equals(levers[currentIndex]))
        {
            currentIndex++;

            if(currentIndex == levers.Count)
            {
                foreach (DoorBehaviour db in door)
                {
                    db.ChangeState(1);
                }
                //Debug.Log("openDoor");
            }
        }
        else
        {
            for(int i = 0; i < currentIndex; i++)
            {
                StartCoroutine(levers[i].L3LCUnActivate());
            }

            StartCoroutine(lb.L3LCUnActivate());
            currentIndex = 0;
        }
    }
}
