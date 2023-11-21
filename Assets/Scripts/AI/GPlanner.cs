using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Node{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public GAction action;

    public Node(Node parent, float cost, Dictionary<string, int> allStates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);
        this.action = action;
    }
}

public class GPlanner{
   public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates states){
        List<GAction> usableActions = new List<GAction>();
        foreach(GAction a in actions){ // ITERATES THROUGH ALL AVAILABLE ACTIONS
            if (a.IsAchievable()){ // ENSURES ONLY ACTIONS WHICH *CAN* BE ACHIEVED WILL BE PASSED INTO THE LIST
                usableActions.Add(a);
            }
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, GWorld.Instance.GetWorld().GetStates(), null); // Defining the starting node -- Mostly consists of null parameters because of this

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success){
            Debug.Log("ERR: No Plan found!");
            return null;
        }

        // FINDS THE CHEAPEST NODE TO REACH TO
        Node cheapest = null;
        foreach(Node leaf in leaves){
            if(cheapest == null){ cheapest = leaf; }
            else if(leaf.cost < cheapest.cost) { cheapest = leaf; }
        }

        // THIS IS REVERSE ENGINEERING THE LIST OF NODES - ITERATING THROUGH THE PARENTS TO GET THE RESULT ACTIONS REQUIRED TO GET TO THE GOAL
        Queue<GAction> result = new Queue<GAction>();
        Node n = cheapest;
        while(n != null){
            if(n.action != null){ result.Enqueue(n.action); }
            n = n.parent;
        }
        result = new Queue<GAction>(result.Reverse());


        // DELETE THIS
        //Debug.Log("The plan is:");
        //foreach(GAction a in result) { Debug.Log("Q: " + a.actionName); }

        return result;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal){
        bool foundPath = false;
        foreach (GAction a in usableActions){
            if (a.IsAchievableGiven(parent.state)){ // CHECK IF THE CUR ACTION IS ACHIEVABLE GIVEN THE CURRENT PARENT STATE
                Dictionary<string, int> curState = new Dictionary<string, int>(parent.state);
                foreach (KeyValuePair<string, int> affects in a.affects){
                    if (!curState.ContainsKey(affects.Key)){ curState.Add(affects.Key, affects.Value); } // ADD THE AFFECT OF THE CURRENT ACTION TO THE STATES LIST
                }

                Node node = new Node(parent, parent.cost + a.cost, curState, a);
                if(GoalAchieved(goal, curState)){
                    leaves.Add(node);
                    foundPath = true;
                }
                else{
                    List<GAction> subset = ActionSubset(usableActions, a);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found){ foundPath = true; }
                }
            }            
        }
        return foundPath;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe){
        List<GAction> subset = new List<GAction>();
        foreach (GAction a in actions){
            if (!a.Equals(removeMe)){
                subset.Add(a);
            }
        }
        return subset;
    } // THIS FUNCTION IS USED TO REMOVE ACTIONS FROM AN ACTION LIST

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state){
        foreach (KeyValuePair<string, int> g in goal) {
            if (!state.ContainsKey(g.Key)){ return false; }
        }
        return true;
    } // COMPARES ALL STATES IN A GOAL AND CHECKS IF ALL THE SUBGOALS EXIST + COMPLETED UNTIL THE GOAL IS DECLARED ACHIEVED
}
