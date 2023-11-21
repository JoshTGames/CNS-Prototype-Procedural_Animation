using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Town_Generator{
    public class Town_Generation : MonoBehaviour{
        #region GRID DATA
        [Serializable] public class Grid_Generation{
            [Tooltip("The size the grid will inherit -- PATHS WILL NOT EXCEED THESE DIMENSIONS!")] public Vector2Int dimensions = new Vector2Int(10, 10);
            [Tooltip("The spacing between each node")] public float cellSize = 10f;
            [Tooltip("If true, the grid will be visible")] public bool showDebug;

            [HideInInspector] public Grid_Generator gridData;
        }
        [SerializeField] Grid_Generation grid;
        public Grid_Generator GetGrid(){ return grid.gridData; }
        #endregion
        #region PATH SETTINGS
        [Serializable] public class Path_Generator_Settings{
            [Tooltip("Used to map the rest of the procedural-ness after the Root Sentence")] public Town_Generation_Rules[] rules;
            [Tooltip("This will be used to generate the starting sequence")] public string rootSentence = "[F]--F";
            [Tooltip("The amount of recursions to make a town -- WILL BRANCH OUT FROM CENTER OF GRID!")] public int iterations = 2;
            [Tooltip("The initial length of the main path")] public int startLength = 8;
            [Tooltip("Chance that the rule will be ignored")] [Range(0, 1)] public float chanceToIgnoreRule = .3f;

            [HideInInspector] public string generatedPath;
        }
        [SerializeField] Path_Generator_Settings pathSettings;
        Town_Generation_Pathing pathGeneration = new Town_Generation_Pathing();
        #endregion

        [SerializeField] GameObject baseplate;
        [SerializeField] Material tempMat;
        private void Awake(){
            grid.gridData = new Grid_Generator(grid.dimensions, grid.cellSize, grid.showDebug);
            pathSettings.generatedPath = pathGeneration.GetPath(pathSettings.rootSentence, pathSettings.rules, pathSettings.iterations, pathSettings.chanceToIgnoreRule);
            
            Transform plate = (baseplate)? GeneratePlate(baseplate) : null; // GENERATES THE FLOORING
            pathGeneration.VisualisePath(pathSettings.generatedPath, grid.gridData, tempMat, pathSettings.startLength, 90);
        }

        /// <summary>RETURNS A VECTOR3 WHICH CAN BE USED IN SCALING/POSITIONING OBJECTS</summary>
        Vector3 GetDimensions(){
            Vector2Int dimensions = grid.gridData.GetDimensions();
            return new Vector3(dimensions.x * (grid.cellSize * .5f), 5, dimensions.y * (grid.cellSize * .5f));
        }
        Transform GeneratePlate(GameObject obj){
            Vector2Int dimensions = grid.gridData.GetDimensions();
            GameObject clonedObj = Instantiate(obj, GetDimensions(), Quaternion.identity);
            Transform objTransform = clonedObj.transform;
            objTransform.localScale = GetDimensions();
            objTransform.position = new Vector3(objTransform.position.x, 0 - objTransform.localScale.y, objTransform.position.z);

            clonedObj.AddComponent<BoxCollider>();
            return objTransform;
        }
    }
}
