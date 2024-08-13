using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFade : MonoBehaviour
{
    #region FIELDS

    private CanvasGroup canvasGroup;
    private float fadeTime = 1.0f;
    private bool startFade = false;
    private bool FadeIn = false;
    private bool FadeOut = false;

    #endregion FIELDS

    #region UNITY METHODS

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (startFade)
        {
            if (FadeIn)
            {
                if (canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha += Time.deltaTime / fadeTime;
                    if (canvasGroup.alpha >= 1)
                    {
                        startFade = false;
                        FadeIn = false;
                    }
                }
            }
            else if (FadeOut)
            {
                if (canvasGroup.alpha >= 0)
                {
                    canvasGroup.alpha -= Time.deltaTime / fadeTime;
                    if (canvasGroup.alpha == 0)
                    {
                        startFade = false;
                        FadeOut = false;
                        this.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    public void setAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
    }

    public void FadeInCanvas(float fadetm)
    {
        canvasGroup.alpha = 0;
        fadeTime = fadetm;
        startFade = true;
        FadeIn = true;
    }

    public void FadeOutCanvas(float fadetm)
    {
        fadeTime = fadetm;
        startFade = true;
        FadeOut = true;
    }

    #endregion METHODS
}