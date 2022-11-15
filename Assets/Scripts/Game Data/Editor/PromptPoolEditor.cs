using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class PromptPoolEditor : OdinMenuEditorWindow
{
    [MenuItem("Tools/Prompt Pool Editor")]
    public static void OpenWindow()
    {
        GetWindow<PromptPoolEditor>().Show();
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        tree.AddAllAssetsAtPath("", "Assets/Game Data/Prompt Pool", typeof(PromptPool));
        
        return tree;
    }
}
