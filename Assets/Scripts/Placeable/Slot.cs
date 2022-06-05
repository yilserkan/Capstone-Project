using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Slot : NetworkBehaviour
{
    [System.Serializable]
    public struct SlotItem
    {
        public ItemTypes ItemType;
        public Transform transform;
        public GameObject item;
        public bool startWithItem;
        public GameObject itemPrefab;
        public int value;
        public bool itemInserted;
    }

    [SerializeField] private SlotItem slot;
    [SerializeField] private GameObject textGameobject;

    public static event Action ItemInserted;
    public static event Action ItemRemoved;

    public SlotItem GetSlotItem { get{ return slot; } set{slot = value;} }

    [ClientRpc]
    public void AddItemToList(Interactable item)
    {
        // if(!isServer) { return ; }

        if(slot.ItemType == item.GetComponent<Item>().ItemType && slot.item == null)
        {
            var newItemPlaceHolder = slot;
            newItemPlaceHolder.item = item.gameObject;
            newItemPlaceHolder.itemInserted = true;
            slot = newItemPlaceHolder;
            EnableValueText();
            ItemInserted?.Invoke();
        }
    }

    public void RemoveItem()
    {
        var newItemPlaceHolderList = slot;
        newItemPlaceHolderList.item = null;
        newItemPlaceHolderList.itemInserted = true;
        slot = newItemPlaceHolderList;
        DisableValueText();
        ItemRemoved?.Invoke();
    }

    public Vector3 GetPositionOfItemSlot(Interactable item)
    {
        if(!item.GetComponent<Item>()) { return Vector3.zero; }
        if (slot.ItemType == item.GetComponent<Item>().ItemType && slot.item == null)
        {
            return slot.transform.position;
        }
        
        return Vector3.zero;
    }

    public void SetInsertedItemFalse()
    {
        var newItemPlaceHolder = slot;
        newItemPlaceHolder.itemInserted = false;
        slot = newItemPlaceHolder;
    }
    
    public void EnableValueText()
    {
        if (textGameobject == null || slot.item.GetComponent<Item>() == null)
        {
            return;
        }
        textGameobject.GetComponent<TMP_Text>().text = slot.item.GetComponent<Item>().data.ToString();
        textGameobject.SetActive(true);
    }
    
    [ClientRpc]
    public void EnableValueTextInitialized(int value)
    {
        if (textGameobject == null)
        {
            return;
        }
        textGameobject.GetComponent<TMP_Text>().text = value.ToString();
        textGameobject.SetActive(true);
    }

    public void DisableValueText()
    {
        if (textGameobject == null)
        {
            return;
        }
        textGameobject.SetActive(false);
    }
}
