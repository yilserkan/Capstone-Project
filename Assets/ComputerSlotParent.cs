using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FPS.Controls;
using System;
using Mirror;

public class ComputerSlotParent : NetworkBehaviour
{
    [SerializeField] private Slot slot;
    [SerializeField] private TMP_Text computerScreenValue;

    [SyncVar(hook = nameof(HandleUpdatedComputerScreenValue))]
    private string computerScreenValueString;

    public string ComputerScreenValue { get{ return computerScreenValueString; } set{ computerScreenValueString = value; } }

    private void Start()
    {
        if(!isServer) { return; }

        Slot.ItemInserted += HandleItemInserted;
        Slot.ItemRemoved += HandleItemRemoved;
        computerScreenValueString = "None";
        // computerScreenValue.text = "None";
    }

    private void OnDestroy() 
    {
        if (!isServer) { return; }

        Slot.ItemInserted -= HandleItemInserted;
        Slot.ItemRemoved -= HandleItemRemoved;
    }

    private void HandleItemInserted()
    {
        if (!slot.GetSlotItem.itemInserted) { return; }
        slot.SetInsertedItemFalse();
        UpdateComputerScreenValue();
        // All Items are correctly inserted

        Debug.Log("Correct Item Inserted Computer");
    }

    private void HandleItemRemoved()
    {
        if (!slot.GetSlotItem.itemInserted) { return; }
        slot.SetInsertedItemFalse();
        computerScreenValueString = "None";
        // computerScreenValue.text = "None";
    }

    private void UpdateComputerScreenValue()
    {
        computerScreenValueString = slot.GetSlotItem.item.GetComponent<Item>().data.ToString();
        // computerScreenValue.text = slot.GetSlotItem.item.GetComponent<Item>().data.ToString();
    }

    private void HandleUpdatedComputerScreenValue(string oldValue, string newValue)
    {
        computerScreenValue.text = newValue;
    }

}
