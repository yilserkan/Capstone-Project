using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class CircuitBoard : NetworkBehaviour
{
    [SerializeField] private QuestionHandler questionHandler;
    [SerializeField] private List<Slot> slots = new List<Slot>();
    [SerializeField] private List<Transform> questionItemsSpawnTransforms = new List<Transform>();

    [SerializeField] private string stringFromDatabase;

    [SerializeField] private CircuitCover circuitCover;

    [SerializeField] private Transform parent;
    
    [SerializeField] private GameObject mctParent;
    
    [SerializeField] private TMP_Text questionText;
    
    [SyncVar(hook = nameof(HandleUpdatedQuestionString))]
    private string questionTextString;

    public Transform GetParent { get{ return parent; } }

    private List<int> slotValuesFromDatabase = new List<int>();

    public static event Action QuestionSolved;

    [ClientRpc]
    public void SetParent(bool value)
    {
        parent.gameObject.SetActive(value);
    }

    private void OnEnable() {
        Slot.ItemInserted += HandleItemInserted;
        QuestionHandler.QuestionValuesReadCB += HandleQuestionValuesRead;
    }

    private void OnDisable() 
    {
        Slot.ItemInserted -= HandleItemInserted;
        QuestionHandler.QuestionValuesReadCB -= HandleQuestionValuesRead;
    }

    private void HandleQuestionValuesRead()
    {
        Debug.Log("Creating Model Invoked");
        if (!isServer) { return; }
        Debug.Log("Creating Model");
        Invoke(nameof(CreateModel), 5f);
        // CreateModel();
    }

    private void HandleItemInserted()
    {
        if (!isServer) { return; }
        
        //slot.EnableValueText();

        // Control if all items are correctly inserted if not return
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].GetSlotItem.item == null || slots[i].GetSlotItem.item.GetComponent<Item>().data != slots[i].GetSlotItem.value )
            {
                return;
            }
        }

        // All Items are correctly inserted

        Debug.Log("Correct Item Inserted");

        circuitCover.CircuitCoverClosingAnimation();

        Invoke(nameof(CallQuestionSolved), 1f);
       
    }

    private void CallQuestionSolved()
    {
        DestroyItems();
        SetActiveParent(false);
        SetParent(false);
        QuestionSolved?.Invoke();
    }

    private void CreateModel()
    {
        Debug.Log("Start Creating Model");
        slotValuesFromDatabase = questionHandler.GetSlotValuesFromDatabase;
        int questionItemIndex = 0;
        
        questionTextString = questionHandler.GetCbSlotValuesFromDatabase;
        
        // Create Items for slots which are starting with items
        for (int i = 0; i < slots.Count; i++)
        {
            Slot slot = slots[i];
            var slotItem = slots[i].GetSlotItem;
            // First element == the model ID which is used in question handler therefore skip it
            slotItem.value = slotValuesFromDatabase[i+1];
            // slotItem.value = slotValuesFromDatabase[i];
            if (slots[i].GetSlotItem.startWithItem)
            {
                slotItem.item = Instantiate(slotItem.itemPrefab, slotItem.transform.position, slotItem.transform.rotation);
                NetworkServer.Spawn(slotItem.item);
                slotItem.item.GetComponent<Interactable>().RpcSetItemParent(slotItem.transform);
                slotItem.item.GetComponent<Item>().data = slotItem.value;
                slot.EnableValueTextInitialized(slotItem.value);
            }
            else
            {
                GameObject slotItemPrefab = Instantiate(slotItem.itemPrefab, questionItemsSpawnTransforms[questionItemIndex].position, Quaternion.identity);
                NetworkServer.Spawn(slotItemPrefab);
                questionItemIndex++;
                Debug.Log(transform.name);
            }
            slots[i].GetSlotItem = slotItem;

        }
        Debug.Log("Model Created");
        circuitCover.CircuitCoverOpeningAnimation();
        SetActiveParent(true);
    
    }

    private void DestroyItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetSlotItem.item != null)
            {
                Destroy(slots[i].GetSlotItem.item);
            }
        }
    }
    
    
    [ClientRpc]
    public void SetActiveParent(bool value)
    {
        mctParent.SetActive(value);
    }
    
    private void HandleUpdatedQuestionString(string oldValue, string newValue)
    {
        questionText.text = questionTextString;
    }

    // private void ReadValuesFromDatabaseInput()
    // {
    //     int indexForString = 0;
    //     string slotStringValue = "";

    //     for (int i = indexForString; i < stringFromDatabase.Length; i++)
    //     {
    //         if (stringFromDatabase[i].Equals('/'))
    //         {
    //             indexForString = i;
    //             slotValuesFromDatabase.Add(System.Convert.ToInt32(slotStringValue));
    //             slotStringValue = "";
    //         }
    //         else if (i + 1 == stringFromDatabase.Length)
    //         {
    //             slotStringValue += stringFromDatabase[i];
    //             slotValuesFromDatabase.Add(System.Convert.ToInt32(slotStringValue));
    //         }
    //         else
    //         {
    //             slotStringValue += stringFromDatabase[i];
    //         }
    //     }
    // }


}
