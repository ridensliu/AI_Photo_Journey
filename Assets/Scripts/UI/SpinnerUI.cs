using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpinnerUI : MonoBehaviour
{
    [SuffixLabel("degree", true)]
    public float spinSpeed = 320f;

    private RectTransform _rectTransform;
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }
}
