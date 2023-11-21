using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : MonoBehaviour{
    Vector3 prevPos;

    [HideInInspector]public Vector3 value;
    private void Update(){
        value = (transform.position - prevPos) / Time.deltaTime;
        prevPos = transform.position;
    }
}
