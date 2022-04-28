using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPanelController : MonoBehaviour
{
    private CanvasGroup cg;

    private int iter = -1;

    private bool inFade = false;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void StartFade()
    {
        if (inFade) return;
        inFade = true;

        iter *= -1;
        StartCoroutine(FadeCanvas((cg.alpha + 1) % 2, iter));
    }

    private IEnumerator FadeCanvas(float targetAlpha, int iter)
    {
        while (cg.alpha != targetAlpha)
        {
            yield return new WaitForFixedUpdate();
            cg.alpha += Time.fixedDeltaTime * iter * 2.5f;
        }

        inFade = false;
    }
}
