using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Controls;

public class InputFieldCheckButton : MonoBehaviour
{
    [SerializeField] private InputFieldQuestion inputFieldQuestion;

    [SerializeField] private Transform inputFieldDoneButtonCollider;

    private void Start()
    {
        PlayerItemInteractController.InteractedWithInputField += HandleInteractedWithButton;
    }

    private void OnDestroy()
    {
        PlayerItemInteractController.InteractedWithInputField -= HandleInteractedWithButton;
    }

    private void HandleInteractedWithButton(Transform interactedItemTransform, Transform playerTransform)
    {
        if (interactedItemTransform != inputFieldDoneButtonCollider ) { return; }
        Debug.Log("Interacted With Button");
        inputFieldQuestion.HandleInteractedWithButton(playerTransform);
    }
}
