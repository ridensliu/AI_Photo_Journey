using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoader : MonoBehaviour
{
    public string targetScene;
    public bool unloadThisScene = true;

    public void Load()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);

        yield return null;
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
        
        if (!unloadThisScene) yield break;
        
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
