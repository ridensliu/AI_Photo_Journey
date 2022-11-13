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
    }

    public void SetStylePrompt(string value)
    {
        NewImageGenerator.Instance.SetStylePrompt(value);
    }
}
