using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Games.Shipments;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Games.Shipments
{
    public class ShipmentsModel : LevelModel
    {

        private const int NODES = 8;
        private int _currentLevel;
        private List<ShipmentNode> _nodes; 
        private List<ShipmentEdge> _edges;
        private List<ShipmentsPath> _solutionPaths;


        public ShipmentsModel()
        {
            _nodes = new List<ShipmentNode>();
            _edges = new List<ShipmentEdge>();
            _solutionPaths = new List<ShipmentsPath>();
            _currentLevel = 1;
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


        public void NextExercise()
        {
            int nodes;
            int solutionPaths;
            int maxLongEdge;
            List<int> edgesBySolutionPath;
            float extraEdgeProbability;
            Scale = Random.Range(3, 11);
            _solutionPaths.Clear();
            Nodes.Clear();
            Edges.Clear();
            
            switch (_currentLevel)
            {
                case 0:
                    nodes = 2;
                    solutionPaths = 1;
                    edgesBySolutionPath = new List<int>(solutionPaths) {1};
                    extraEdgeProbability = 0;
                    maxLongEdge = 10;
                    _currentLevel++;
                    break;
                case 1:
                    nodes = Random.Range(2, 5);
                    solutionPaths = 1;
                    edgesBySolutionPath = new List<int>(solutionPaths) { nodes - 1 };
                    extraEdgeProbability = 0;
                    maxLongEdge = 5;

                    break;
                default:
                    nodes = Random.Range(4, 6);
                    solutionPaths = 2;
                    edgesBySolutionPath = new List<int>(solutionPaths) { 1, 2};
                    extraEdgeProbability = 0;
                    maxLongEdge = 4;


                    break;
            }
            GenerateNodes(nodes);
            GenerateSolutionPaths(solutionPaths, edgesBySolutionPath);
            GenerateEdgesToSolutionPaths(maxLongEdge);

            GenerateExtraEdges(extraEdgeProbability, maxLongEdge);
            Debug.Log("cost: " + _solutionPaths[0].GetTotalCost());

        }

        private void GenerateExtraEdges(float extraEdgeProbability, int maxLong)
        {
            if(Math.Abs(extraEdgeProbability) < 0.000001) return;
            foreach (ShipmentNode node in Nodes)
            {
                foreach (ShipmentNode otherNode in Nodes)
                {
                    if(node.Equals(otherNode)) continue;
                    if(GetEdge(node.Id, otherNode.Id) != null) continue;
                    if (Random.Range(0, 1) < extraEdgeProbability)
                    {
                        ShipmentEdge shipmentEdge = new ShipmentEdge
                        {
                            IdNodeA = node.Id,
                            IdNodeB = otherNode.Id,
                            Length = Random.Range(2, maxLong + 1)
                        };
                        Edges.Add(shipmentEdge);
                    }
                }
            }
        }

        private void GenerateEdgesToSolutionPaths(int maxLong)
        {
            foreach (ShipmentsPath shipmentsPath in _solutionPaths)
            {
                shipmentsPath.EdgesList = new List<ShipmentEdge>(shipmentsPath.NodesList.Count - 1);
                for (var i = 0; i < shipmentsPath.NodesList.Count - 1; i++)
                {

                    ShipmentEdge shipmentEdge = GetEdge(shipmentsPath.NodesList[i].Id, shipmentsPath.NodesList[i + 1].Id);
                    if (shipmentEdge == null)
                    {
                        shipmentEdge = new ShipmentEdge
                        {
                            IdNodeA = shipmentsPath.NodesList[i].Id,
                            IdNodeB = shipmentsPath.NodesList[i + 1].Id,
                            Length = Random.Range(2, maxLong + 1)
                        };
                        Edges.Add(shipmentEdge);
                    }


                    shipmentsPath.EdgesList.Add(shipmentEdge);
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
                for (int j = shipmentsPath.NodesList.Capacity - 3; j >= 0; j--)
                {
                    shipmentsPath.NodesList.Add(Nodes[nodeRandomizer.Next()]);
                }  
                // Agrego el ultimo
                shipmentsPath.NodesList.Add(Nodes[Nodes.Count - 1]);   
                _solutionPaths.Add(shipmentsPath);          
            }
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
            List<int> measuresList = ShipmentsView.instance.measuresList;
            foreach (ShipmentsPath shipmentsPath in _solutionPaths)
            {
                if(shipmentsPath.EdgesList.Count != edgeAnswers.Count) continue;
                int i = shipmentsPath.EdgesList.Count - 1;
                for (; i >= 0; i--)
                {
                    if(shipmentsPath.EdgesList[i].IdNodeA != edgeAnswers[i].IdNodeA) break;
                    if(shipmentsPath.EdgesList[i].IdNodeB != edgeAnswers[i].IdNodeB) break;
                    if(measuresList[i] != edgeAnswers[i].Length / Scale) break;
                }
                if (i == -1) return true;

            }
            return false;

        }

        public List<ShipmentEdge> GetEdgesByIdNode(int idNode)
        {
            return Edges.FindAll(e => e.IdNodeA == idNode || e.IdNodeB == idNode);
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


    }
}

public class ShipmentsPath
{
    public List<ShipmentNode> NodesList { get; set; }

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
}