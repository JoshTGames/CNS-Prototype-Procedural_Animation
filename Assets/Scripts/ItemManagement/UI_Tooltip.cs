using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_Tooltip : MonoBehaviour{
    [SerializeField] GameObject frame;
    [SerializeField] TextMeshProUGUI itemName, description;

    // STATS 
    [SerializeField] TextMeshProUGUI stat_DMG, stat_MAG, stat_ATKSPD, stat_RLDSPD;
    TextMeshProUGUI[] stats { get { return new TextMeshProUGUI[] {stat_DMG, stat_MAG, stat_ATKSPD, stat_RLDSPD}; }}
    // -----
    [SerializeField] Image itemImage;

    [Serializable] private class InteractSettings{
        public Image interactButton;
        public float interactDuration;
    }
    [SerializeField] InteractSettings interactSettings;

    bool canInteract = false;

    Interactable obj;

    private void Update(){
        interactSettings.interactButton.fillAmount = (Input.GetKey(KeyCode.E) && canInteract)? 
                Mathf.MoveTowards(interactSettings.interactButton.fillAmount, 0, interactSettings.interactDuration * Time.deltaTime):
                Mathf.MoveTowards(interactSettings.interactButton.fillAmount, 1, (interactSettings.interactDuration * 2) * Time.deltaTime);


        if(interactSettings.interactButton.fillAmount <= 0 && obj){
            GM_Utilities.current.PickItem(GM_Utilities.current.GetPlayer().GetChild(0).GetComponent<Inventory>(), obj); // REQUESTS SYSTEM TO PICK UP ITEM            
        }
    }




    void ShowTooltip(Interactable obj){
        itemName.text = obj.GetObjectType().name;
        itemName.colorGradient = obj.GetObjectType().itemColour;
        itemImage.sprite = obj.GetObjectType().itemImage;
        description.text = obj.GetObjectType().itemDescription;

        if(obj.GetObjectType().itemType == Item.ItemType.Weapon){
            stat_DMG.gameObject.SetActive(true);
            stat_DMG.text = "DMG: " + obj.GetObjectType().stats.weaponData.weaponDamage;

            stat_MAG.gameObject.SetActive(true);
            stat_MAG.text = "MAG: " + obj.GetObjectType().stats.weaponData.magCapacity;

            stat_ATKSPD.gameObject.SetActive(true);
            stat_ATKSPD.text = "ATK-SPD: " + obj.GetObjectType().stats.weaponData.fireRate;

            stat_RLDSPD.gameObject.SetActive(true);
            stat_RLDSPD.text = "RLD-SPD: " + obj.GetObjectType().stats.weaponData.reloadTime;
        }

        canInteract = true; // ALLOWS FOR OBJECT COLLECTION
        frame.SetActive(true);
    }
    void HideTooltip(){
        canInteract = false;
        foreach (TextMeshProUGUI label in stats){ label.gameObject.SetActive(false); }
        frame.SetActive(false); 
    }

    private void FixedUpdate(){
        RaycastHit hit;
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool found = false;
        if (Physics.Raycast(camRay, out hit, 100, 1 << LayerMask.NameToLayer("Item"))) {            
            if (hit.transform.GetComponent<Interactable>()){
                obj = hit.transform.GetComponent<Interactable>();
                if (obj && obj.CanPickup(GM_Utilities.current.GetPlayer().GetChild(0))){
                    ShowTooltip(obj);
                    found = true;
                }                
            }            
        }
        if(!found){ HideTooltip(); }
    }
}
