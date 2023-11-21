using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Town_Generator{
    public class Grid_Generator{
        #region VARIABLES
        public class Node{
            public Vector2Int position = Vector2Int.zero;
            public bool isUsable = true;
            public Node parent;

            public Node(Vector2Int pos, bool isUsable = true, Node parent = null){
                this.position = pos;
                this.isUsable = isUsable;
                this.parent = parent;
            }
        }

        Vector2Int dimensions;
        float cellSize;


        public Node[] nodes;
        #endregion
        
        public Grid_Generator(Vector2Int dimensions, float cellSize, bool showDebug = false){
            #region VARIABLE ASSIGNMENT
            this.dimensions = dimensions;
            this.cellSize = cellSize;

            nodes = new Node[dimensions.x * dimensions.y];
            #endregion

            GameObject folder = null;
            if (showDebug){
                folder = new GameObject("DebugTextFolder");
                Debug.DrawLine(
                    new Vector3(GetWorldPos(dimensions.x, 0).x, 0, GetWorldPos(dimensions.x, 0).y),
                    new Vector3(GetWorldPos(dimensions.x, dimensions.y).x, 0, GetWorldPos(dimensions.x, dimensions.y).y),
                    Color.white, 100f
                ); // CLOSE Vertical

                Debug.DrawLine(
                    new Vector3(GetWorldPos(0, dimensions.y).x, 0, GetWorldPos(0, dimensions.y).y),
                    new Vector3(GetWorldPos(dimensions.x, dimensions.y).x, 0, GetWorldPos(dimensions.x, dimensions.y).y),
                    Color.white, 100f
                ); // CLOSE Horizontal
            }

            for (int x = 0; x < dimensions.x; x++){
                for (int y = 0; y < dimensions.y; y++){
                    nodes[GetNodeIndex(x, y, dimensions.x)] = new Node(new Vector2Int(x, y));

                    if (showDebug){
                        Vector3 curPos = new Vector3(GetWorldPos(x, y).x, 0, GetWorldPos(x, y).y);
                        CreateWorldText(new Vector2(x, y).ToString(), Color.white, folder.transform,
                            curPos + new Vector3(cellSize, 0, cellSize)
                            * .5f, 20
                        );
                        Debug.DrawLine(
                            curPos,
                            new Vector3(GetWorldPos(x, y + 1).x, 0, GetWorldPos(x, y + 1).y),
                            Color.white, 100f
                        ); // Vertical
                        Debug.DrawLine(
                            curPos,
                            new Vector3(GetWorldPos(x + 1, y).x, 0, GetWorldPos(x + 1, y).y),
                            Color.white, 100f
                        ); // Horizontal
                    }
                }
            }
        }


        #region FUNCTIONS    
        /// <summary> RETURNS WORLD POSITION FROM A GRID AFTER NODE HAS BEEN CREATED -- THE PASSED VALUE NEEDS TO BE THE EXACT NODE POSITION </summary>
        public Vector2 GetWorldPos(int x, int y) { return new Vector2(x, y) * cellSize; }
        /// <summary> THIS RETURNS THE EXACT NODE POSITION CLOSEST TO THE GIVEN VECTOR </summary>
        public Vector2 GetNodeWorldPosition(Vector2 rawPosition){
            Vector2 min = nodes[0].position, max = nodes[nodes.Length - 1].position;
            rawPosition = new Vector2(Mathf.Clamp(rawPosition.x / 10, min.x, max.x), Mathf.Clamp(rawPosition.y / 10, min.y, max.y));
            return new Vector2(rawPosition.x / cellSize, rawPosition.y / cellSize);
        }
        /// <summary> RETURNS THE INDEX A GIVEN NODE IS LOCATED BASED ON THE POSITION PASSED </summary>
        public int GetNodeIndex(int x, int y, int width) { return x + width * y; } // TREATS THE 1D ARRAY AS A 2D GRID AND RETURNS THE INDEX AT WHICH MATCHES A GIVEN POSITION

        public Vector2Int GetDimensions() { return dimensions; }
        public float GetCellSize() { return cellSize; }
        #endregion
        #region DEBUGGING
        TextMesh CreateWorldText(string text, Color colour, Transform parent = null, Vector3 localPos = default(Vector3), int fontSize = 40, TextAnchor txtAnchor = TextAnchor.MiddleCenter, TextAlignment txtAlignment = TextAlignment.Center, int sortingOrder = 0){
            if (colour == null) { colour = Color.white; }
            return CreateWorldText(parent, text, localPos, fontSize, (Color)colour, txtAnchor, txtAlignment, sortingOrder);
        }
        TextMesh CreateWorldText(Transform parent, string text, Vector3 localPos, int fontSize, Color colour, TextAnchor txtAnchor, TextAlignment txtAlignment, int sortingOrder){
            GameObject go = new GameObject("WorldText", typeof(TextMesh));
            Transform newTransform = go.transform;
            newTransform.SetParent(parent, false);
            newTransform.localPosition = localPos;
            TextMesh tm = go.GetComponent<TextMesh>();
            tm.anchor = txtAnchor;
            tm.alignment = txtAlignment;
            tm.text = text;
            tm.fontSize = fontSize;
            tm.color = colour;
            tm.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return tm;
        }
        #endregion
    }
}
