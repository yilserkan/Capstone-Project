using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Controls;
using System;
using TMPro;
using Mirror;

public class Computer : NetworkBehaviour
{
    [SerializeField] private ComputerSlotParent computerSlotParent;
    [SerializeField] private GameObject computerUI;
    [SerializeField] private TMP_Text resistorValue;

    [SerializeField] private Slot slot;

    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private PlayerCameraController cameraController;
    [SerializeField] private PlayerItemInteractController ItemInteractController;

    [SyncVar]
    private bool computerInUse;
    public bool SetComputerInUse{ set{ computerInUse = value; } }

    private void Start()   
    {
        PlayerItemInteractController.InteractedWithComputer += HandleInteractedWithComputer;
    }

    private void OnDestroy() 
    {
        PlayerItemInteractController.InteractedWithComputer -= HandleInteractedWithComputer;
    }

    private void HandleInteractedWithComputer(Transform interactedItemTransform, Transform playerTransform)
    {
        if(interactedItemTransform != gameObject.transform || computerInUse) { return; }

        movementController = playerTransform.GetComponent<PlayerMovementController>();
        cameraController = playerTransform.GetComponent<PlayerCameraController>();
        ItemInteractController = playerTransform.GetComponent<PlayerItemInteractController>();

        Debug.Log("Interacted with Coputer");
        resistorValue.text = computerSlotParent.ComputerScreenValue;


        computerUI.SetActive(true);
        movementController.enabled = false;
        cameraController.enabled = false;
        ItemInteractController.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;

        if(isClientOnly)
        {
            ItemInteractController.CmdSetComputerInUse(this, true);
            return;
        }

        computerInUse = true;
    }


    public void ComputerUIUpButton()
    {
        if(slot.GetSlotItem.item == null) { return; }

        string tempStringResistorValue = resistorValue.text;
        int tempIntResistorValue = System.Convert.ToInt32(tempStringResistorValue);
        tempIntResistorValue++;
        resistorValue.text = tempIntResistorValue.ToString();

        if(isClientOnly)
        {   
            ItemInteractController.CmdChangeComputerScreenValue(computerSlotParent, resistorValue.text);
            return;
        }
        computerSlotParent.ComputerScreenValue = resistorValue.text;
    }

    public void ComputerUIDownButton()
    {
        if (slot.GetSlotItem.item == null) { return; }

        string tempStringResistorValue = resistorValue.text;
        int tempIntResistorValue = System.Convert.ToInt32(tempStringResistorValue);
        tempIntResistorValue--;
        resistorValue.text = tempIntResistorValue.ToString();

        if (isClientOnly)
        {
            ItemInteractController.CmdChangeComputerScreenValue(computerSlotParent, resistorValue.text);
            return;
        }

        computerSlotParent.ComputerScreenValue = resistorValue.text;
    }

    public void ComputerUIDoneButton()
    {   
        computerUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        movementController.enabled = true;
        cameraController.enabled = true;
        ItemInteractController.enabled = true;

        if(isServer)
        {
            computerInUse = false;
        }
        else
        {
            ItemInteractController.CmdSetComputerInUse(this, false);
        }

        if (slot.GetSlotItem.item == null) { return; }

        int tempIntResistorValue = System.Convert.ToInt32(resistorValue.text);

        if(isClientOnly)
        {
            ItemInteractController.CmdSetComputerScreenValue(slot, tempIntResistorValue);
            return;
        }

        slot.GetSlotItem.item.GetComponent<Item>().data = tempIntResistorValue;
        
    }

}
