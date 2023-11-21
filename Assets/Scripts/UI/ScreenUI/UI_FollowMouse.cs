using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FollowMouse : MonoBehaviour{
    
    void Update(){ transform.position = Input.mousePosition; }
}
