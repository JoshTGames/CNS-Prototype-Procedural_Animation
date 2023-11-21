using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZombie : GAgent{
    private void Start(){
        base.Start();
        SubGoal s1 = new SubGoal("ArrivedAtPlr", 1, true);
        goals.Add(s1, 3);
    }
}
