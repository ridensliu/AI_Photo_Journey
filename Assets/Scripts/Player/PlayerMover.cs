using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using StarterAssets;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [HorizontalGroup("Direction")]
    [LabelWidth(100)]
    public bool moveX;
    [HorizontalGroup("Direction")]
    [LabelWidth(100)]
    public bool moveY;
    [HorizontalGroup("Direction")]
    [LabelWidth(100)]
    public bool moveZ;
    
    [Space]
    public float time = 2f;
    public LeanTweenType tweenType;

    public void Move()
    {
        var go = FirstPersonController.Instance.gameObject;
        var position = go.transform.position;
        var targetPosition = transform.position;
        
        if (moveX) position.x = targetPosition.x;
        if (moveY) position.y = targetPosition.y;
        if (moveZ) position.z = targetPosition.z;
        
        LeanTween.move(go, position, time)
            .setEase(tweenType);
    }
}
