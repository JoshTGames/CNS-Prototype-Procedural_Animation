using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World_Generation : MonoBehaviour{
    /*
    #region SETTINGS
    [Serializable] private class GridSettings{
        public Vector2Int dimensions = new Vector2Int(10, 10);
        public float cellSize = 10f;
        public bool showDebug;
    }
    [SerializeField] GridSettings gridSettings;
    Grid_Generation grid;
    #endregion


    private void Awake(){
        grid = new Grid_Generation(gridSettings.dimensions, gridSettings.cellSize, gridSettings.showDebug); // GENERATE GRID
        // PROCEDURAL WORLD
        Transform plate = GeneratePlate();

        // CAMERA STUFF
        Vector2Int dimensions = grid.GetDimensions();
        Camera.main.transform.position = new Vector3(dimensions.x * (gridSettings.cellSize * .5f), 50, dimensions.y * (gridSettings.cellSize * .5f)) + new Vector3(0, 0, -100);
        Camera.main.transform.LookAt(new Vector3(dimensions.x * (gridSettings.cellSize * .5f), 5, dimensions.y * (gridSettings.cellSize * .5f)));
    }



    [SerializeField] GameObject baseplate;
    Transform GeneratePlate(){
        Vector2Int dimensions = grid.GetDimensions();
        GameObject clonedObj = Instantiate(baseplate, new Vector3(dimensions.x * (gridSettings.cellSize * .5f), 5, dimensions.y * (gridSettings.cellSize * .5f)), Quaternion.identity);
        Transform objTransform = clonedObj.transform;
        objTransform.localScale = new Vector3(dimensions.x * (gridSettings.cellSize * .5f), 5, dimensions.y * (gridSettings.cellSize * .5f));
        objTransform.position = new Vector3(objTransform.position.x, 0 - objTransform.localScale.y, objTransform.position.z);

        clonedObj.AddComponent<BoxCollider>();
        return objTransform;
    }


    // DEBUGGING
    private void Update(){
        if (Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 100;
            Vector2 pos = grid.GetNodeWorldPosition(Camera.main.ScreenToWorldPoint(mousePos)) * 10;            
            Vector2Int newPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));        
            Debug.Log(grid.GetNodeIndex(newPos.x, newPos.y, grid.GetDimensions().x));
        }
    }
    */
}
