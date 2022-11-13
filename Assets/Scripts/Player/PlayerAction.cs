using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAction : MonoBehaviour
{
    public UnityEvent onEscape;
    
    private void OnEscape()
    {
        onEscape.Invoke();
    }
}
