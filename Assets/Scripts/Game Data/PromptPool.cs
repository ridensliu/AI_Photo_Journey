using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game Data/Prompt Pool", fileName = "NewPromptPool")]
public class PromptPool : ScriptableObject
{
    [TableList(ShowIndexLabels = true)]
    public PromptInfo[] infos;
    
    [Serializable]
    public struct PromptInfo
    {
        public string content;
        public string style;
    }
}
