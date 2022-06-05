using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace FPS.Controls
{
    public class PlayerItemInteractController : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float maxInteractDistance;
        [SerializeField] private LayerMask itemLayerMask;
        [SerializeField] private LayerMask computerLayerMask;
        [SerializeField] private LayerMask itemDropLayerMask;
        [SerializeField] private LayerMask buttonLayerMask;
        [SerializeField] private LayerMask inputFieldLayerMask;
        [SerializeField] private Transform playerItemTransform;
        [SerializeField] private Material transparentMaterial;

        public Transform GetPlayerItemTransform{ get { return playerItemTransform; } }

        [SyncVar]
        [SerializeField]private Slot lastSlot;

        [SyncVar]
        private Interactable playerItem;

        private GameObject itemPlaceholder;

    
        [SyncVar]
        private Vector3 itemPlaceHolderPosition;

        [SyncVar]
        private Vector3 itemPlaceholderDirection;

        public Vector3 GetItemPlaceholder{ get { return itemPlaceHolderPosition; } }

        private Controls controls;
        private Controls Controls
        {
            get
            {
                if (controls != null)
                {
                    return controls;
                }
                return controls = new Controls();
            }
        }

        private RaycastHit hit;

        public static event Action<Transform, Transform> InteractedWithComputer;
        public static event Action<Transform, Transform> InteractedWithButton;
        public static event Action<Transform, Transform> InteractedWithInputField;

        public override void OnStartAuthority()
        {
            enabled = true;

            Controls.Player.Interact.performed += ctx => HandleInteractInput();
        }

        private void OnEnable() => Controls.Enable();

        private void OnDisable() => Controls.Disable();

        private void Update() => DisplayItemPlaceholder();

        private void HandleInteractInput()
        {
            //Check if interacted with Computer
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance, computerLayerMask))
            {
                InteractedWithComputer?.Invoke(hit.transform, transform);
            }

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance, buttonLayerMask))
            {
                InteractedWithButton?.Invoke(hit.transform, transform);
            }

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance, inputFieldLayerMask))
            {
                Debug.Log("HEyoooooooooo");
                InteractedWithInputField?.Invoke(hit.transform, transform);
            }

            //Player Doesn't already has Picked Up an Item. Pick Item UP
            if(!playerItem)
            {
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance, itemLayerMask))
                {
                    // if(playerItem.transform.parent)
                    playerItem = hit.transform.gameObject.GetComponent<Interactable>();

                    // CmdChangePlayerItem();

                    if (playerItem != null)
                    {  
                        if(isServer)
                        {
                            playerItem.RpcRemoveItemFromSlot();
                            playerItem.RpcSetItemParent(transform);
                        }
                        else if(isClient)
                        {
                            CmdRemoveItemFromSlot(playerItem);
                            CmdSetItempParent(playerItem, transform);
                        }
                        
                        // CallSetItemParent(playerItem.gameObject, transform);
                    }
                }
                return;
            }
            
            //Player has already an Item picked Up. Drop Item

            //Check oif player is looking at a place in reach to place the object
            if(!itemPlaceholder.activeSelf) { return; }

            if(lastSlot != null)
            {
                // CmdAddItemToList();
                if(isServer)
                {
                    lastSlot.AddItemToList(playerItem);
                    playerItem.RpcSetItemParent(lastSlot.transform);
                }
                else if(isClient)
                {
                    CmdAddItemToList(lastSlot, playerItem);
                    CmdSetItempParent(playerItem, lastSlot.transform);
                }

                //playerItem.SetItemParent(lastSlot.transform);
                lastSlot = null;
            }
    
            else
            {
                if(isServer)
                {
                    playerItem.ResetItemsParent();
                }
                else if(isClient)
                {
                    CmdResetItemsParent(playerItem);
                }
            }

            // playerItem.PlaceObjectAtPosition(itemPlaceholder.transform, itemPlaceholder.transform.up);

            itemPlaceHolderPosition = itemPlaceholder.transform.position;
            itemPlaceholderDirection = itemPlaceholder.transform.up;

            CallPlaceObjectAtPosition(playerItem.gameObject, itemPlaceHolderPosition ,itemPlaceholderDirection);

            Destroy(itemPlaceholder);
            playerItem = null;
        }

        private void DisplayItemPlaceholder()
        {   
            //If Player has no Item
            if(!playerItem) { return; } 

            if(!itemPlaceholder)
            {
                itemPlaceholder = Instantiate(playerItem.ItemPrefab);
                itemPlaceholder.transform.localScale = playerItem.transform.localScale;
            }

            // if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance, itemDropLayerMask))
            // {
            //     itemPlaceholder.GetComponentInChildren<MeshRenderer>().material = transparentMaterial;
            //     //Placing item in a Slot 
            //     if(hit.transform.tag == "SlotParent")
            //     {
            //         Slot[] hitSlots = hit.transform.GetComponentsInChildren<Slot>();
            //         for (int i = 0; i < hitSlots.Length; i++)
            //         {
            //             if(Vector3.Distance(hitSlots[i].transform.position, hit.point) < .5f)
            //             {
            //                 Vector3 itemPlaceholderNewPosition = hitSlots[i].GetPositionOfItemSlot(playerItem);
            //                 if (itemPlaceholderNewPosition != Vector3.zero)
            //                 {
            //                     // itemPlaceholder.transform.rotation = hit.transform.rotation;
            //                     itemPlaceholder.transform.up = hit.normal.normalized;
            //                     itemPlaceholder.transform.position = itemPlaceholderNewPosition;
            //                     lastSlot = hitSlots[i];
            //                     itemPlaceholder.SetActive(true);
            //                     return;
            //                 }
            //             }
            //             lastSlot = null;
            //         }

            //     }

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance, itemDropLayerMask))
            {
                itemPlaceholder.GetComponentInChildren<MeshRenderer>().material = transparentMaterial;
                //Placing item in a Slot 
               
                Slot hitSlot = hit.transform.GetComponent<Slot>();

                if (hitSlot != null)
                {
                    Vector3 itemPlaceholderNewPosition = hitSlot.GetPositionOfItemSlot(playerItem);
                    if (itemPlaceholderNewPosition != Vector3.zero)
                    {
                        // itemPlaceholder.transform.rotation = hitSlot.transform.rotation;
                        itemPlaceholder.transform.up = hit.normal.normalized;
                        itemPlaceholder.transform.position = itemPlaceholderNewPosition;
                        lastSlot = hitSlot;
                        itemPlaceholder.SetActive(true);
                        return;
                    }
                }
                lastSlot = null;
                

                //Placing item somewhere in the world
                itemPlaceholder.transform.position =  hit.point;

                itemPlaceholder.transform.up = hit.normal.normalized;

                itemPlaceholder.SetActive(true);
                
                return;
            }
           
            itemPlaceholder.SetActive(false);

        }

 

        #region Pick Up Item
        public void CallSetItemParent(GameObject item, Transform parent)
        {
            if (!hasAuthority)
            {
                return;
            }

            // CmdSetItempParent(item, parent);
        }

        [Command]
        public void CmdSetItempParent(Interactable _item, Transform _parent)
        {
            _item.RpcSetItemParent(_parent);
            // RpcSetItemParent(item, parent);
        }

        [ClientRpc]
        public void RpcSetItemParent(GameObject item, Transform parent)
        {
            item.GetComponent<Interactable>().GetParentGameObject = parent.GetComponent<PlayerItemInteractController>().GetPlayerItemTransform.gameObject;
            item.transform.parent = parent.GetComponent<PlayerItemInteractController>().GetPlayerItemTransform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
        #endregion

        #region Drop Item
        private void CallPlaceObjectAtPosition(GameObject item , Vector3 pos, Vector3 up)
        {
            if(!hasAuthority) { return; }

            CmdPlaceObjectAtPosition(item,pos, up);
        }  

        [Command]
        private void CmdPlaceObjectAtPosition(GameObject item, Vector3 pos, Vector3 up)
        {
            RpcPlaceObjectAtPosition(item,pos, up);
        }

        [ClientRpc]
        private void RpcPlaceObjectAtPosition(GameObject item ,Vector3 pos, Vector3 up)
        {
            item.transform.position = pos;
            item.transform.up = up;
        }
        #endregion

        #region Cmd Calls

        [Command]
        public void CmdChangeAuthority()
        {
            playerItem.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
            Debug.Log(playerItem.connectionToClient.connectionId);
        }

        [Command]
        private void CmdAddItemToList(Slot _lastSlot, Interactable _playerItem)
        {
            _lastSlot.AddItemToList(_playerItem);
        }

        [Command]
        private void CmdResetItemsParent(Interactable _playerItem)
        {
            _playerItem.RpcResetItemsParent();
        }

        [Command]
        private void CmdRemoveItemFromSlot(Interactable _playerItem)
        {
            _playerItem.RpcRemoveItemFromSlot();
        }

        [Command]
        public void CmdChangeComputerScreenValue(ComputerSlotParent computerSlotParent, string resistorValue)
        {
            computerSlotParent.ComputerScreenValue = resistorValue;
        }

        [Command]
        public void CmdSetComputerScreenValue(Slot slot, int value)
        {
            slot.GetSlotItem.item.GetComponent<Item>().data = value;
        }

        [Command]
        public void CmdSetComputerInUse(Computer computer ,bool newValue)
        {
            computer.SetComputerInUse = newValue;
        }

        [Command]
        public void CmdSetInputFieldInUse(InputFieldQuestion inputFieldQuestion, bool newValue)
        {
            inputFieldQuestion.SetInUse = newValue;
        }

        [Command]
        public void CmdHandleInteractedWithButton(MultipleChoice multipleChoice, int buttonID)
        {
            multipleChoice.CheckAnswer(buttonID);
        }

        [Command]
        public void CmdHandleInteractedWithInputField(InputFieldQuestion inputFieldQuestion)
        {
            inputFieldQuestion.CheckAnswer();
        }

        [Command]
        public void CmdUpdateInGameInputFieldString(InputFieldQuestion inputFieldQuestion, string value)
        {
            inputFieldQuestion.CallUpdateInGameInputFieldString(value);
        }

        #endregion

    }
}

