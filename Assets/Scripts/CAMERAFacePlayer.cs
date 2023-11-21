using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMERAFacePlayer : MonoBehaviour{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset, zoomOffset;
    [SerializeField] Vector2 zoomFOV;
    [SerializeField] private float speed;

    bool useZoomOffset = false;
    Camera cam;

    private void Awake(){
        cam = GetComponent<Camera>();

        if (!GM_Utilities.current) { return; }
        GM_Utilities.current.UpdatePlr += UpdateTarget;
        GM_Utilities.current.Cam_Zoom += Zoom;
    }
    
    private void Update(){        
        if (!target) { return; }
        transform.position = Vector3.Lerp(transform.position, target.position + ((useZoomOffset)? zoomOffset : offset), Time.deltaTime / speed);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, (useZoomOffset)? zoomFOV.y : zoomFOV.x, Time.deltaTime / speed);
        transform.LookAt(target);
    }


    // EVENTS
    private void Zoom(bool zoom){ useZoomOffset = zoom; }
    private void UpdateTarget(Transform newTarget){ target = newTarget; }

    private void OnDrawGizmosSelected(){
        if (!target) { return; }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(target.position + offset, .2f);
        Gizmos.DrawWireSphere(target.position + zoomOffset, .2f);
    }
}


