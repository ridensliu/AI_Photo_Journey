using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRectUpdater : MonoBehaviour
{
    public bool cropToOne = true;
    public bool immediately = true;
    
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        var targetRect = GetTargetRect();

        if (immediately)
        {
            _camera.rect = targetRect;
        }
        else
        {
            _camera.rect = LerpRect(_camera.rect, targetRect, 5.5f * Time.deltaTime);
        }
    }

    private Rect LerpRect(Rect a, Rect b, float t)
    {
        return new Rect(
            Mathf.Lerp(a.x, b.x, t),
            Mathf.Lerp(a.y, b.y, t),
            Mathf.Lerp(a.width, b.width, t),
            Mathf.Lerp(a.height, b.height, t)
            );
    }

    private Rect GetTargetRect()
    {
        if (!cropToOne)
        {
            return new Rect(0, 0, 1, 1);
        }
        
        var ratio = (float)Screen.width / (float)Screen.height;

        return ratio > 1 ? new Rect((1 - 1f / ratio) / 2f, 0, 1f / ratio, 1) : new Rect(0, (1 - 1f / ratio) / 2f, 1, 1f / ratio);
    }

    public void SetCropToOne(bool state)
    {
        cropToOne = state;
    }
}
