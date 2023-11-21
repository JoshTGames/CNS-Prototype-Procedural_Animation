using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Hover : MonoBehaviour{    
    void Update(){
        Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.rotation = q;
    }
}
