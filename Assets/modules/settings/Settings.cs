using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Settings
{
    public string strInputDirectory 
    {
        get { return $"{Application.dataPath}/../inputs"; }
    }
    public string strOutputDirectory
    {
        get { return $"{Application.dataPath}/../outputs"; }
    }
    public string strMainFolder
    {
        get { return $"{Application.dataPath}/.."; }
    }

    public string strSDDirectory
    {
        get 
        {
#if !UNITY_EDITOR
            return $"{Application.dataPath}/../stable-diffusion";
#else
            // return $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/stable-diffusion";
            return "G:/CCI_FinalProject/aiimages-gitpull/build/stable-diffusion";
#endif
        }
    }
    public string strEnvPath
    {
        get
        {
#if !UNITY_EDITOR
            return $"{Application.dataPath}/../env";
#else
            // return $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/anaconda3/envs/ldm";
            return "G:/CCI_FinalProject/aiimages-gitpull/build/env";
#endif
        }
    }

    public string strAnacondaBatPath
    {
        get
        {
#if !UNITY_EDITOR
            return $"{Application.dataPath}/../env/Scripts/activate.bat";
#else
            // return $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/anaconda3/Scripts/activate.bat";
            return "G:/CCI_FinalProject/aiimages-gitpull/build/env/Scripts/activate.bat";
#endif
        }
    }

    public bool bAcceptedLicense = false;
    public bool bIsFirstStart = true;
    public bool bDidSetup = false;
    public bool bFullPrecision = false;
    public bool bFreeGPUMemory = false;
    public int iMaxStepCount = 150;
    public float fUIScale = 0.8f;
    public int iGPU = 0;
    public bool bUseBackgroundTexture = true;

    public static string strSettingsName = "settings.json";
    private static string strSettingsPath = "";

    public static Settings Load()
    {
        Debug.Log("Loading settings");

        Settings settings = new Settings();

        strSettingsPath = Path.Combine(Application.persistentDataPath, strSettingsName);
        if (File.Exists(strSettingsPath) && !string.IsNullOrEmpty(File.ReadAllText(strSettingsPath)))
                settings = JsonUtility.FromJson<Settings>(File.ReadAllText(strSettingsPath));
           

        return settings;
    }

    public void Save()
    {
        Debug.Log("Saving settings.");

        strSettingsPath = Path.Combine(Application.persistentDataPath, strSettingsName);

        if (File.Exists(strSettingsPath))
            File.Delete(strSettingsPath);

        File.WriteAllText(strSettingsPath, JsonUtility.ToJson(this, prettyPrint:true));
    }
}
