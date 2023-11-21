using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour{
    public Transform[] hands;
    Vector3[] defaultPositions; // STORES LOCAL POSITIONS   


    void GetDefaults(){
        defaultPositions = new Vector3[hands.Length];
        for (int i = 0; i < hands.Length; i++) { defaultPositions[i] = hands[i].localPosition; }
    }

    private void Start(){
        // SET DEFAULT POSITIONS OF HANDS
        GetDefaults();
        GM_Utilities.current.EquipManagement += EquipItem;
    }

    GameObject clonedItem = null;
    void EquipItem(Transform _hands, Item item = null){        
        Interactable[] existingItem = _hands.GetComponentsInChildren<Interactable>();
        foreach (Interactable curItem in existingItem){
            Destroy(curItem.gameObject);
        } // IF AN ITEM IS BEING HELD...
        
        if (hands == null || hands.Length <= 0) { 
            hands = new Transform[_hands.childCount];
            for (int i = 0; i < _hands.childCount; i++){ hands[i] = _hands.GetChild(i); }
            GetDefaults();
        } // GETS HANDS SETUP
        if (item){ clonedItem = Instantiate(item.itemPrefab, _hands); } // INSTANTIATE NEW OBJECT IN HANDS
    }


    private void Update(){
        if (hands.Length <= 0) { return; }
        // RAW HAND MOVEMENT
        if (!clonedItem && hands[0].localPosition != defaultPositions[0]){ // AND NOT PUNCHING
            for (int i = 0; i < hands.Length; i++) { hands[i].localPosition = defaultPositions[i]; }            
        }
        else if(clonedItem){ // IF HANDS NOT AT GUN GRIPS MOVE THEM TO GUN GRIPS
            for (int i = 0; i < clonedItem.transform.childCount; i++){ 
                hands[i].position = clonedItem.transform.GetChild(i).position;                
            }
        }


        // ARM PIVOTING
            // AIM HANDS TO TRACK ENEMIES DIRECTLY (THUS AIMING WEAPON)
            // IF NO ENEMIES THEN SLIGHTLY AIM WEAPON DOWN
    }
}
