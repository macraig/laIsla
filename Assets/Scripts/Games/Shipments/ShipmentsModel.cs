using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Games.Shipments;
using Assets.Scripts.Metrics.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Games.Shipments
{
    public class ShipmentsModel : LevelModel
    {
        public const int START_TIME = 120, CORRECT_SCENE_TIME = 50;
        private bool withTime;

        private const int NODES = 13;
        private int _currentLevel;
        public int _remainExercises = 6;
        private List<ShipmentNode> _nodes; 
        private List<ShipmentEdge> _edges;
        private List<ShipmentsPath> _solutionPaths;
        private List<int> _edgesBySolutionPath;

        private bool lastCorrect;
        private int timer;


        public ShipmentsModel()
        {
            timer = START_TIME;

            _nodes = new List<ShipmentNode>();
            _edges = new List<ShipmentEdge>();
            _solutionPaths = new List<ShipmentsPath>();
            _currentLevel = 0;
            lastCorrect = true;
            MetricsController.GetController().GameStart();

        }

        public List<ShipmentNode> Nodes
        {
            get { return _nodes; }
        }

        public List<ShipmentEdge> Edges
        {
            get { return _edges; }
        }

        public int Scale { get; set; }

        public int RemainExercises
        {
            get { return _remainExercises; }
            set { _remainExercises = value; }
        }


        public void NextExercise()
        {
            int nodes;
            int solutionPaths;
            float extraEdgeProbability;
            int nodesRequiered;
            Scale = 10;
            _solutionPaths.Clear();
            Nodes.Clear();
            Edges.Clear();

            switch (_currentLevel)
            {
                case 0:
                    nodes = 2;
                    solutionPaths = 1;
                    _edgesBySolutionPath = new List<int>(solutionPaths) {1};
                    extraEdgeProbability = 0;
                    break;
                case 1:
                    nodes = Random.Range(3, 6);
                    solutionPaths = 1;
                    _edgesBySolutionPath = new List<int>(solutionPaths) { 2 };
                    extraEdgeProbability = 0;

                    break;
                case 2:
                    nodes = Random.Range(5, 8);
                    solutionPaths = 1;
                    _edgesBySolutionPath = new List<int>(solutionPaths)
                    { GetEdgesToSolutionPath(nodes) };
                    extraEdgeProbability = 0.25f;
              
                    break;
                case 3:
                    nodes = Random.Range(5, 8);
                    solutionPaths = 2;
                    _edgesBySolutionPath = new List<int>(solutionPaths)
                   { GetEdgesToSolutionPath(nodes)
                    , GetEdgesToSolutionPath(nodes) };
                    nodesRequiered = GetQuantityNodesRequiered(_edgesBySolutionPath[0] + _edgesBySolutionPath[1]);
                    if (nodes < nodesRequiered) nodes = nodesRequiered;
                    extraEdgeProbability = 0;

                    break;
                default:
                    nodes = Random.Range(6, 9);
                    solutionPaths = 2;
                    _edgesBySolutionPath = new List<int>(solutionPaths)
                    { GetEdgesToSolutionPath(nodes)
                    , GetEdgesToSolutionPath(nodes) };
                    nodesRequiered = GetQuantityNodesRequiered(_edgesBySolutionPath[0] + _edgesBySolutionPath[1]);
                    if(nodes < nodesRequiered) nodes = nodesRequiered;
                    
                    extraEdgeProbability = 1f;
                    break;           
            }
            // Necesito que haya una cantidad mínima de nodos, para poder cumplir las restricciones
            if (!CheckMinNodes(nodes))
            {
                NextExercise();
                return;
            }
           
            GenerateNodes(nodes);

            GenerateSolutionPaths(solutionPaths, _edgesBySolutionPath);


            while (SolutionsPathsAreEquals())
            {
                _solutionPaths.Clear();
                _edges.Clear();
                GenerateSolutionPaths(solutionPaths, _edgesBySolutionPath);
            }

            if (!IsValidGraph())
            {
                NextExercise();
                return;
            }
            RemainExercises--;
        }

        private static int GetEdgesToSolutionPath(int nodes)
        {
            int edgesToSolutionPath = Mathf.RoundToInt((nodes - 1) * Random.Range(0.5f, 1));
            return edgesToSolutionPath > 5 ? 5 : edgesToSolutionPath;
        }

        private bool CheckMinNodes(int nodes)
        {
            int pathsSize = 0;
            foreach (var edges in _edgesBySolutionPath)
            {
                pathsSize += edges + 1;
            }

            return nodes >= GetQuantityNodesRequiered(pathsSize);
        }

        private int GetQuantityNodesRequiered(int pathsSize)
        {
            return pathsSize - 2*_edgesBySolutionPath.Count;
        }

        private bool IsValidGraph()
        {
            return CheckEdgesQuantity() && CheckMaxEdges();
        }

        private bool CheckEdgesQuantity()
        {
            return Edges.Count < Nodes.Count * 2 - 2 ;
        }

        private bool CheckMaxEdges()
        {
            foreach (ShipmentNode node in Nodes)
            {
                if (GetEdgesByIdNode(node.Id).Count > 3) return false;
            }
            return true;
        }

        private bool SolutionsPathsAreEquals()
        {
            for (int i = _solutionPaths.Count - 1; i >= 0; i--)
            {
                for (int j = _solutionPaths.Count - 1; j >= 0; j--)
                {
                    if(i == j) continue;;
                    if (_solutionPaths[i].Equals(_solutionPaths[j])) return true;

                }
            }

            return false;
        }

        private void GenerateExtraEdges(float extraEdgeProbability)
        {
            if(Math.Abs(extraEdgeProbability) < 0.000001) return;
            foreach (ShipmentNode node in Nodes)
            {
                foreach (ShipmentNode otherNode in Nodes)
                {
                    if(node.Id == otherNode.Id) continue;
                    if(GetEdge(node.Id, otherNode.Id) != null) continue;
                    bool b = GetEdgesByIdNode(node.Id).Count < 2;
                    bool b1 = GetEdgesByIdNode(otherNode.Id).Count < 2;
                    if (Random.Range(0, 1) < extraEdgeProbability && b && b1/* && Edges.Count < (Nodes.Count/2)  * 3 - 4 */ )
                    {
                        ShipmentEdge shipmentEdge = new ShipmentEdge
                        {
                            IdNodeA = node.Id,
                            IdNodeB = otherNode.Id,
                        };
                        Edges.Add(shipmentEdge);
                    }
                }
            }
        }

        private ShipmentEdge GetEdge(int idA, int idB)
        {
            return Edges.Find(e => (e.IdNodeA == idA && e.IdNodeB == idB) || (e.IdNodeA == idB && e.IdNodeB == idA));
        }

        private void GenerateSolutionPaths(int solutionPaths, List<int> edgesBySolutionPath)
        {
            // hago - 2 xq el ultimo se lo voy a agregar al final. El 0 lo agrego si o si. 
            Randomizer nodeRandomizer = Randomizer.New(Nodes.Count - 2, 1, false);
            for (int i = solutionPaths - 1; i >= 0; i--)
            {
                nodeRandomizer.Restart();
                int pathLength = edgesBySolutionPath[i];
                ShipmentsPath shipmentsPath = new ShipmentsPath
                {
                    // Agrego el primero
                    // la cantidad de nodos es la de aristas + 1
                    NodesList = new List<ShipmentNode>(pathLength + 1) {Nodes[0]}
                };
                // es -3 xq el 0 y el ultimo ya estan fijos
                for (int j = 1; j <= shipmentsPath.NodesList.Capacity - 2; j++)
                {
                    ShipmentNode shipmentNode = Nodes[nodeRandomizer.Next()];

                    while (shipmentsPath.NodesList.Exists(e => e.Id == shipmentNode.Id) || GetEdgesByIdNode(shipmentNode.Id).Count > 2)
                    {
                        shipmentNode = Nodes[nodeRandomizer.Next()];
                    }
                    shipmentsPath.NodesList.Add(shipmentNode);

                    if (!ExistsEdge(shipmentsPath.NodesList[j - 1].Id, shipmentsPath.NodesList[j].Id))
                    {
                        ShipmentEdge edge = new ShipmentEdge();
                        edge.IdNodeA = shipmentsPath.NodesList[j - 1].Id;
                        edge.IdNodeB = shipmentsPath.NodesList[j].Id;
                        Edges.Add(edge);
                    }
                    
                                      
                }
                // Agrego el ultimo
                shipmentsPath.NodesList.Add(Nodes[Nodes.Count - 1]);
                ShipmentEdge lastEdge = new ShipmentEdge();
                lastEdge.IdNodeA = shipmentsPath.NodesList[shipmentsPath.NodesList.Count - 2].Id;
                lastEdge.IdNodeB = shipmentsPath.NodesList[shipmentsPath.NodesList.Count - 1].Id;
                Edges.Add(lastEdge);

                _solutionPaths.Add(shipmentsPath);          
            }
        }

        private bool ExistsEdge(int idA, int idB)
        {
            return _edges.Exists(e => (e.IdNodeA == idA && e.IdNodeB == idB) || (e.IdNodeB == idA && e.IdNodeA == idB));
        }

        private void GenerateNodes(int totalNodes)
        {
            Nodes.Clear();
            GenerateStartNode();
            Randomizer nodeRandomizer = Randomizer.New(NODES - 1);
            nodeRandomizer.ExcludeNumbers(new List<int>() {0});
            for (int i = totalNodes - 3; i >= 0; i--)
            {
                ShipmentNode shipmentNode = new ShipmentNode
                {
                    Id = nodeRandomizer.Next(),
                    Type = ShipmentNodeType.Other
                };
                Nodes.Add(shipmentNode);
            }
            GenerateLastNode(nodeRandomizer.Next());

        }

        private void GenerateLastNode(int id)
        {
            ShipmentNode shipmentNode = new ShipmentNode
            {
                Id = id,
                Type = ShipmentNodeType.Finish
            };
            Nodes.Add(shipmentNode);
        }

        private void GenerateStartNode()
        {
            ShipmentNode shipmentNode = new ShipmentNode
            {
                Id = 0,
                Type = ShipmentNodeType.Start
            };
            Nodes.Add(shipmentNode);

        }

        public bool IsCorrectAnswer(List<ShipmentEdge> edgeAnswers)
        {

            for (int i = 0; i < edgeAnswers.Count; i++)
            {
                edgeAnswers[i].Length = edgeAnswers[i].Length/Scale;
                ShipmentEdge edge = Edges.Find(e => e.Equals(edgeAnswers[i]));
                if (edge == null) return false;
            }
            bool isCorrectAnswer = edgeAnswers[edgeAnswers.Count - 1].IdNodeB == Nodes.Find(e => e.Type == ShipmentNodeType.Finish).Id;
            if (lastCorrect) _currentLevel++;
            lastCorrect = isCorrectAnswer;
            LogAnswer(isCorrectAnswer);
            return isCorrectAnswer;

        }

        public List<ShipmentEdge> GetEdgesByIdNode(int idNode)
        {
            return Edges.FindAll(e => e.IdNodeA == idNode || e.IdNodeB == idNode);
        }


        public bool GameEnd()
        {
            return 0 == RemainExercises;
        }

        public void DecreaseTimer()
        {
            if (timer > 0) timer--;
        }

        public bool IsTimerDone()
        {
            return timer == 0;
        }

        public void CorrectTimer()
        {
            timer += CORRECT_SCENE_TIME;
        }

        public int GetTimer()
        {
            return timer;
        }

        public void Resart()
        {
            timer = CORRECT_SCENE_TIME;
        }


/*1  function Dijkstra(Graph, source):
2      dist[source] ← 0                                    // Initialization
3
4      create vertex set Q
5
6      for each vertex v in Graph:           
7          if v ≠ source
8              dist[v] ← INFINITY                          // Unknown distance from source to v
9              prev[v] ← UNDEFINED                         // Predecessor of v
10
11         Q.add_with_priority(v, dist[v])
12
13
14     while Q is not empty:                              // The main loop
15         u ← Q.extract_min()                            // Remove and return best vertex
16         for each neighbor v of u:                       // only v that is still in Q
17             alt = dist[u] + length(u, v)
18             if alt<dist[v]
19                 dist[v] ← alt
20                 prev[v] ← u
21                 Q.decrease_priority(v, alt)
22
23     return dist[], prev[]*/
        public int GetCheaperSolutionPathCost(int idFrom, int idTo)
        {
            List<int> dist = new List<int>();
            dist.Add(0);

            Queue<ShipmentNode> vertexes = new Queue<ShipmentNode>();

            foreach (ShipmentNode node in Nodes)
            {
                if (node.Id != idFrom)
                {
                    
                }
            }

            ShipmentNode startNode = Nodes.Find(e => e.Id == idFrom);
            List<ShipmentEdge> edges = GetEdgesByIdNode(startNode.Id);
/*
            int min = edges.Ma;
*/
            return 0;
        }
    }

    public class ShipmentNode
    {
        public int Id { get; set; }

        public ShipmentNodeType Type { get; set; }

        public bool Equals(ShipmentNode otherNode)
        {
            return Id == otherNode.Id;
        }
    }

    public enum ShipmentNodeType
    {
        Start, Finish, Other
    }

    public class ShipmentEdge
    {
        public int IdNodeA { get; set; }

        public int IdNodeB { get; set; }

        public int Length { get; set; }

        public bool Equals(ShipmentEdge otherEdge)
        {
            return Length == otherEdge.Length && (IdNodeA == otherEdge.IdNodeA && IdNodeB == otherEdge.IdNodeB) || (IdNodeB == otherEdge.IdNodeA && IdNodeA == otherEdge.IdNodeB);

        }
    }
}

public class ShipmentsPath
{
    public List<ShipmentNode> NodesList { get; set; }

    public bool Equals(ShipmentsPath otherPath)
    {
        if (NodesList.Count != otherPath.NodesList.Count) return false;
        for (int i = NodesList.Count - 1; i >= 0; i--)
        {
            if (NodesList[i].Id != otherPath.NodesList[i].Id) return false;
        }
        return true;
    }

    /*
        public List<ShipmentEdge> EdgesList { get; set; }

        public int GetTotalCost()
        {
            int cost = 0;
            foreach (ShipmentEdge edge in EdgesList)
            {
                cost += edge.Length;
            }
            return cost;
        }
        */

}