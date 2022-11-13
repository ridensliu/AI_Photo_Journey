using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundMover : MonoBehaviour
{
    public Vector3[] targetPositions = new Vector3[2];
    public int currentIndex;
    public UnityEvent onMoveStart;
    public UnityEvent onMoveComplete;
    public float delayTime = 1f;
    public float tweenTime = 3f;
    public LeanTweenType tweenType = LeanTweenType.easeInOutCubic;

    public void GoToNextPosition()
    {
        if (currentIndex + 1 < targetPositions.Length) currentIndex++;
        
        LeanTween.move(gameObject, targetPositions[currentIndex], tweenTime)
            .setDelay(delayTime)
            .setEase(tweenType)
            .setOnStart(HandeMoveStart)
            .setOnComplete(HandleMoveComplete);
    }

    private void HandeMoveStart()
    {
        onMoveStart.Invoke();
    }

    private void HandleMoveComplete()
    {
        onMoveComplete.Invoke();
    }

    private void Reset()
    {
        targetPositions = new[]
        {
            transform.position,
            transform.position + Vector3.up * 10f
        };
    }

#if UNITY_EDITOR
    private Collider _collider;
    
    private void OnValidate()
    {
        if (targetPositions.Length < 2)
        {
            Reset();
        }
        else
        {
            targetPositions[0] = transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (_collider == null) _collider = GetComponent<Collider>();

        Gizmos.color = new Color(0.49f, 0.48f, 0.78f, 0.5f);
        foreach (var pos in targetPositions)
        {
            Gizmos.DrawWireCube(pos, _collider.bounds.size);
        }
    }
#endif
}
