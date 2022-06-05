using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class QuestionHandler : NetworkBehaviour
{
    [SerializeField] private List<string> questionPool;
    [SerializeField] private List<GameObject> questionModels = new List<GameObject>();
    [SerializeField] private MultipleChoice multipleChoice;
    [SerializeField] private InputFieldQuestion inputFieldQuestion;

    [SerializeField] private List<int> slotValuesFromDatabase;
    [SerializeField] private string cbValuesFromDatabase;
    public string GetCbSlotValuesFromDatabase{ get { return cbValuesFromDatabase; } }
    public List<int> GetSlotValuesFromDatabase{ get { return slotValuesFromDatabase; } }

    [SerializeField] private List<string> mctValuesFromDatabase;
    public List<string> GetMCTValuesFromDatabase { get { return mctValuesFromDatabase; } }

    [SerializeField] private List<string> inputFieldValuesFromDatabase;
    public List<string> GetInputFieldValuesFromDatabase { get { return inputFieldValuesFromDatabase; } }


    public static event Action QuestionValuesReadCB;
    public static event Action QuestionValuesReadMCT;
    public static event Action QuestionValuesReadInputField;

    public override void OnStartServer()
    {
        // Get question from Database

        if(!isServer) { return; }
        
        Invoke(nameof(DisableCircuitBoards), 2f);

        CircuitBoard.QuestionSolved += HandleQuestionSolved;
        MultipleChoice.QuestionSolved += HandleQuestionSolved;
        InputFieldQuestion.QuestionSolved += HandleQuestionSolved;

        DatabaseConnection.questionsRead += HandleQuestionRead;

        StartCoroutine(DatabaseConnection.GetQuestions());

        // Invoke(nameof(GetQuesitonFromPool), 3f);
        // GetQuesitonFromPool();
    }

    private void OnDestroy() 
    {
        CircuitBoard.QuestionSolved -= HandleQuestionSolved;
        MultipleChoice.QuestionSolved -= HandleQuestionSolved;
        InputFieldQuestion.QuestionSolved -= HandleQuestionSolved;

        DatabaseConnection.questionsRead -= HandleQuestionRead;
    }

    private void GetQuesitonFromPool()
    {
        int questionIndex = UnityEngine.Random.Range(0, questionPool.Count);

        if(questionPool[questionIndex][0].Equals('0'))
        {
            ReadValuesFromDatabaseInput(questionIndex);
        }
        else if(questionPool[questionIndex][0].Equals('1'))
        {
            ReadValuesForMCTQuestion(questionIndex);
        }
        else if (questionPool[questionIndex][0].Equals('2'))
        {
            ReadValuesForInputFieldQuestion(questionIndex);
        }
        
        questionPool.RemoveAt(questionIndex);
    }

    private void ReadValuesForInputFieldQuestion(int index)
    {
        int indexForString = 2;
        string slotStringValue = "";

        for (int i = indexForString; i < questionPool[index].Length; i++)
        {
            if (questionPool[index][i].Equals('/'))
            {
                indexForString = i;
                inputFieldValuesFromDatabase.Add(slotStringValue);
                slotStringValue = "";
            }
            else if (i + 1 == questionPool[index].Length)
            {
                slotStringValue += questionPool[index][i];
                inputFieldValuesFromDatabase.Add(slotStringValue);
            }
            else
            {
                slotStringValue += questionPool[index][i];
            }
        }
        // questionModels[slotValuesFromDatabase[0]].SetActive(true);
        Invoke(nameof(InvokeQuestionValuesReadInputField), 1f);
    }

    private void ReadValuesForMCTQuestion(int index)
    {
        int indexForString = 2;
        string slotStringValue = "";

        for (int i = indexForString; i < questionPool[index].Length; i++)
        {
            if (questionPool[index][i].Equals('/'))
            {
                indexForString = i;
                mctValuesFromDatabase.Add(slotStringValue);
                slotStringValue = "";
            }
            else if (i + 1 == questionPool[index].Length)
            {
                slotStringValue += questionPool[index][i];
                mctValuesFromDatabase.Add(slotStringValue);
            }
            else
            {
                slotStringValue += questionPool[index][i];
            }
        }
        // questionModels[slotValuesFromDatabase[0]].SetActive(true);
        Invoke(nameof(InvokeQuestionValuesReadMCT), 1f);
    }

    private void HandleQuestionSolved()
    {
        if (!isServer) { return; }
        // questionModels[slotValuesFromDatabase[0]].SetActive(false);
        mctValuesFromDatabase.Clear();
        slotValuesFromDatabase.Clear();
        inputFieldValuesFromDatabase.Clear();
        if(questionPool.Count != 0)
        {
            GetQuesitonFromPool();
            return;
        }
        //All Questions Solved
    }

    private void HandleQuestionRead(string questions)
    {
        int indexForString = 0;
        string slotStringValue = "";

        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i].Equals('|'))
            {
                indexForString = i;
                questionPool.Add(slotStringValue);
                slotStringValue = "";
            }
            else
            {
                slotStringValue += questions[i];
            }
        }

        Invoke(nameof(GetQuesitonFromPool), 3f);
    }

    public void ReadValuesFromDatabaseInput(int index)
    {
        int indexForString = 2;
        string slotStringValue = "";
        
        for (int i = indexForString; i < questionPool[index].Length; i++)
        {
            if (questionPool[index][i].Equals('/'))
            {
                indexForString = i + 1;
                Debug.Log($"1----------{slotStringValue}");
                slotValuesFromDatabase.Add(System.Convert.ToInt32(slotStringValue));
                slotStringValue = "";
                break;
            }
            else
            {
                slotStringValue += questionPool[index][i];
            }
        }
        
        for (int i = indexForString; i < questionPool[index].Length; i++)
        {
            if (questionPool[index][i].Equals('/'))
            {
                indexForString = i + 1;
                Debug.Log($"2----------{slotStringValue}");
                cbValuesFromDatabase = slotStringValue;
                slotStringValue = "";
                break;
            }
            else
            {
                slotStringValue += questionPool[index][i];
            }
        }

        for (int i = indexForString; i < questionPool[index].Length; i++)
        {
            if (questionPool[index][i].Equals('/'))
            {
                indexForString = i;
                Debug.Log($"{i}----------{slotStringValue}");
                slotValuesFromDatabase.Add(System.Convert.ToInt32(slotStringValue));
                slotStringValue = "";
            }
            else if (i + 1 == questionPool[index].Length)
            {
                slotStringValue += questionPool[index][i];
                Debug.Log($"{i}----------{slotStringValue}");
                slotValuesFromDatabase.Add(System.Convert.ToInt32(slotStringValue));
            }
            else
            {
                slotStringValue += questionPool[index][i];
            }
        }
        // questionModels[slotValuesFromDatabase[0]].SetActive(true);
        EnableQuestionCircuitBoard();
        Debug.Log("Invoke Creating Model");
        Invoke(nameof(InvokeQuestionValuesReadCB), 2f);

    }

    private void InvokeQuestionValuesReadCB()
    {
        QuestionValuesReadCB?.Invoke();
    }

    private void InvokeQuestionValuesReadMCT()
    {
        QuestionValuesReadMCT?.Invoke();
    }

    private void InvokeQuestionValuesReadInputField()
    {
        QuestionValuesReadInputField?.Invoke();
    }


    private void DisableCircuitBoards()
    {
        for (int i = 0; i < questionModels.Count; i++)
        {
            questionModels[i].GetComponent<CircuitBoard>().SetParent(false);
        }
        multipleChoice.SetActiveParent(false);
        inputFieldQuestion.SetActiveParent(false);
    }

   
    private void EnableQuestionCircuitBoard()
    {
        Debug.Log(slotValuesFromDatabase[0]);
        questionModels[slotValuesFromDatabase[0]].GetComponent<CircuitBoard>().SetParent(true);
    }

}
