using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Visuals_SlotItemHolder : MonoBehaviour{
    public Item item;
    public int quantity;

    Image img;
    TextMeshProUGUI label;

    private void Start(){ 
        img = transform.GetChild(0).GetComponent<Image>();
        label = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update(){
        if (!item) { return; }
        img.sprite = item.itemImage;
        if(item.maxStack > 1 && label.gameObject.activeSelf == false){ label.gameObject.SetActive(true); }
        else if(item.maxStack <=1 && label.gameObject.activeSelf == true){ label.gameObject.SetActive(false); }
        label.text = ($"{quantity}x");
    }
}
