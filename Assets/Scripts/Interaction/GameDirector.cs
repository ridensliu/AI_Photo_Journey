using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    public static GameDirector Instance;
    
    public string startSceneName = "TowerLevelC";
    public UnityEvent onRestartGame;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        if (!SceneManager.GetSceneByName("GameScene").isLoaded)
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
        }

        if (!SceneManager.GetSceneByName(startSceneName).isLoaded)
        {
            SceneManager.LoadScene(startSceneName, LoadSceneMode.Additive);
        }

        yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(startSceneName));
    }

    [Button("Restart Game")]
    public void RestartGame()
    {
        NewImageGenerator.Instance.SetAllowGenerating(true);
        onRestartGame.Invoke();

        StartCoroutine(UnloadSceneAndRestart());
    }

    private IEnumerator UnloadSceneAndRestart()
    {
        var op1 = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        yield return new WaitUntil(() => op1.isDone);

        var op2 = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));
        
        yield return new WaitUntil(() => op2.isDone);

        var op3 = Resources.UnloadUnusedAssets();
        
        yield return new WaitUntil(() => op3.isDone);

        StartCoroutine(LoadScene());
    }
}
