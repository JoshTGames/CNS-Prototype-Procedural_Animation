using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public class WorldState{
    public string key;
    public int value;
}
public class WorldStates{
    public Dictionary<string, int> states;
    public WorldStates(){ states = new Dictionary<string, int>(); }




    public bool HasState(string key){ return states.ContainsKey(key); } // IF THE STATE EXISTS IN THE STATES DICTIONARY *AS A KEY* THEN RETURN TRUE ELSE FALSE

    public void AddState(string key, int value){ states.Add(key, value); } // SIMPLY ADDS THE STATE TO THE STATES DICTIONARY
    public void RemoveState(string key){ 
        if (HasState(key)){ states.Remove(key); }
    } // SIMPLY REMOVES THE STATE TO THE STATES DICTIONARY

    public void ModifyState(string key, int value){
        if (HasState(key)){
            states[key] += value;
            if(states[key] <= 0) { RemoveState(key); }
        }
        else { AddState(key, value); }
    } // SIMPLY CHECKS IF STATE EXISTS, IF SO THEN ADD/SUBTRACT THE EXISTING VALUE WITH THE NEW VALUE
    public void SetState(string key, int value){ 
        if (HasState(key)){ states[key] = value; }
        else { AddState(key, value); }
    } // THIS WILL OVERRIDE THE EXISTING VALUE (IF IT DOES EXIST) WITH THE PASSED VALUE OTHERWISE IF IT DOESNT EXIST, IT'LL ADD THE STATE


    public Dictionary<string, int> GetStates(){ return states; } // SIMPLY GRABS ALL THE STATES AND RETURNS THEM TO CALLER
}
