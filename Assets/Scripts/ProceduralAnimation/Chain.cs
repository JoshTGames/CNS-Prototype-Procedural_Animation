using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chain : MonoBehaviour{
    [Serializable] private class ChainSettings {
        [Tooltip("Distance between each node")] public float targetDist;
        [Tooltip("Smaller the value, faster the points will movetowards each other")] public float smoothSpeed;

        public List<Transform> nodes;
        [HideInInspector] public List<Vector3> nodeVel;
        [HideInInspector] public List<Vector3> actualNodeVel;
    }
    [SerializeField] private ChainSettings chainSettings;



    private void Awake(){
        for (int i = 0; i < chainSettings.nodes.Count; i++){
            chainSettings.nodeVel.Add(Vector3.zero);
            chainSettings.actualNodeVel.Add(Vector3.zero);
        }
    }

    private void Update(){
        for (int i = 1; i < chainSettings.nodes.Count; i++){            
            Vector3 curVel = chainSettings.nodeVel[i];
            Vector3 targetPos = chainSettings.nodes[i - 1].position + (chainSettings.nodes[i].position - chainSettings.nodes[i - 1].position).normalized * chainSettings.targetDist;
            Vector3 newPos = Vector3.SmoothDamp(chainSettings.nodes[i].position, targetPos, ref curVel, chainSettings.smoothSpeed);

            Vector3 velocity = (chainSettings.nodes[i].position - chainSettings.actualNodeVel[i]);
            chainSettings.actualNodeVel[i] = velocity;
            
            chainSettings.nodes[i].LookAt(chainSettings.nodes[i-1]);
            chainSettings.nodes[i].position = newPos;
            chainSettings.nodeVel[i] = curVel;
        }
    }
}
