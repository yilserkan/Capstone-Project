using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Controls;
using System;

public class MultipleChoiceButton : MonoBehaviour
{
    [SerializeField] private MultipleChoice multipleChoice;

    [SerializeField] private int buttonID;

    private void Start()
    {
        PlayerItemInteractController.InteractedWithButton += HandleInteractedWithButton;
    }

    private void OnDestroy()
    {
        PlayerItemInteractController.InteractedWithButton -= HandleInteractedWithButton;
    }

    private void HandleInteractedWithButton(Transform interactedItemTransform, Transform playerTransform)
    {
        if(interactedItemTransform != gameObject.transform) { return; }
        Debug.Log("Interacted With Button");
        multipleChoice.HandleInteractedWithButton(playerTransform ,buttonID);
    }
}
