using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhotoTaker : MonoBehaviour
{
    public UnityEvent onTakePhoto;
    public UnityEvent onClosePicture;
    public UnityEvent onToggleTask;
    public UnityEvent onChangePrompt;

    private void OnTakePhoto()
    {
        if (!GeneratorConnection.instance.bInitialized) return;
        
        onTakePhoto.Invoke();
    }

    private void OnClosePreview()
    {
        if (!GeneratorConnection.instance.bInitialized) return;
        
        onClosePicture.Invoke();
    }

    private void OnToggleTask()
    {
        if (!GeneratorConnection.instance.bInitialized) return;
        
        onToggleTask.Invoke();
    }

    private void OnChangePrompt()
    {
        if (!GeneratorConnection.instance.bInitialized) return;
        onChangePrompt.Invoke();
    }
}
