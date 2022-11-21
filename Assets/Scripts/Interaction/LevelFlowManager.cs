using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class LevelFlowManager : MonoBehaviour
{
    [TableList(ShowIndexLabels = true)]
    public StageEvent[] stages;

    [ShowInInspector, ReadOnly]
    private int _currentStage;
    
    [Serializable]
    public struct StageEvent
    {
        public UnityEvent onReachStage;
    }

    public void Progress()
    {
        stages[_currentStage++].onReachStage.Invoke();
        NewImageGenerator.Instance.SetAllowGenerating(false);
    }
}
