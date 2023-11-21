using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FaceTarget : MonoBehaviour{
    [Serializable] private class PartToRotate{
        public Transform item;
        public float rotSpeed;
        
        public bool lockXRotation, lockYRotation, lockZRotation;

        public Vector2 xAngleClamp, yAngleClamp, zAngleClamp;

        public Transform dependencyObj;        
    }
    [SerializeField] private PartToRotate[] part;

    [SerializeField] bool faceMouse;
    public Transform target;
    Vector3 targetPos = Vector3.zero;


    
    private void Update(){
        if (faceMouse){
            RaycastHit hit;
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camRay, out hit, 100)){ targetPos = hit.point; }            
        }
        else if (target) { targetPos = target.position; }
        
        if(faceMouse || target){
            RotateToPoint(targetPos);
        }
    }

    #region USEFUL FUNCTIONS
    void RotateToPoint(Vector3 pos){
        foreach (PartToRotate curPart in part){

            Quaternion sRot;
            if (curPart.dependencyObj){ sRot = Quaternion.Slerp(curPart.item.rotation, curPart.dependencyObj.rotation, Time.deltaTime * curPart.rotSpeed); } // ROTATES TO DEPENDENCY
            else{ // ROTATES TO TARGET
                Quaternion targetRot = Quaternion.LookRotation(pos - curPart.item.position);

                Vector3 clampedRot = new Vector3(
                    clampAngle(targetRot.eulerAngles.x, curPart.xAngleClamp.x, curPart.xAngleClamp.y),
                    clampAngle(targetRot.eulerAngles.y, curPart.yAngleClamp.x, curPart.yAngleClamp.y),
                    clampAngle(targetRot.eulerAngles.z, curPart.zAngleClamp.x, curPart.zAngleClamp.y)
                );


                targetRot = Quaternion.Euler(clampedRot);
                sRot = Quaternion.Slerp(curPart.item.rotation, targetRot, Time.deltaTime * curPart.rotSpeed);
            }

            if (curPart.lockXRotation) { sRot.x = curPart.item.rotation.x; }
            if (curPart.lockYRotation) { sRot.y = curPart.item.rotation.y; }
            if (curPart.lockZRotation) { sRot.z = curPart.item.rotation.z; }
            curPart.item.rotation = sRot;
        }
    }
    float clampAngle(float angle, float min, float max){
        if (angle > 180f) { angle -= 360; }
        angle = Mathf.Clamp(angle, min, max);

        if (angle < 0f) { angle += 360; }
        return angle;
    }
    #endregion
}
