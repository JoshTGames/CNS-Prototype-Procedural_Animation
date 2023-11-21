using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour{
    [Serializable] private class AccessSettings{        
        public string[] tag;
    }
    [SerializeField] AccessSettings accessSettings;

    [Tooltip("This is the reference name of the inventory")] public string inventoryName;
    [Tooltip("The size of the inventory")] public int slots;
    
    private Dictionary<int, List<Item>> inventory = new Dictionary<int, List<Item>>(); // INT IS THE SLOT NUMBER, AND THEN WE WILL HOLD A LIST OF OBJECTS, IF LIST EXCEEDS MAX STACK OF OBJECT, THEN CREATE A NEW KEY

    [SerializeField] Item.ItemType[] exemptItemTypes; // ANYTHING IN THIS ARRAY CANNOT BE PICKED UP BY THIS OBJECT SCRIPT

    public bool IsAccessible(string tag){
        bool found = false;
        foreach (string curTag in accessSettings.tag){
            if(curTag == tag) { 
                found = true; 
                break; 
            }
        }
        return (found || accessSettings.tag.Length == 0) ? true : false;
    } // RETURNS IF THIS STORAGE IS ACCESSIBLE OR NOT
    public Dictionary<int, List<Item>> ReadAll(string tag){
        return (IsAccessible(tag)) ? inventory : null;
    } // RETURNS THE INVENTORY OF THE OBJECT


    private bool CanAcceptItem(Item.ItemType itemType){              
        foreach (Item.ItemType item in exemptItemTypes){
            if (itemType == item) { return false; }
        }
        return true;
    } // THIS MAKES SURE ONLY ITEMS OF X TYPE CAN BE ADDED TO INVENTORY  


    private int Add(int slotIndex, Item obj, int amt){
        int fillAmt = 0;
           
        if (inventory[slotIndex].Count + amt > obj.maxStack){ // IF OVERFLOWING WITH NEW AMT...            
            fillAmt = obj.maxStack - inventory[slotIndex].Count;  // FILL UP SLOT TO MAX...               
        }
        else{ fillAmt = amt; } // THIS MEANS THAT THERE IS NO OVERFLOW

        for (int x = 0; x < fillAmt; x++) { inventory[slotIndex].Add(obj); }
        return amt -= fillAmt; // RETURNS THE EXCESS; IF 0, NOTHING WILL HAPPEN OTHERWISE THE PARENT FUNCTION WILL TRY CREATE A NEW SLOT
    } // ADDS OBJS TO SLOT
    private void Remove(int slotIndex, int amt){
        foreach (Item obj in inventory[slotIndex]){ 
            if(amt <= 0) { break; }
            inventory[slotIndex].Remove(obj);
            amt--;
        }
    } // REMOVES OBJS FROM SLOT
    public bool Swap(int slotIndexA, int slotIndexB){
        if (inventory.ContainsKey(slotIndexB)){
            List<Item> tempItem = inventory[slotIndexB];
            inventory[slotIndexB] = inventory[slotIndexA];
            inventory[slotIndexA] = tempItem;
            UpdateVisuals();
            return true;
        }
        return false;
    } // SWAPS ITEM A WITH B IF B EXISTS (IN LOCAL INVENTORY!) -- THIS WONT WORK CROSS-INVENTORY

    private void RemoveSlot(int slotIndex){ inventory.Remove(slotIndex); } // REMOVES AN EXISTING VALUE   

    public int UpdateInventory(Item obj, int amt){        
        int tempAmt = amt; // USED TO UPDATE VISUALS
        if (CanAcceptItem(obj.itemType)) { 
            for (int i = 0; i < slots; i++) {
                if (inventory.ContainsKey(i) && inventory[i][0].name == obj.name){ // IF KEY EXISTS and is of the same object...
                    if (inventory[i].Count + tempAmt <= 0) { RemoveSlot(i); } // DELETES ENTIRE SLOT FROM INVENTORY
                    else if (inventory[i].Count + tempAmt > inventory[i].Count){ // ADDING TO STACK
                        tempAmt = Add(i, obj, tempAmt); // EXCESS                     
                    } 
                    else if (inventory[i].Count + tempAmt < inventory[i].Count) { Remove(i, tempAmt); } // REMOVING FROM STACK                
                }

                if(!inventory.ContainsKey(i) && tempAmt > 0){ // CREATING NEW SLOT IF NO OBJECT IS PRESENT IN THAT SLOT                                                      
                    Item[] newObjs = new Item[Mathf.Clamp(tempAmt, 0, obj.maxStack)];
                    for (int x = 0; x < newObjs.Length; x++) { 
                        newObjs[x] = obj; 
                    }

                    inventory.Add(i, newObjs.ToList());
                    tempAmt -= newObjs.Length;
                }
            }
        } // IF ITEM CAN BE ACCEPTED...

        if (tempAmt != amt){ UpdateVisuals(); } // TEMPAMT HAS CHANGED
        return tempAmt; // IF DOESN'T RETURN 0 THEN IT WASN'T SUCCESSFUL WITH ADDING ALL THE OBJECTS
    } // <-- PARENT FUNCTION CONTROLS WHATS HAPPENS TO A SLOT


    void UpdateVisuals(){
        GM_Utilities.current.UpdateInventory(GM_Utilities.current.GetPlayer().GetChild(0), inventoryName, inventory);
    }


    private void Start(){        
        GM_Utilities.current.GetItem += PickItem;
    }
    void PickItem(Inventory inv, Interactable item){
        if(inv.transform == this.transform){ // IF THE INVENTORY PARSED IS THIS SCRIPT...
            int excess = UpdateInventory(item.GetObjectType(), item.quantity);            
            if(excess <= 0){
                Destroy(item.gameObject);
                return;
            }            
            item.quantity = excess;            
        }
    }
}