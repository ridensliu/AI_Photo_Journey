using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LevelBus : MonoBehaviour
{
    public UnityEvent onStartGenerateImage;
    public UnityEvent onPictureGenerated;
    public UnityEvent onChangePrompt;
    
    private void Start()
    {
        NewImageGenerator.Instance.onStartGenerating.AddListener(() =>
        {
            onStartGenerateImage.Invoke();
        });
        NewImageGenerator.Instance.onPictureGenerated.AddListener(_ =>
        {
            onPictureGenerated.Invoke();
        });
        FirstPersonController.Instance.GetComponent<PhotoTaker>().onChangePrompt.AddListener(() =>
        {
            onChangePrompt.Invoke();
        });
    }

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

    public void UpdateTipsText(string content)
    {
        PromptTextUI.Instance.UpdateTipsText(content);
        PromptTextUI.Instance.SetOnlyTips(false);
    }

    public void ShowOnlyTips(string content)
    {
        PromptTextUI.Instance.UpdateTipsText(content);
        PromptTextUI.Instance.SetOnlyTips(true);
    }

    public void ShowPromptTextUI()
    {
        PromptTextUI.Instance.Show();
    }

    public void HidePromptTextUI()
    {
        PromptTextUI.Instance.Hide();
    }

    public void SetAllowGenerate(bool state)
    {
        NewImageGenerator.Instance.SetAllowGenerating(state);
    }

    public void SetPlayerFreezeMovement(bool state)
    {
        FirstPersonController.Instance.SetFreezeMovement(state);
    }

    public void SetPlayerGravity(float g)
    {
        FirstPersonController.Instance.Gravity = g;
    }
    
    public void SetPlayerJumpHeight(float h)
    {
        FirstPersonController.Instance.JumpHeight = h;
    }
}
