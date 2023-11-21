using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour{
    [Tooltip("The spawn location will be based off this offset")] public Transform offsetObject;
    [Serializable] public class Positioning{ public Vector3 position, targetRotation; }
    public Positioning aimedOffset, relaxedOffset, locomotionOffset;


    [SerializeField] float debugRadius = .25f;


    private void OnDrawGizmosSelected(){
        if (!offsetObject) { return; }

        // AIMED OFFSET
        Gizmos.color = Color.red;        
        Gizmos.DrawSphere(aimedOffset.position, debugRadius);
        Gizmos.DrawLine(aimedOffset.position, aimedOffset.targetRotation);

        // RELAXED OFFSET
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(relaxedOffset.position, debugRadius);
        Gizmos.DrawLine(relaxedOffset.position, relaxedOffset.targetRotation);

        // LOCOMOTION OFFSET
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(locomotionOffset.position, debugRadius);
        Gizmos.DrawLine(locomotionOffset.position, locomotionOffset.targetRotation);
    }

    private void OnValidate(){
        if (Application.isEditor){ // ONLY CALLS IF IN UNITY EDITOR

        }
    }
}
