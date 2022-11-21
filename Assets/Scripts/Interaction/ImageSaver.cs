using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageSaver : MonoBehaviour
{
    public static ImageSaver Instance;
    
    public string folderPath;
    public int maxImage = 5;

    private int _sourceCount;
    private int _outputCount;

    private void Awake()
    {
        Instance = this;
    }

    public void CopyImageTo(string imagePath)
    {
        Directory.CreateDirectory(folderPath);
        File.Copy(imagePath, Path.Combine(folderPath, "i-" + _sourceCount + ".png"), true);
        _sourceCount = (_sourceCount + 1) % maxImage;
    }

    public void SaveTextureTo(Texture2D tex)
    {
        File.WriteAllBytes(Path.Combine(folderPath, $"o-{_outputCount}.png"), tex.EncodeToPNG());
        _outputCount = (_outputCount + 1) % maxImage;
    }
}
