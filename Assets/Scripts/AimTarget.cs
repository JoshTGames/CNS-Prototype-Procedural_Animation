using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTarget : MonoBehaviour{
    [SerializeField] bool faceMouse;
    public Transform target;
    [SerializeField] float rotateSpeed;

    public bool lockXRotation, lockYRotation, lockZRotation;
    public Vector2 xAngleClamp, yAngleClamp, zAngleClamp;

    public Transform dependencyObj;

    Interactable interactable;

    Vector3 targetPos = Vector3.zero;

    [SerializeField] Vector3 idleRotation;
    Vector3 defaultGunPos { get { return (interactable != null)? transform.parent.position + transform.parent.TransformDirection(interactable.GetDefaultPos()) : transform.position; } }
    Vector3 gunIdlePos { get { return (interactable != null) ? transform.parent.position + transform.parent.TransformDirection(interactable.GetDefaultPos() + idleRotation) : transform.position + idleRotation; } }



    private void Start(){
        if (!dependencyObj) { dependencyObj = GM_Utilities.current.GetPlayer().GetChild(0); }
        interactable = GetComponent<Interactable>();
    }

    private void Update(){
        if (!dependencyObj) { return; }

        bool doesClamp = false;
        if (faceMouse && Input.GetMouseButton(1)){
            doesClamp = true;
            RaycastHit hit;
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camRay, out hit, 100)) { targetPos = hit.point; }
        }
        else if (target) {
            doesClamp = true;
            targetPos = target.position; 
        }
        else{ targetPos = gunIdlePos; }
        
        if (faceMouse){ GM_Utilities.current.ZoomOnTarget(Input.GetMouseButton(1)); } // FACE MOUSE BASICALLY INDICATES THAT THIS OBJ IS CONTROLLED BY A PLAYER -- SO CALL EVENT IF TRUE
        transform.localRotation = RotateToTarget(targetPos, doesClamp);
    }

    Quaternion RotateToTarget(Vector3 _targetPos, bool doesClamp = false){
        #region GET DIRECTION       
        Vector3 dir = _targetPos - transform.position;
        Quaternion target = Quaternion.LookRotation(dir.normalized, transform.up);
        Quaternion targetRot = Quaternion.Inverse(dependencyObj.rotation) * target;
              
        Vector3 clampedRot = new Vector3(
            (doesClamp)? clampAngle(targetRot.eulerAngles.x, xAngleClamp.x, xAngleClamp.y) : targetRot.eulerAngles.x,
            (doesClamp)? clampAngle(targetRot.eulerAngles.y, yAngleClamp.x, yAngleClamp.y) : targetRot.eulerAngles.y,
            clampAngle(targetRot.eulerAngles.z, zAngleClamp.x, zAngleClamp.y)
        );
        targetRot = Quaternion.Euler(clampedRot);
        #endregion
        #region INTERPOLATION
        Quaternion slerp = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
        if (lockXRotation) { slerp.x = transform.rotation.x; }
        if (lockYRotation) { slerp.y = transform.rotation.y; }
        if (lockZRotation) { slerp.z = transform.rotation.z; }
        #endregion
        return slerp;
    } // WORLD SPACE > LOCAL SPACE

    float clampAngle(float angle, float min, float max){
        if (angle > 180f) { angle -= 360; }
        angle = Mathf.Clamp(angle, min, max);

        if (angle < 0f) { angle += 360; }
        return angle;
    }


    private void OnDrawGizmos(){        
        Gizmos.color = Color.white;        
        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.DrawWireSphere(targetPos, .1f);
    }
}
