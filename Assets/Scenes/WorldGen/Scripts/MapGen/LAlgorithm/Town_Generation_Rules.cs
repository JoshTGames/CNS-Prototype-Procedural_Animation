using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Town_Generator{
    [CreateAssetMenu(fileName = "New Rule", menuName = "ScriptableObjects/Procedural Town/Create Rule")]
    public class Town_Generation_Rules : ScriptableObject{
        public string letter;
        [SerializeField] string[] results;
        [SerializeField] bool randomResult = false;
        public string GetResult(){
            if (randomResult) { 
                return results[UnityEngine.Random.Range(0, results.Length)];                
            }
            return results[0];
        }
    }

}
