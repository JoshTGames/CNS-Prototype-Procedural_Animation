using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //-- CHANGE THIS TO A* PATHFINDING PRO LIBRARY

public abstract class GAction : MonoBehaviour{
    public string actionName = "Action";
    public float cost = 1.0f;
    public GameObject target; 
    public string targetTag;
    public float duration = 0;
    public float distanceToFinish = 1f;

    public WorldState[] preConditions, afterAffects;
    public Dictionary<string, int> pre_conditions, affects;

    public WorldStates agentBeliefs;

    public bool running = false;

    public NavMeshAgent agent;

    public GAction(){
        pre_conditions = new Dictionary<string, int>();
        affects = new Dictionary<string, int>();
    }


    private void Awake(){
        agent = this.gameObject.GetComponent<NavMeshAgent>(); // CHANGE THIS TO A* PATHFINDING PRO LIBRARY


        // GETTING ALL WORLD STATES AVAILABLE
        if (preConditions != null){
            foreach(WorldState w in preConditions){ pre_conditions.Add(w.key, w.value); }
        }

        if (afterAffects != null){
            foreach (WorldState w in afterAffects){ affects.Add(w.key, w.value); }
        }
    }

    public bool IsAchievable(){ return true; }

    public bool IsAchievableGiven(Dictionary<string, int> conditions){
        foreach(KeyValuePair<string, int> p in pre_conditions){
            if (!conditions.ContainsKey(p.Key)){
                return false;
            }
        }
        return true;
    } // CHECKS IF THIS ACTION IS ACHIEVABLE GIVEN THE CURRENT CONDITIONS

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}