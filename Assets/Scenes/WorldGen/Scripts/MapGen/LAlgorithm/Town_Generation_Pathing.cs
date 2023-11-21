using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Town_Generator{
    public class Town_Generation_Pathing{
        #region GENERATE STRING PATH
        Town_Generation_Rules[] rules;
        int maxIterations;
        float ignoreRuleChance;

        /// <summary>Returns a string -- holding instructions to build a visual path</summary> 
        public string GetPath(string word, Town_Generation_Rules[] _rules = null, int _maxIterations = 1, float _ignoreRuleChance = 0f){
            if(word == null) { return null; }
            rules = _rules;
            maxIterations = _maxIterations;
            ignoreRuleChance = _ignoreRuleChance;
            return GeneratePath(word);
        }
        private string GeneratePath(string word = null, int curIteration = 0){
            #region SAFETY MEASURES
            if (word == null || word == "") { return null; }
            if (curIteration >= maxIterations) { return word; }            
            #endregion

            StringBuilder newWord = new StringBuilder();
            foreach (char c in word){
                newWord.Append(c);
                ProcessWordsRecursively(newWord, c, curIteration);
            }
            return newWord.ToString();
        }
        private void ProcessWordsRecursively(StringBuilder newWord, char c, int curIteration){
            foreach (Town_Generation_Rules rule in rules){
                if (rule.letter == c.ToString()){

                    if (UnityEngine.Random.value < ignoreRuleChance && curIteration > 1) { return; }
                    newWord.Append(GeneratePath(rule.GetResult(), curIteration + 1));
                }
            }
        }
        #endregion


        #region DEBUG VISUALS
        List<Vector3> positionsTravelled = new List<Vector3>();

        int length;
        public int Length{
            get{
                if (length > 0) { return length; }
                else { return 1; }
            }
            set => length = value;
        }



        
        public void ApplyPath(string sequence, Grid_Generator grid, int _length = 8, int _angle = 90){
            length = _length;
            Stack<Town_Generation_Agent> points = new Stack<Town_Generation_Agent>();
            #region STARTING POS
            Vector2Int gridDimensions = Vector2Int.FloorToInt(grid.GetDimensions() / 2);


            Vector2 nodePos = grid.GetWorldPos(gridDimensions.x, gridDimensions.y);
            Vector3 curPos = new Vector3(nodePos.x, 0, nodePos.y) + new Vector3(grid.GetCellSize() / 2, 0, grid.GetCellSize() / 2);
            Debug.Log(gridDimensions);
            #endregion

            Vector3 dir = Vector3.forward;
            Vector3 tempPos = Vector3.zero;

            positionsTravelled.Add(curPos);
            foreach (char c in sequence){
                PathingInstructions encoding = (PathingInstructions)c;
                switch (encoding){
                    case PathingInstructions.SAVE:
                        points.Push(new Town_Generation_Agent{
                            position = curPos,
                            direction = dir,
                            length = Length
                        });
                        break;
                    case PathingInstructions.LOAD:
                        if (points.Count > 0){
                            Town_Generation_Agent agent = points.Pop();
                            curPos = agent.position;
                            dir = agent.direction;
                            Length = agent.length;
                        }
                        else { throw new System.Exception("Don't have any points to save..."); }
                        break;
                    case PathingInstructions.DRAW:
                        tempPos = curPos;
                        curPos += dir * (grid.GetCellSize() * Length);                        
                        Length -= 2; // MAY ERROR
                        positionsTravelled.Add(curPos);
                        break;
                    case PathingInstructions.TURN_RIGHT:
                        dir = Quaternion.AngleAxis(_angle, Vector3.up) * dir;
                        break;
                    case PathingInstructions.TURN_LEFT:
                        dir = Quaternion.AngleAxis(-_angle, Vector3.up) * dir;
                        break;
                    default:
                        break;
                }
            }
        }
        

        /// <summary>
        /// THIS FUNCTION IS FOR DEBUGGING -- SHOWS LINES WHERE THE PATH WOULD GO ONTO THE GRID
        /// </summary>
        /// <param name="sequence">Rule set the visualiser will use to path the visuals</param>
        /// <param name="lineMaterial">Colour properties of the debugging</param>
        /// <param name="length"></param>
        /// <param name="angle">Rotation the agent will rotate by</param>
        public void VisualisePath(string sequence, Grid_Generator grid, Material _lineMaterial, int _length = 8, int _angle = 90){
            length = _length;            
            Stack<Town_Generation_Agent> points = new Stack<Town_Generation_Agent>();
            #region STARTING POS
            Vector2Int gridDimensions = Vector2Int.FloorToInt(grid.GetDimensions()/2);

            
            Vector2 nodePos = grid.GetWorldPos(gridDimensions.x, gridDimensions.y);
            Vector3 curPos = new Vector3(nodePos.x, 0, nodePos.y) + new Vector3(grid.GetCellSize() /2,0, grid.GetCellSize() /2);
            Debug.Log(gridDimensions);
            #endregion

            Vector3 dir = Vector3.forward;
            Vector3 tempPos = Vector3.zero;

            positionsTravelled.Add(curPos);
            foreach (char c in sequence){
                PathingInstructions encoding = (PathingInstructions)c;
                switch (encoding){
                    case PathingInstructions.SAVE:
                        points.Push(new Town_Generation_Agent{
                            position = curPos,
                            direction = dir,
                            length = Length
                        });
                        break;
                    case PathingInstructions.LOAD:
                        if (points.Count > 0){
                            Town_Generation_Agent agent = points.Pop();
                            curPos = agent.position;
                            dir = agent.direction;
                            Length = agent.length;
                        }
                        else { throw new System.Exception("Don't have any points to save..."); }
                        break;
                    case PathingInstructions.DRAW:
                        tempPos = curPos;
                        curPos += dir * (grid.GetCellSize() * Length);
                        DrawLine(tempPos, curPos, Color.red, _lineMaterial);
                        Length -= 2; // MAY ERROR
                        positionsTravelled.Add(curPos);
                        break;
                    case PathingInstructions.TURN_RIGHT:
                        dir = Quaternion.AngleAxis(_angle, Vector3.up) * dir;
                        break;
                    case PathingInstructions.TURN_LEFT:
                        dir = Quaternion.AngleAxis(-_angle, Vector3.up) * dir;
                        break;
                    default:
                        break;
                }
            }            
        }

        private void DrawLine(Vector3 start, Vector3 end, Color colour, Material lineMaterial){            
            GameObject line = new GameObject("Line");
            line.transform.position = start;
            LineRenderer lR = line.AddComponent<LineRenderer>();
            lR.material = lineMaterial;
            lR.startColor = colour;
            lR.endColor = colour;
            lR.startWidth = 2f;
            lR.endWidth = 2f;
            lR.SetPosition(0, start);
            lR.SetPosition(1, end);
            line.transform.SetParent(GameObject.Find("DebugTextFolder").transform);
        }
        #endregion
    }
}
