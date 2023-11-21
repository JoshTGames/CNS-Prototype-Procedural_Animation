using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FaceCam : MonoBehaviour{
    Camera cam;
    private void Start(){
        cam = Camera.main;
    }
    void Update(){
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }
}
