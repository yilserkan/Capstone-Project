using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FPS.Controls;

public class Interactable : NetworkBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private GameObject parentGameobject;

    public GameObject GetParentGameObject { get { return parentGameobject; } set { parentGameobject = value; } }

    public GameObject ItemPrefab { get{return itemPrefab;} }
    
    private Transform pTransform;

    [ClientRpc]
    public void RpcSetItemParent(Transform parentTransform)
    {
        Vector3 localScale = transform.localScale;
        parentGameobject = parentTransform.gameObject;
        if(parentTransform.GetComponent<PlayerItemInteractController>())
        {
            transform.parent = parentTransform.gameObject.GetComponent<PlayerItemInteractController>().GetPlayerItemTransform;
        }
        else
        {
            transform.parent = parentTransform;
        }
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = localScale;
    }

    [ClientRpc]
    public void RpcPlaceObjectAtPosition(Transform placeholderTransform, Vector3 up)
    {
        transform.position = placeholderTransform.position;
        transform.up = up;
        // transform.rotation = placeholderTransform.rotation;
        // transform.eulerAngles = new Vector3(0,0,270f);
    }

    [ClientRpc]
    public void ResetItemsParent()
    {
        parentGameobject = null;
        transform.parent = null;
    }

    [ClientRpc]
    public void RpcRemoveItemFromSlot()
    {
        if(parentGameobject == null) { return; }
        Slot parentSlot = parentGameobject.GetComponent<Slot>();
        if(parentSlot != null)
        {
            parentSlot.RemoveItem();
        }
    }

    [ClientRpc]
    public void RpcResetItemsParent()
    {
        parentGameobject = null;
        transform.parent = null;
    }
}
