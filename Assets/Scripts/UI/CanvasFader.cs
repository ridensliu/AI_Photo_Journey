using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFader : MonoBehaviour
{
    public float tweenTime = 0.5f;
    public LeanTweenType tweenType = LeanTweenType.easeInOutCubic;
    public float alphaStart = 1;
    public float alphaEnd = 0;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        _canvasGroup.alpha = alphaStart;
        LeanTween.alphaCanvas(_canvasGroup, alphaEnd, tweenTime)
            .setEase(tweenType);
    }
}
