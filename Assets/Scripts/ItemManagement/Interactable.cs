using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour{
    [SerializeField] Item objectType;
    [SerializeField] float interactRadius;

    [SerializeField] Vector3 equippedOffset;
    public Vector3 GetEquippedOffset() { return equippedOffset; }

    Vector3 localDefaultPosition;
    public Vector3 GetDefaultPos(){ return localDefaultPosition; }

    public int quantity;


    public float GetRadius(){ return interactRadius; }
    public Item GetObjectType(){ return objectType; }

    public bool CanPickup(Transform obj){        
        Collider[] objs = Physics.OverlapSphere(transform.position, interactRadius, 1 << obj.gameObject.layer);        
        foreach (Collider item in objs){            
            if (item.transform == obj){ return true; }
        }
        return false;
    }

    private void Start(){
        if (transform.GetComponentInParent<HandController>()){
            localDefaultPosition = transform.parent.localPosition + equippedOffset;
            transform.localPosition = localDefaultPosition;
        }
    }

    private void OnDrawGizmosSelected(){
        #region DEBUG: EQUIPPED
        if (transform.GetComponentInParent<HandController>() && Application.isEditor){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.parent.position + equippedOffset, .1f);
        }
        #endregion
        #region DEBUG: PICKUP RADIUS
        else{ 
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
        #endregion
    }

}
