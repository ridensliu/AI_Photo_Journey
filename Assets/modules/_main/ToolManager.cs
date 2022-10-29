using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using Newtonsoft.Json;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;
    public static Settings s_settings;
    public Setup setup;

    public static History s_history = new History();
    public static List<Template> s_liStyleTemplates = new List<Template>();
    public static List<Template> s_liContentTemplates = new List<Template>();
    public static List<string> s_liFavoriteGUIDs = new List<string>();
    public static List<User> s_liUsers = new List<User>();
    public static Dictionary<ImageInfo, List<object>> s_dictDisplayedBy = new Dictionary<ImageInfo, List<object>>();

    public GameObject goImagePreviewPrefab;
    public EndlessHistory endlessHistory;

    public Color colorFavorite;
    public Color colorButton;

    public Canvas canvasMain;
    public Canvas canvasTitle;
    public Canvas canvasTooltips;

    public static Texture2D s_texDefaultMissing { get => Instance.texDefaultMissing; }
    public Texture2D texDefaultMissing;
    public GameObject goWarning;
    public TMP_Text textWarning;
    public GameObject goWarningTriangle;

    public GameObject goTextureBackground;

    public GameObject goContextMenuWindowPrefab;
    public Transform transContextMenuCanvas;
    public GameObject goTooltipPrefab;
    public Transform transTooltipCanvas;
    public PaletteView paletteView;
    public TextAsset textDefaultStyles;

    // UI
    public TMP_Text textFeedback;
    public TMP_Text textVersion;

    public Slider sliderUIScale;

    // start button
    public Button buttonStart;
    public TMP_Text textStartButton;
    public Image imageIconStart;
    public Sprite spritePlay;
    public Sprite spriteStop;
    public Color colorPlay;
    public Color colorStop;

    public List<ImageInfo> liRequestQueue = new List<ImageInfo>(); // requester and prompt
    private ImageInfo outputCurrentRequested;

    public List<ImagePreview> liImagePreviews;

    public GeneratorConnection genConnection; // Generate Connection
    public OptionsVisualizer options;

    public GameObject goSelectPage;
    public GameObject goLicensePage;
    public GameObject goAboutPage;

    public enum Page { Select, Install, Main, License }

    public User userActive = new User();

    public UnityEvent eventQueueUpdated = new UnityEvent();
    public UnityEvent eventHistoryUpdated = new UnityEvent();
    public UnityEvent<ImageInfo> eventHistoryElementDeleted = new UnityEvent<ImageInfo>();

    private float fProcessingTime = 0f;
    private float fLoadingTime = 0f;

    private bool bKeepRequesting = false;

    private string strWarningText = "";
    private bool bWontWork = false;


    void Awake()
    {
        Application.runInBackground = true;
        StartCoroutine(ieCheckVersion());

        s_liUsers.Add(userActive);

        Instance = this;
        Load();
    }

    private void Start()
    {
        Debug.Log(strGetWarningText());
        bWontWork = strGetWarningText().Contains("Won't work");
        if (bWontWork) // HACK
        {
            goWarning.SetActive(true);
            textWarning.text += "\n" + strGetWarningText();

            goWarningTriangle.SetActive(true);
            goWarningTriangle.GetComponent<Tooltip>().strText = strWarningText;
        }

        // backward comp
        string strSettingsPath = Path.Combine(Application.persistentDataPath, Settings.strSettingsName);
        if (File.Exists(strSettingsPath) && !File.ReadAllText(strSettingsPath).Contains("bFreeGPUMemory"))
        {
            Debug.Log("Found old settings file. Setting firstStart again.");
            s_settings.bIsFirstStart = true;
        }

        Startup();

        StartCoroutine(ieAutoSave(120f));
        StartCoroutine(ieStartDelayed());
    }

    private void Startup()
    {
        // FIRST STARTUP
        if (s_settings.bIsFirstStart)
        {
            if (SystemInfo.graphicsDeviceName.ToLower().Contains("gtx 16") && SystemInfo.graphicsMemorySize < 4500)
            {
                UnityEngine.Debug.Log("Detected GTX 16XX with low VRAM. Switching to using FreeMemory mode.");
                s_settings.bFreeGPUMemory = true;
                s_settings.Save();
            }

            SetUIScale(Camera.main.pixelHeight / 1080f * 0.8f);
            Setup.CreateDesktopShortcut();

            LoadDefaultStyles();
        }

        if (!s_settings.bAcceptedLicense)
            OpenPage(Page.License);
        else
            OpenPage(Page.Main);

        s_settings.bIsFirstStart = false;
        s_settings.Save();

        //CreateWebBat();
    }

    public IEnumerator ieStartDelayed()
    {
        yield return null; // delay, so options don't overwrite it with their own init
        if (s_history.liOutputs.Count > 0)
            options.LoadOptions(s_history.liOutputs.Last());
    }

    void Update()
    {
        // init or not?
        bool bButtonActive = genConnection.bInitialized; 
        
        // buttonStart: start button; make sure the button is interactive
        if (bButtonActive != buttonStart.interactable) 
            buttonStart.interactable = bButtonActive;

        // if genConnection is initialized
        if (!genConnection.bInitialized)
        {
            fLoadingTime += Time.deltaTime; // load time
            if (fLoadingTime < 200f)
                textFeedback.text = $"Loading model. Please wait... ({fLoadingTime.ToString("0")}s) {(bWontWork ? strGetWarningText() : "")}";
            else
                textFeedback.text = $"<color=#FF0000>Loading model... ({fLoadingTime.ToString("0")}s) {(bWontWork ? strGetWarningText() : "")} - Ask for Discord support!</color>";
        }
        // if genConnection is processing
        else if (genConnection.bProcessing)
        {
            fProcessingTime += Time.deltaTime;
            textFeedback.text = $"Painting {(bKeepRequesting ? "forever + " : "")}{liRequestQueue.Count + 1}... {fProcessingTime.ToString("0.0")}s";
        }
        else
        {
            if (bKeepRequesting && liRequestQueue.Count == 0)
                RequestImage(); //定义新的输出图片 并添加到 LiRequestQuene 中

            if (liRequestQueue.Count > 0)
            {
                outputCurrentRequested = liRequestQueue.First(); //目前在请求输出得图片
                outputCurrentRequested.eventStartsProcessing.Invoke(outputCurrentRequested);
                genConnection.RequestImage(outputCurrentRequested, OnTextureReceived); //生成输出图片 png
                liRequestQueue.RemoveAt(0);
                fProcessingTime = 0f;
                eventQueueUpdated.Invoke();
            }
            else
            {
                textFeedback.text = "Ready!";
                fLoadingTime = 0f;
            }
        }
    }

    public void OnTextureReceived(Texture2D _tex, string _strFilePathFull, bool _bWorked)
    {
        ImageInfo outputRequested = outputCurrentRequested;
        outputRequested.SetTex(_tex);
        outputRequested.fGenerationTime = fProcessingTime;
        outputRequested.dateCreation = DateTime.UtcNow;
        outputRequested.strFilePathRelative = _strFilePathFull.Replace(s_settings.strOutputDirectory, "");
        outputRequested.strGpuModel = $"{SystemInfo.graphicsDeviceName} ({Mathf.RoundToInt(SystemInfo.graphicsMemorySize)} MB)";
        outputRequested.iActualWidth = _tex.width;
        outputRequested.iActualHeight = _tex.height;

        if (_bWorked)
        {
            s_history.liOutputs.Add(outputRequested);
            eventHistoryUpdated.Invoke();
        }

        if (!_bWorked)
        {
            liRequestQueue.Clear();
            if (bKeepRequesting)
                TogglePlay();
            eventQueueUpdated.Invoke();
            fLoadingTime = 0f;
        }

        outputRequested.eventStoppedProcessing.Invoke(outputRequested, _bWorked);
    }

    public void TogglePlay()
    {
        bKeepRequesting = !bKeepRequesting;

        textStartButton.text = bKeepRequesting ? "Stop" : "Start";
        imageIconStart.sprite = bKeepRequesting ? spriteStop : spritePlay;

        ColorBlock colorBlock = buttonStart.colors;
        colorBlock.normalColor = bKeepRequesting ? colorStop : colorPlay;
        colorBlock.pressedColor = bKeepRequesting ? colorStop : colorPlay;
        colorBlock.selectedColor = bKeepRequesting ? colorStop : colorPlay;
        buttonStart.colors = colorBlock;
    }

    public void RequestImage()
    {
        UnityEngine.Debug.Log($"Starting preview for {options.promptGet(_bPreviewSteps:true, _bPreviewUpscale:true, _bPreviewFaceEnhance:true).strToString()}");

        ImageInfo outputNew = new ImageInfo()   //定义新的输出image信息
        {
            strGUID = Guid.NewGuid().ToString(),
            prompt = options.promptGet(_bPreviewSteps: true, _bPreviewUpscale: true, _bPreviewFaceEnhance: true),
            extraOptionsFull = options.extraOptionsGet(),
            userCreator = userActive
        };
        RequestImage(outputNew); //将新的输出信息添加到 LiRequestQueue 请求列表中
    }

    public void RequestImage(ImageInfo _output)
    {
        liRequestQueue.Add(_output);
        eventQueueUpdated.Invoke();
    }

    public string strGetWarningText()
    {
        if (!string.IsNullOrEmpty(strWarningText))
            return strWarningText;

        string strOutput = "";
        strOutput += $"<b>Your graphics card</b>: {SystemInfo.graphicsDeviceName} ({Mathf.RoundToInt(SystemInfo.graphicsMemorySize / 1000f)} GB)\n";

        int iWorks = 2; // yes, maybe, no
        string strProblem = "";
        if (SystemInfo.graphicsMemorySize < 3900)
        {
            iWorks = 0;
            strProblem += "\nNot enough GPU memory. Needs at least 4 GB.";
        }

        if (SystemInfo.systemMemorySize < 11000)
        {
            iWorks = 0;
            strProblem += "\nAI images needs at least 12 GB of memory (RAM).";
        }

        if (Application.dataPath.Contains(" "))
        {
            iWorks = 0;
            strProblem += $"\nPath contains space! Make sure all folders above aiimages don't contain spaces. Path: {Application.dataPath}";
        }

        if (SystemInfo.graphicsDeviceName.ToLower().Contains("amd")
            || SystemInfo.graphicsDeviceName.ToLower().Contains("ati")
            || SystemInfo.graphicsDeviceName.ToLower().Contains("radeon"))
        {
            iWorks = 0;
            strProblem += "\nAMD graphic cards won't work (yet). :(";
        }
        else if(SystemInfo.graphicsDeviceName.ToLower().Contains("intel"))
        {
            iWorks = 0;
            strProblem += "\nYour PC appears to not have a dedicated graphics card. :(";
        }
        else if (!SystemInfo.graphicsDeviceName.ToLower().Contains("rtx 30")
            && !SystemInfo.graphicsDeviceName.ToLower().Contains("rtx 20")
            && !SystemInfo.graphicsDeviceName.ToLower().Contains("gtx 1"))
        {
            iWorks = Mathf.Min(iWorks, 1);
            strProblem += "\nGPU is not the newest.";
        }

        if (!setup.bEverythingIsThere())
        {
            strProblem += "There are files missing. Try reinstalling the application if you encounter issues.";
            iWorks = Mathf.Min(iWorks, 1);
        }

        if (iWorks == 2)
            strOutput += "Should work! <3";
        else if (iWorks == 1)
            strOutput += "Might work! <3" + strProblem;
        else if (iWorks == 0)
            strOutput += "Won't work, most likely. :(" + strProblem;

        strWarningText = strOutput;

        return strOutput;
    }

    public void OpenPage(Page _page)
    {
        goSelectPage.SetActive(false);
        goLicensePage.SetActive(false);

        switch (_page)
        {
            case Page.Install:
                setup.Open();
                break;
            case Page.Select:
                goSelectPage.SetActive(true);
                break;
            case Page.Main:
                if (!genConnection.bInitialized)
                {
                    fLoadingTime = 0f;
                    textFeedback.text = "Loading model...";
                    genConnection.Init();
                }
                break;
            case Page.License:
                goLicensePage.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OpenOutputFolder()
    {
        Application.OpenURL($"file://{s_settings.strOutputDirectory}");
    }

    public void OpenTrashFolder()
    {
        Application.OpenURL($"file://{Path.Combine(s_settings.strOutputDirectory, "trashcan")}");
    }

    public void SetAboutPage(bool _bActive)
    {
        goAboutPage.SetActive(_bActive);
    }

    public void ShowSetup()
    {
        OpenPage(Page.Install);
    }

    public void ShowMainPage()
    {
        OpenPage(Page.Main);
    }

    public void Save()
    {
        s_settings.Save();
        s_history.Save();

        SaveData saveData = new SaveData()
        {
            liStyleTemplates = s_liStyleTemplates,
            liContentTemplates = s_liContentTemplates,
            liUsers = s_liUsers,
            liFavoriteGUIDs = s_liFavoriteGUIDs,
            palette = paletteView.palette
        };
        saveData.Save();
    }

    public void Load()
    {
        s_settings = Settings.Load();
        s_history = s_history.Load();

        SaveData saveData = SaveData.Load();
        s_liStyleTemplates = saveData.liStyleTemplates;
        s_liContentTemplates = saveData.liContentTemplates;
        s_liUsers = saveData.liUsers;
        s_liFavoriteGUIDs = saveData.liFavoriteGUIDs;
        paletteView.Set(saveData.palette);
        sliderUIScale.value = s_settings.fUIScale;

        UpdateBackgroundTexture();

        Directory.CreateDirectory(s_settings.strInputDirectory);
        Directory.CreateDirectory(s_settings.strOutputDirectory);
        Directory.CreateDirectory(Path.Combine(s_settings.strOutputDirectory, "favorites"));

        options.optionSteps.UpdateLimit();
    }

    public void UpdateBackgroundTexture()
    {
        goTextureBackground.SetActive(s_settings.bUseBackgroundTexture);
    }

    public void OpenLicense()
    {
        Application.OpenURL("https://huggingface.co/spaces/CompVis/stable-diffusion-license");
    }

    public void DeleteImage(ImageInfo _img)
    {
        if (s_liFavoriteGUIDs.Contains(_img.strGUID))
        {
            Debug.Log("Cannot delete favorite images.");
            return;
        }

        PreviewImage.Instance.SetVisible(false, null);

        MoveToTrashFolder(_img);
        DeleteFromHistory(_img);
    }

    public void MoveToTrashFolder(ImageInfo _img)
    {
        string strTrashFolder = Path.Combine(s_settings.strOutputDirectory, "trashcan");
        Directory.CreateDirectory(strTrashFolder);

        try
        {
            if (File.Exists(_img.strFilePathFull()))
                File.Move(_img.strFilePathFull(), Path.Combine(strTrashFolder, Path.GetFileName(_img.strFilePathFull())));
        }
        catch
        {
            Debug.Log("Tried to delete file that is already in trash");
        }
        
    }

    public void DeleteFromHistory(ImageInfo _img)
    {
        s_history.liOutputs.Remove(_img);
        eventHistoryElementDeleted.Invoke(_img);
        eventHistoryUpdated.Invoke();
    }

    public void SaveTemplate(ImageInfo _output, bool _bContent)
    {
        Template template = new Template()
        {
            strName = DateTime.Now.ToString("yyyy_MM_dd_hh_mm"),
            strGUID = Guid.NewGuid().ToString(),
            outputTemplate = _output
        };

        UnityEngine.Debug.Log($"Added {(_bContent ? "content" : "style")} template {template.strName}.");

        if (_bContent)
            s_liContentTemplates.Add(template);
        else
            s_liStyleTemplates.Add(template);
    }

    public void DeleteTemplate(Template _template, bool _bContent)
    {
        if (_bContent)
            s_liContentTemplates.RemoveAll(x => x.strGUID == _template.strGUID);
        else
            s_liStyleTemplates.RemoveAll(x => x.strGUID == _template.strGUID);
    }

    public void OpenURL(string _strURL)
    {
        Application.OpenURL(_strURL);
    }

    public void AgreeLicense()
    {
        UnityEngine.Debug.Log("Agreed License");
        s_settings.bAcceptedLicense = true;
        s_settings.Save();
        Startup();
    }

    public void OpenSettingsFolder()
    {
        Application.OpenURL($"File://{Application.persistentDataPath}");
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Setup.RenewDesktopShortcut();
        Save();
    }

    public static void AddDisplayer(ImageInfo _img, object _oDisplayer)
    {
        if (!s_dictDisplayedBy.ContainsKey(_img))
            s_dictDisplayedBy.Add(_img, new List<object> { _oDisplayer });
        else
        {
            if (!s_dictDisplayedBy[_img].Contains(_oDisplayer))
                s_dictDisplayedBy[_img].Add(_oDisplayer);
        }
            
    }

    public static void RemoveDisplayer(ImageInfo _img, object _oDisplayer)
    {
        if (!s_dictDisplayedBy.ContainsKey(_img))
        {
            UnityEngine.Debug.Log("Tried to remove displayer from imgInfo that doesn't have it.");
            return;
        }

        s_dictDisplayedBy[_img].RemoveAll(x => x == _oDisplayer);

        if (s_dictDisplayedBy[_img].Count == 0 && _img.bHasTexture() && _img.texGet() != s_texDefaultMissing)
                Destroy(_img.texGet());
    }

    public IEnumerator ieCheckVersion()
    {
        UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get("http://aiimag.es/data/version_newest.txt");
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log(webRequest.error);
            textVersion.text = $"v{Application.version}";
            yield break;
        }

        if (webRequest.downloadHandler.text == Application.version)
        {
            textVersion.text = $"up-to-date - v{Application.version}";
        }
        else
        {
            textVersion.text = $"<color=#00FF00>NEW AVAILABLE</color> - v{Application.version}";
        }
    }

    private IEnumerator ieAutoSave(float _fEvery)
    {
        yield return new WaitForSeconds(_fEvery);
        s_settings.Save();

        StartCoroutine(ieAutoSave(_fEvery));
    }

    public void SetUIScale(float _fScale)
    {
        s_settings.fUIScale = _fScale;
        canvasMain.scaleFactor = _fScale;
        canvasTitle.scaleFactor = _fScale;
        canvasTooltips.scaleFactor = _fScale;

        endlessHistory.OnScaleUpdate();
    }

    public void LoadDefaultStyles()
    {
        List<Template> liDefaultStyles = JsonConvert.DeserializeObject<List<Template>>(textDefaultStyles.text);
        s_liStyleTemplates.AddRange(liDefaultStyles);
        //File.WriteAllText("default_styles_generated.txt", JsonConvert.SerializeObject(s_liStyleTemplates, Formatting.Indented));
    }

    public void CreateWebBat()
    {
        string strContent = "@echo off";
        strContent += $"\ncall \"{ToolManager.s_settings.strAnacondaBatPath}\"";
        strContent += $"\n{ToolManager.s_settings.strSDDirectory.Substring(0, 2)}";
        strContent += $"\ncd \"{ToolManager.s_settings.strSDDirectory}\"";
        strContent += "\nactivate ldm";
        strContent += $"\nset \"TRANSFORMERS_CACHE={Application.dataPath}/../ai_cache/huggingface/transformers\"";
        strContent += $"\nset \"TORCH_HOME={Application.dataPath}/../ai_cache/torch\"";
        strContent += $"\npython scripts/dream.py --web";
        strContent += "\nstart \"\" http://localhost:9090";

        try
        {
            File.WriteAllText($"{Application.dataPath}/../start_web_version.bat", strContent);
        }
        catch
        {
            Debug.Log("Could not create web bat.");
        }
    }
}
