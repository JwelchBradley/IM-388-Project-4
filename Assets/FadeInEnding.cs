using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInEnding : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0;

        while (t < 1)
        {
            yield return new WaitForSecondsRealtime(.012f);
            print(t);
            t += 0.02f;
            if(t > 1)
            {
                t = 1;
            }
            canvasGroup.alpha = t;
        }
    }
}
