using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelBus : MonoBehaviour
{
    public void SetPlayerInputState(bool state)
    {
        var input = FirstPersonController.Instance.GetComponent<PlayerInput>();
        
        if (state)
        {
            input.ActivateInput();
        }
        else
        {
            input.DeactivateInput();
        }
    }

    public void SetContentPrompt(string value)
    {
        NewImageGenerator.Instance.SetContentPrompt(value);
        StartCoroutine(DelayUpdateText());
    }

    public void SetStylePrompt(string value)
    {
        NewImageGenerator.Instance.SetStylePrompt(value);
    }

    private IEnumerator DelayUpdateText()
    {
        yield return null;
        PromptTextUI.Instance.SetPromptText(NewImageGenerator.Instance.GetPromptText());
        PromptTextUI.Instance.Show();
    }

    public void GetRandomTextFromPool(PromptPool pool)
    {
        NewImageGenerator.Instance.GetRandomTextFromPool(pool);
        PromptTextUI.Instance.SetPromptText(NewImageGenerator.Instance.GetPromptText());
        PromptTextUI.Instance.Show();
    }
}
