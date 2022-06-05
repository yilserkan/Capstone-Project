using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Controls;
using Mirror;
using TMPro;

public class MultipleChoice : NetworkBehaviour
{
    [SerializeField] private TMP_Text question;
    [SerializeField] private TMP_Text questionA;
    [SerializeField] private TMP_Text questionB;
    [SerializeField] private TMP_Text questionC;
    [SerializeField] private TMP_Text questionD;

    [SyncVar(hook = nameof(HandleUpdatedQuestionString))]
    private string questionString;
    [SyncVar(hook = nameof(HandleUpdatedQuestionAString))]
    private string questionAString;
    [SyncVar(hook = nameof(HandleUpdatedQuestionBString))]
    private string questionBString;
    [SyncVar(hook = nameof(HandleUpdatedQuestionCString))]
    private string questionCString;
    [SyncVar(hook = nameof(HandleUpdatedQuestionDString))]
    private string questionDString;

    [SerializeField] private GameObject mctParent;

    [SerializeField] private QuestionHandler questionHandler;

    private List<string> questionValues = new List<string>();

    public static event Action QuestionSolved;

    [SyncVar]
    private string answer;

    private void OnEnable()
    {
        QuestionHandler.QuestionValuesReadMCT += HandleQuestionValuesRead;
    }

    private void OnDisable()
    {
        QuestionHandler.QuestionValuesReadMCT -= HandleQuestionValuesRead;
    }

    private void HandleQuestionValuesRead()
    {
        if (!isServer) { return; }
        Invoke(nameof(CreateQuestion), 5f);
        // CreateModel();
    }

    private void CreateQuestion()
    {
        questionValues = questionHandler.GetMCTValuesFromDatabase;
        questionString = questionValues[0];
        questionAString = questionValues[1];
        questionBString = questionValues[2];
        questionCString = questionValues[3];
        questionDString = questionValues[4];
        answer = questionValues[5];

        SetActiveParent(true);
    }

    public void HandleInteractedWithButton(Transform playerTransform, int buttonID)
    {
        if(isServer)
        {
            CheckAnswer(buttonID);
        }
        else
        {
            playerTransform.GetComponent<PlayerItemInteractController>().CmdHandleInteractedWithButton(this ,buttonID);
        }
    }    

    public void CheckAnswer(int buttonID)
    {
        Debug.Log("PressedButton");
        switch (buttonID)
        {
            case 1:
                if(questionA.text == answer)
                {
                    HandleQuestionSolved();
                }
                break;
            case 2:
                if (questionB.text == answer)
                {
                    HandleQuestionSolved();
                }
                break;
            case 3:
                if (questionC.text == answer)
                {
                    HandleQuestionSolved();
                }
                break;
            case 4:
                if (questionD.text == answer)
                {
                    HandleQuestionSolved();
                }
                break;
        }
    }

    private void HandleQuestionSolved()
    {
        QuestionSolved?.Invoke();
        SetActiveParent(false);
    }

    [ClientRpc]
    public void SetActiveParent(bool value)
    {
        mctParent.SetActive(value);
    }



    private void HandleUpdatedQuestionString(string oldValue, string newValue)
    {
        question.text = questionString;
    }

    private void HandleUpdatedQuestionAString(string oldValue, string newValue)
    {
        questionA.text = questionAString;
    }

    private void HandleUpdatedQuestionBString(string oldValue, string newValue)
    {
        questionB.text = questionBString;
    }

    private void HandleUpdatedQuestionCString(string oldValue, string newValue)
    {
        questionC.text = questionCString;
    }

    private void HandleUpdatedQuestionDString(string oldValue, string newValue)
    {
        questionD.text = questionDString;
    }

}
