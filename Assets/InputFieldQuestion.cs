using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Controls;
using System;
using Mirror;
using TMPro;

public class InputFieldQuestion : NetworkBehaviour
{
    [SerializeField] private GameObject inputFieldUI;
    [SerializeField] private Transform inputFieldColliderTransform;

    [SerializeField] private GameObject inputFieldParent;

    [SerializeField] private TMP_Text question;

    [SerializeField] private QuestionHandler questionHandler;

    [SyncVar(hook = nameof(HandleUpdatedQuestionString))]
    private string questionString;

    [SyncVar(hook = nameof(HandleUpdatedInputFieldString))]
    [SerializeField] private string inputFieldInGameTextString;

    [SerializeField] private TMP_Text inputFieldInGameText;
    [SerializeField] private TMP_InputField inputFieldUIText;

    private PlayerMovementController movementController;
    private PlayerCameraController cameraController;
    private PlayerItemInteractController ItemInteractController;

    [SyncVar]
    private bool inUse;
    public bool SetInUse { set { inUse = value; } }

    [SyncVar]
    private string answer;

    private List<string> questionValues = new List<string>();

    public static event Action QuestionSolved;

    private void Start()
    {
        PlayerItemInteractController.InteractedWithInputField += HandleInteractedWithInputField;
    }

    private void OnDestroy()
    {
        PlayerItemInteractController.InteractedWithInputField -= HandleInteractedWithInputField;
    }

    private void OnEnable()
    {
        QuestionHandler.QuestionValuesReadInputField += HandleQuestionValuesRead;
    }

    private void OnDisable()
    {
        QuestionHandler.QuestionValuesReadInputField -= HandleQuestionValuesRead;
    }

    private void HandleInteractedWithInputField(Transform interactedItemTransform, Transform playerTransform)
    {
        if(interactedItemTransform != inputFieldColliderTransform || inUse) { return; }

        movementController = playerTransform.GetComponent<PlayerMovementController>();
        cameraController = playerTransform.GetComponent<PlayerCameraController>();
        ItemInteractController = playerTransform.GetComponent<PlayerItemInteractController>();

        if(inputFieldInGameTextString.Equals(""))
        {
            inputFieldUIText.text = string.Empty;
        }
        else
        {
            inputFieldUIText.text = inputFieldInGameText.text;
        }


        Debug.Log("Interacted with Input Field");

        inputFieldUI.SetActive(true);
        movementController.enabled = false;
        cameraController.enabled = false;
        ItemInteractController.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;

        if (isClientOnly)
        {
            ItemInteractController.CmdSetInputFieldInUse(this, true);
            return;
        }

        inUse = true;
    }

    private void HandleQuestionValuesRead()
    {
        if (!isServer) { return; }
        Invoke(nameof(CreateQuestion), 5f);
    }

    private void CreateQuestion()
    {
        questionValues = questionHandler.GetInputFieldValuesFromDatabase;
        questionString = questionValues[0];
        answer = questionValues[1];

        inputFieldInGameText.text = "Enter Answer";

        SetActiveParent(true);
    }

    public void HandleInteractedWithButton(Transform playerTransform)
    {
        if (isServer)
        {
            CheckAnswer();
        }
        else
        {
            playerTransform.GetComponent<PlayerItemInteractController>().CmdHandleInteractedWithInputField(this);
        }
    }

    public void CheckAnswer()
    {
        if(inUse) { return; }

        if(inputFieldInGameTextString.ToLower().Equals(answer.ToLower()))
        {
            HandleQuestionSolved();
        }
    }

    private void HandleQuestionSolved()
    {
        SetActiveParent(false);
        inputFieldInGameTextString = "";
        QuestionSolved?.Invoke();
    }

    public void InputFieldUIDoneButton()
    {
        inputFieldUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        movementController.enabled = true;
        cameraController.enabled = true;
        ItemInteractController.enabled = true;

        if (isServer)
        {
            inUse = false;
        }
        else
        {
            ItemInteractController.CmdSetInputFieldInUse(this, false);
        }
    }

    [ClientRpc]
    public void SetActiveParent(bool value)
    {
        inputFieldParent.SetActive(value);
    }

    public void UpdateInGameInputFieldString(string value)
    {
        if(isServer)
        {
            inputFieldInGameTextString = value;
        }
        else
        {
            ItemInteractController.CmdUpdateInGameInputFieldString(this ,value);
        }
    }

    public void CallUpdateInGameInputFieldString(string value)
    {
        inputFieldInGameTextString = value;
    }

    private void HandleUpdatedQuestionString(string oldValue, string newValue)
    {
        question.text = questionString;
    }

    private void HandleUpdatedInputFieldString(string oldValue, string newValue)
    {
        inputFieldInGameText.text = newValue;
    }
}
