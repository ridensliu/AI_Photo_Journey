using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoneShaker : MonoBehaviour
{
    public float shakeSpeed = 0.2f;
    public Vector3 shakeStrength = new Vector3(0, 0.1f, 0);

    private Vector3 _startPos;
    private float[] _perlinOffset;

    private void Start()
    {
        _startPos = transform.localPosition;
        _perlinOffset = new float[3];

        for (var i = 0; i < _perlinOffset.Length; i++)
        {
            _perlinOffset[i] = Random.Range(0f, 1000f);
        }
    }

    private void Update()
    {
        transform.localPosition = _startPos + new Vector3(
            Mathf.PerlinNoise(_perlinOffset[0], Time.time * shakeSpeed) * shakeStrength.x,
            Mathf.PerlinNoise(_perlinOffset[1], Time.time * shakeSpeed) * shakeStrength.y,
            Mathf.PerlinNoise(_perlinOffset[2], Time.time * shakeSpeed) * shakeStrength.z);
    }
}
