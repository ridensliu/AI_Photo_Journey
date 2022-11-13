using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    public bool hideSelfAfterTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter.Invoke();
            if (hideSelfAfterTrigger) gameObject.SetActive(false);
        }
    }
}
