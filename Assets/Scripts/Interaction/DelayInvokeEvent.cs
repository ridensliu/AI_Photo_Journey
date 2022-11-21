using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayInvokeEvent : MonoBehaviour
{
    public float waitTime = 0.5f;
    public bool invokeOnEnable;
    public bool invokeOnce;
    public UnityEvent onReachTime;

    private bool _invoked;

    private void OnEnable()
    {
        if (invokeOnEnable)
        {
            Call();
        }
    }

    public void Call()
    {
        StartCoroutine(DelayInvokeUnityEvent(waitTime));
    }

    public void Call(float time)
    {
        StartCoroutine(DelayInvokeUnityEvent(time));
    }

    private IEnumerator DelayInvokeUnityEvent(float time)
    {
        if (invokeOnce && _invoked) yield break;
        
        yield return new WaitForSeconds(time);
        
        onReachTime.Invoke();
        _invoked = true;
    }
}
