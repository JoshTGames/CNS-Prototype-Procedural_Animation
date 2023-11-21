using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;



public class GM_Utilities : MonoBehaviour{
    public static GM_Utilities current;
    
    

    // PLAYER STUFF
    private Transform player;  
    public Transform GetPlayer(){ return player; }

    [SerializeField] private GameObject playerObj;
    [SerializeField] private Transform spawnLoc;
    
    public event Action<Transform> UpdatePlr;
    public event Action<bool> Cam_Zoom;
    // PLAYER STUFF





    private void Awake(){
        current = this;

        if(player == null){
            GameObject newPlr = Instantiate(playerObj, (spawnLoc != null) ? spawnLoc.position : Vector3.zero, Quaternion.identity);
            player = newPlr.transform;
        } 
    }

    private void Start(){
        if (player) { UpdatePlr?.Invoke(player.GetChild(0)); }
    }

    public void ZoomOnTarget(bool zoomIn){ Cam_Zoom?.Invoke(zoomIn); }


    // GENERAL STUFF
    // THIS CAN BE CALLED FROM ANY CHARACTER CAPABLE OF PICKING UP ITEMS
    #region INVENTORY MANAGEMENT
    public event Action<Inventory, Interactable> GetItem;    
    public void PickItem(Inventory inv, Interactable item){ GetItem?.Invoke(inv, item); }


    public event Action<Transform, String, Dictionary<int, List<Item>>> UpdateInv; // SEARCH FOR THE ITEM AND UPDATE THE VISUALISERS APPROPRIATELY (Transform = Object, Inventory)
    public void UpdateInventory(Transform obj, string invName, Dictionary<int, List<Item>> inv){ UpdateInv?.Invoke(obj, invName, inv); }
    #endregion
    #region EQUIPPING MANAGEMENT
    public event Action<Transform, Item> EquipManagement;
    public void EquipItem(Transform hands, Item item = null){ EquipManagement?.Invoke(hands, item); }
    #endregion
    // GENERAL STUFF
}

