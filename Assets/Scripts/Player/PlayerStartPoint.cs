using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerStartPoint : MonoBehaviour
{
    private void Start()
    {
        UpdatePlayerPosition();
        // StartCoroutine(SetPlayerPosition());
    }

    private IEnumerator SetPlayerPosition()
    {
        UpdatePlayerPosition();
        yield return null;
        UpdatePlayerPosition();
    }

    private void UpdatePlayerPosition()
    {
        FirstPersonController.Instance.SetFreezeMovement(true);
        FirstPersonController.Instance.transform.position = transform.position;
        FirstPersonController.Instance.SetFreezeMovement(false);
    }
}
