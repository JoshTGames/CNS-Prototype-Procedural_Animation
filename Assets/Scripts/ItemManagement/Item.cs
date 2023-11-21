using UnityEngine;
using System;
using TMPro;

[CreateAssetMenu(fileName = "NewObject", menuName = "ScriptableObjects/InventoryManagement/Create Item")]
public class Item : ScriptableObject{
    [Tooltip("This is the physical item itself")] public GameObject itemPrefab;
    [Tooltip("This is item presented to the player in world space form")] public GameObject physicsItemPrefab;
    [Tooltip("This is what is used inside storage")] public Sprite itemImage;
    [TextArea] public string itemDescription;
    [Tooltip("Helps the system figure out how to handle this object")] public ItemType itemType;
    [Tooltip("Possibly the rarity of the item?")] public VertexGradient itemColour;
    public ItemStats stats;
    [Tooltip("Max you can hold of this item")] public int maxStack = 1;

    

    [Serializable] public enum ItemType{        
        Weapon,
        Consumable,
        Resource,
        Ammunition
    }
}