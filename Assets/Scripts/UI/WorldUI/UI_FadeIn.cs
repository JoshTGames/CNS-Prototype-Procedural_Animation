using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeIn : MonoBehaviour{
    [SerializeField] Interactable interactable;
    [SerializeField] float duration;

    Image[] images; 
    private void Start(){
        images = GetComponentsInChildren<Image>();
    }

    void ShowImg(bool isActive){
        foreach(Image img in images){
            img.color = new Color(img.color.r, img.color.b, img.color.b, Mathf.MoveTowards(img.color.a, (isActive)? 1: 0, duration * Time.deltaTime));
        }
    }
    
    private void Update(){
        ShowImg(interactable.CanPickup(GM_Utilities.current.GetPlayer().GetChild(0)));        
    }
}
