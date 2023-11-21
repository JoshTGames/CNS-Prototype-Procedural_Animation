using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal{
    public Dictionary<string, int> sGoals;
    public bool remove;

    public SubGoal(string s, int i, bool r){
        sGoals = new Dictionary<string, int>();
        sGoals.Add(s, i);
        remove = r;
    }
}

public class GAgent : MonoBehaviour{
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction curAction;
    SubGoal curGoal;


    public void Start(){
        GAction[] acts = GetComponents<GAction>();
        foreach(GAction a in acts){
            actions.Add(a);
        }
    }

    bool invoked = false;
    void CompleteAction(){
        curAction.running = false;
        curAction.PostPerform();
        invoked = false;
    }

    private void LateUpdate(){
        if(curAction != null && curAction.running){
            if(curAction.agent.hasPath && curAction.agent.remainingDistance < curAction.distanceToFinish){
                if (!invoked){
                    Invoke("CompleteAction", curAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        if(planner == null || actionQueue == null){
            planner = new GPlanner();

            var sortedGoals = from entry in goals orderby entry.Value descending select entry;
            foreach(KeyValuePair<SubGoal, int> sg in sortedGoals){
                actionQueue = planner.plan(actions, sg.Key.sGoals, null);
                if(actionQueue != null){
                    curGoal = sg.Key;
                    break;
                }
            }
        }
        if(actionQueue != null && actionQueue.Count == 0) {
            if (curGoal.remove){ goals.Remove(curGoal); }
            planner = null;
        }

        if(actionQueue != null && actionQueue.Count > 0){
            curAction = actionQueue.Dequeue();
            if (curAction.PrePerform()){
                if(curAction.target == null && curAction.targetTag != ""){ curAction.target = GameObject.FindWithTag(curAction.targetTag); }
                if(curAction.target != null){ 
                    curAction.running = true;
                    curAction.agent.SetDestination(curAction.target.transform.position);
                }
            }
            else{ actionQueue = null; }
        }
    }
}
