using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : NetworkBehaviour
{
    [SyncVar]
    public ItemTypes ItemType;

    [SyncVar]
    public int data;
}
