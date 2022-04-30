using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndSpikesController : MonoBehaviour
{
    public UnityEvent<bool> spikeAction;

    [SerializeField]
    private float timeBeforeFirstSpike = 0.5f;

    [SerializeField]
    private float timeBeforeSecondSpike = 0.8f;

    [SerializeField]
    private float timeBetweenSpikes = 2.0f;

    [SerializeField]
    private float restartWaitTime = 1f;

    private Coroutine spikeRoutineRef;

    private bool hasStarted = false;

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hasStarted)
        {
            hasStarted = true;
            spikeRoutineRef = StartCoroutine(SpikeRoutine(false));
        }
    }

    public void ResetSpikes()
    {
        spikeAction.Invoke(true);
        StopCoroutine(spikeRoutineRef);
        spikeRoutineRef = StartCoroutine(SpikeRoutine(true));
    }

    private IEnumerator SpikeRoutine(bool shouldRestartWait)
    {
        if (shouldRestartWait)
            yield return new WaitForSeconds(restartWaitTime);

        yield return new WaitForSeconds(timeBeforeFirstSpike);
        spikeAction.Invoke(false);
        yield return new WaitForSeconds(timeBeforeSecondSpike);

        while (true)
        {
            spikeAction.Invoke(false);
            yield return new WaitForSeconds(timeBetweenSpikes);
        }
    }
}
