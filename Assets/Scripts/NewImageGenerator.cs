using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NewImageGenerator : MonoBehaviour
{
    public static NewImageGenerator Instance;
    
    [HideInInspector]
    public User userActive = new User();
    public Texture2D texDefaultMissing;

    [BoxGroup("Prompt")]
    public string contentString;
    [BoxGroup("Prompt")]
    public string styleString;
    [BoxGroup("Prompt")]
    [Range(0, 1)]
    public float referenceStrength = 0.3f;
    
    private Settings _settings;
    public Settings Settings => _settings;
    public UnityEvent onModelLoaded;
    public UnityEvent onStartGenerating;
    public UnityEvent<Texture2D> onPictureGenerated;
    public bool IsGenerating { get; private set; }
    private bool _allowGenerating = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _settings ??= Settings.Load();
        GeneratorConnection.instance.settings = _settings;
        GeneratorConnection.instance.texDefaultMissing = texDefaultMissing;
        GeneratorConnection.instance.Init();

        StartCoroutine(CheckModelLoad());
    }

    private IEnumerator CheckModelLoad()
    {
        while (!GeneratorConnection.instance.bInitialized)
        {
            yield return null;
        }
        
        onModelLoaded.Invoke();
    }

    [Button("Generate")]
    public void GenerateImage()
    {
        ImageInfo outputNew = new ImageInfo()   //定义新的输出image信息
        {
            strGUID = Guid.NewGuid().ToString(),
            prompt = new Prompt()
            {
                iSeed = Mathf.FloorToInt(Random.Range(0, 100000000)),
                strContentPrompt = contentString,
                strStylePrompt = styleString
            },
            // extraOptionsFull = options.extraOptionsGet(),
            userCreator = userActive
        };
        
        GeneratorConnection.instance.RequestImage(outputNew, (texture2D, s, arg3) =>
        {
            Debug.Log(texture2D.width);
        });
    }

    [Button("Take Screenshot")]
    public void TakeScreenshot()
    {
        ScreenshotHelper.Instance.Take((file) =>
        {
            Debug.Log(file);
        });
    }

    [Button("Take Screenshot & Gen")]
    public void TakeScreenshotAndGenerate()
    {
        if (!_allowGenerating) return;
        
        if (!GeneratorConnection.instance.bInitialized || IsGenerating) return;

        IsGenerating = true;
        onStartGenerating.Invoke();
        
        ScreenshotHelper.Instance.Take((file) =>
        {
            ImageSaver.Instance.CopyImageTo(new StartImage()
            {
                strFilePath = file,
                fStrength = referenceStrength
            }.strGetFullPath());
            
            ImageInfo outputNew = new ImageInfo()   //定义新的输出image信息
            {
                strGUID = Guid.NewGuid().ToString(),
                prompt = new Prompt()
                {
                    iSeed = Mathf.FloorToInt(Random.Range(0, 100000000)),
                    strContentPrompt = contentString,
                    strStylePrompt = styleString,
                    startImage = new StartImage()
                    {
                        strFilePath = file,
                        fStrength = referenceStrength
                    }
                },
                // extraOptionsFull = options.extraOptionsGet(),
                userCreator = userActive
            };
        
            GeneratorConnection.instance.RequestImage(outputNew, (texture2D, s, arg3) =>
            {
                IsGenerating = false;
                onPictureGenerated.Invoke(texture2D);
                ImageSaver.Instance.SaveTextureTo(texture2D);
            });
        });
    }

    public void SetContentPrompt(string value)
    {
        contentString = value;
    }

    public void SetStylePrompt(string value)
    {
        styleString = value;
    }

    public string GetPromptText()
    {
        return "Content: " + contentString + ", \n" + "Style: " + styleString;
    }

    public void GetRandomTextFromPool(PromptPool pool)
    {
        var info = pool.infos[Random.Range(0, pool.infos.Length)];
        SetContentPrompt(info.content);
        SetStylePrompt(info.style);
    }

    public void SetAllowGenerating(bool state)
    {
        _allowGenerating = state;
    }
}
