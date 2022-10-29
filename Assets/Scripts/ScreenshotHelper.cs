using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotHelper : MonoBehaviour
{
    public RenderTexture renderTexture;

    private Texture2D _screenshotTex;
    private int _screenshotCount;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _screenshotTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
    }

    // private void OnEnable()
    // {
    //     StartCoroutine(TakeScreenshot());
    // }

    public void Take(Action<string> callback)
    {
        gameObject.SetActive(true);
        StartCoroutine(TakeScreenshot(callback));
    }

    private IEnumerator TakeScreenshot(Action<string> callback)
    {
        _camera.Render();
        
        yield return null;

        var temp = RenderTexture.active;

        RenderTexture.active = renderTexture;
        _screenshotTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        var bytes = _screenshotTex.EncodeToPNG();
        var path = Path.Combine(Application.dataPath, $"../inputs/screenshot-{_screenshotCount++}.png");
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = temp;
        
        callback.Invoke($"screenshot-{_screenshotCount - 1}.png");
        
        gameObject.SetActive(false);
    }
}
