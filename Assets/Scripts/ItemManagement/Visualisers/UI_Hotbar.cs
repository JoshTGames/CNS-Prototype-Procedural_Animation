using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Hotbar : MonoBehaviour{    
    [SerializeField] GameObject slotPrefab;
    [SerializeField] string inventoryName;
    Animator[] slotAnimators = new Animator[0];

    Animator anim;
    bool isOpen = false; // HANDLES IF UI IS OPEN OR NOT

    private void Awake(){
        GM_Utilities.current.UpdatePlr += UpdateHands;
    }
    private void Start(){        
        GM_Utilities.current.UpdateInv += UpdateVars;
        anim = GetComponent<Animator>();
    }

    void UpdateVars(Transform obj, string _inventoryName, Dictionary<int, List<Item>> inventory = null){
        if(_inventoryName != inventoryName) { return; }
        inventory = inventory ?? obj.GetComponent<Inventory>().ReadAll(obj.tag); // ASSIGN IF NULL

        // CHECKING IF NUMBER OF SLOTS HAVE CHANGED...
        Animator[] tempSlots = (slotAnimators == null || slotAnimators.Length < inventory.Keys.Count)? _ = new Animator[inventory.Keys.Count] : null; // IF NUMBER OF SLOTS IS DIFFERENT TO WHAT WE HAVE...
        if(tempSlots != null && tempSlots.Length > 0){
            for (int i = 0; i < slotAnimators.Length; i++){ tempSlots[i] = slotAnimators[i]; } // POPULATING NEW SLOTS WITH OLDER ONES
        }
        // IF SLOTANIMATORS LENGTH > TEMP SLOTS THEN DELETE OBJECTS PAST TEMPSLOTS LENGTH        
        for (int i = (tempSlots != null)? tempSlots.Length: 0; i < ((slotAnimators != null && slotAnimators.Length >0)? slotAnimators.Length: 0); i++){ 
            if(slotAnimators != null && slotAnimators[i].gameObject != null){
                GameObject temp = slotAnimators[i].gameObject;
                slotAnimators[i] = null;
                Destroy(temp);
            }            
        }
        slotAnimators = (tempSlots != null && tempSlots.Length >0)? tempSlots : slotAnimators;

        // ITERATE THROUGH ALL VALUES, CHECK IF ANY CHANGED. IF SO THEN EITHER CHANGE VALUES INSIDE VISUAL SLOT OR DELETE IT
        foreach (KeyValuePair<int, List<Item>> item in inventory){

            Visuals_SlotItemHolder sIH;
            try { sIH = slotAnimators[item.Key].GetComponent<Visuals_SlotItemHolder>(); }
            catch (System.Exception){ sIH = null; }
            
            if(!sIH){ // IF SLOT DOESN'T EXIST
                GameObject clonedSlot = Instantiate(slotPrefab, transform);
                slotAnimators[item.Key] = clonedSlot.GetComponent<Animator>();
                sIH = clonedSlot.GetComponent<Visuals_SlotItemHolder>();
            }
            sIH.item = item.Value[0];
            sIH.quantity = item.Value.Count;
        }
        isOpen = true;
    }

    int selectedSlot = 0;
    int prvSlot = 0;


    Transform hands;
    void UpdateHands(Transform plr){ hands = plr.GetChild(0).GetChild(1); }

    
    void Update(){
        #region SLOT SELECTION
        if (slotAnimators == null || slotAnimators.Length <= 0) { return; }

        selectedSlot = Mathf.Clamp(Mathf.FloorToInt((isOpen)? selectedSlot + Input.mouseScrollDelta.y : selectedSlot), 0, slotAnimators.Length-1);      
        slotAnimators[(prvSlot != selectedSlot)? prvSlot : selectedSlot].SetBool("IsSelected", (prvSlot != selectedSlot) ? false : true); // If prvSlot doesnt = selected slot then it'll switch off prvSlot until it does equal selectedSlot in which case it'll turn on selectedSlot


        if(prvSlot != selectedSlot && hands != null) {            
            GM_Utilities.current.EquipItem(hands, slotAnimators[selectedSlot].GetComponent<Visuals_SlotItemHolder>().item);
            prvSlot = selectedSlot;
        }        
        #endregion
        #region HOTBAR OPENING/CLOSING
        if (Input.GetKeyDown(KeyCode.Tab) && transform.childCount >0){ isOpen = !isOpen; }
        else if(transform.childCount <= 0) { isOpen = false; }

        if(isOpen != anim.GetBool("IsVisible")){ 
            anim.SetBool("IsVisible", isOpen);
            if(hands != null) { GM_Utilities.current.EquipItem(hands, (isOpen)? slotAnimators[selectedSlot].GetComponent<Visuals_SlotItemHolder>().item : null); } // Unequips an item
        }
        #endregion        
    }
}
