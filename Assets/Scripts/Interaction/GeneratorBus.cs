using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneratorBus : MonoBehaviour
{
    public UnityEvent onModelLoaded;
    public UnityEvent onStartGenerating;
    public UnityEvent<Texture2D> onPictureGenerated;

    private void Start()
    {
        var generator = NewImageGenerator.Instance;
        
        generator.onModelLoaded.AddListener(() => { onModelLoaded.Invoke(); });
        generator.onStartGenerating.AddListener(() => { onStartGenerating.Invoke(); });
        generator.onPictureGenerated.AddListener(tex => { onPictureGenerated.Invoke(tex); });
    }

    public void TakeScreenshotAndGenerate()
    {
        NewImageGenerator.Instance.TakeScreenshotAndGenerate();
    }
}
