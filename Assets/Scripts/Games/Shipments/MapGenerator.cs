using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Games.Shipments;
using UnityEngine;
using Vectrosity;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour {

    public List<MapPlace> Places;
    public GameObject Map;
    public Ruler Ruler;

    public Texture dottedLineMaterial;
    public Material[] DottedLineMateriallMaterial;
    public int lastMaterial;

    public Sprite[] CrosSprites;
    public Sprite[] PlacesSprites;
    public Sprite FirstPlaceSprite;

    private List<VectorLine> lines;

    private int iterations = 0;

    // Use this for initialization
    void Start()
    {

/*
        LocatePlaces();
*/
  /*      foreach (MapPlace place in Places)
        {
            foreach (MapPlace otherPlace in Places)
            {
                Vector3 position = place.CrossReference.gameObject.transform.position;

                if (place.Equals(otherPlace)) continue;
                if (Random.Range(0f, 1f) < 0.1)
                {
                    Vector3 otherPosition = otherPlace.CrossReference.gameObject.transform.position;
                    VectorLine line = new VectorLine("Curve", new List<Vector2> { new Vector2(position.x, position.y), new Vector2(otherPosition.x, otherPosition.y) }, null, 8.0f, LineType.Continuous);

                    line.material = DottedLineMateriallMaterial;
                    line.textureScale = 1f;

                    line.Draw();
                    Debug.Log(line.GetLength());
                }
            }
        }*/
    }

/*
    public void LocatePlaces(List<ShipmentNode> nodes, List<ShipmentEdge> edges)
    {
        foreach (MapPlace mapPlace in Places.GetRange(1, Places.Count - 1))
        {
            mapPlace.Id = -1;
        }


        float scaleFactor = FindObjectOfType<Canvas>().scaleFactor;
        float edge = Places[0].GetComponent<RectTransform>().sizeDelta.x;
        float distanceMin = Mathf.Sqrt(2)*edge;
        Vector2 mapSize = Map.GetComponent<RectTransform>().sizeDelta;
        float xMax = mapSize.x - edge / 2;
        float yMax = mapSize.y * scaleFactor - edge;

        List<MapPlace> locatedPlaces = new List<MapPlace>(Places.Count);
        SetFirstPlace(nodes[0], xMax, yMax);
        locatedPlaces.Add(Places[0]);
        int i = 1;
        for (; i < nodes.Count; i++)
        {
            /*
                        if(nodes[i].Id == 0) continue;
            #1#
            Places[i].SetData(nodes[i].Id, PlacesSprites[nodes[i].Id], CrosSprites[nodes[i].Type == ShipmentNodeType.Other ? 1 : 0], nodes[i].Type);
            List<int> edgeDistances = new List<int>();
         /*   List<MapPlace> listOfPlaces =
                locatedPlaces.FindAll(
                    e =>
                    {
                        ShipmentEdge shipmentEdge = ShipmentsView.instance.Model.GetEdgesByIdNode(Places[i].Id)
                            .Find(f => f.IdNodeA == e.Id || f.IdNodeB == e.Id);
                        if (shipmentEdge != null) edgeDistances.Add(shipmentEdge);
                        return shipmentEdge != null;
                    });#1#
            List<MapPlace> listOfPlaces = locatedPlaces.FindAll(
                e =>
                {
                    ShipmentEdge shipmentEdge = edges.Find(f => (f.IdNodeA == Places[i].Id && f.IdNodeB == e.Id) || (f.IdNodeB == Places[i].Id && f.IdNodeA == e.Id));
                    if(shipmentEdge != null) edgeDistances.Add(shipmentEdge.Length);
                    return shipmentEdge != null;
                });
            do
            {
                LocatePlace(Places[i], xMax, yMax);
               
            } while (!CheckEdgeDistances(Places[i], listOfPlaces, edgeDistances) || !CheckMinDistances(Places[i], locatedPlaces, distanceMin * 2));
            Places[i].gameObject.SetActive(true);
        }
        for (int j = Places.Count - 1; j >= i; j--)
        {
            Places[j].gameObject.SetActive(false);
        }
    }
*/


    private void SetFirstPlace(ShipmentNode node, float xMax, float yMax)
    {
        Places[0].SetData(node.Id, FirstPlaceSprite, CrosSprites[node.Type == ShipmentNodeType.Other ? 1 : 0], node.Type);
        Places[0].transform.localPosition = new Vector2(-xMax, yMax);
    }
    /*
        private bool CheckEdgeDistances(MapPlace mapPlace, List<MapPlace> locatedPlaces, List<int> edgeDistances)
        {
            for (var i = locatedPlaces.Count - 1; i >= 0; i--)
            {
                MapPlace locatedPlace = locatedPlaces[i];
    /*
                float distance = Vector2.Distance(locatedPlace.transform.localPosition, mapPlace.transform.localPosition);
    #1#
                float referenceDistance = Vector2.Distance(locatedPlace.CrossReference.transform.position, mapPlace.CrossReference.transform.position);
                float f = referenceDistance / Ruler.GetUnityDistances();
                if (Math.Abs(f - edgeDistances[i]) > 0.1)
                {
                    return false;
                }

            }

            return true;
        }
    */


    private bool CheckMinDistances(MapPlace mapPlace, List<MapPlace> locatedPlaces, float distanceMin)
    {


        iterations++;
        if (iterations > 100)
        {
            return false;
        }
          foreach (MapPlace locatedPlace in locatedPlaces)
         {
             if(locatedPlace.Id == mapPlace.Id) continue;;
             float distance = Vector2.Distance(locatedPlace.transform.localPosition, mapPlace.transform.localPosition);
             if (distance < distanceMin)
                 return false;          
         }

        return true;
    }



    public void LocatePlace(MapPlace mapPlace, float xMax, float yMax)
    {/*
        mapPlace.transform.localPosition = new Vector2((Randomizer.RandomBoolean() ? 1 : -1)*Random.Range(0, xMax),
            (Randomizer.RandomBoolean() ? 1 : -1)*Random.Range(0, yMax));*/

        mapPlace.transform.localPosition = new Vector2(Random.Range(0, xMax),
            (Randomizer.RandomBoolean() ? 1 : -1)*Random.Range(-yMax, 0));
    }


    public void TraceEdges(List<ShipmentEdge> edges)
    {
        if (lines == null)
        {
            lines = new List<VectorLine>();

        }
        foreach (VectorLine line in lines)
        {
            line.rectTransform.gameObject.SetActive(false);
        }


        foreach (ShipmentEdge shipmentEdge in edges)
        {
            MapPlace mapPlace = Places.Find(e => e.Id == shipmentEdge.IdNodeA);
            MapPlace place = Places.Find(e => e.Id == shipmentEdge.IdNodeB);
            Vector3 position = mapPlace.CrossReference.gameObject.transform.position;
            Vector3 vector3 = place.CrossReference.gameObject.transform.position;
            VectorLine line = new VectorLine("Curve", new List<Vector2> { position, vector3 }, null, 8.0f, LineType.Continuous);

            line.SetCanvas(FindObjectOfType<Canvas>());

            line.material = DottedLineMateriallMaterial[lastMaterial++];
            if (lastMaterial == DottedLineMateriallMaterial.Length) lastMaterial = 0;
            line.textureScale = 1f;
            line.textureScale = 1f;
            line.Draw();
            lines.Add(line);
            line.rectTransform.transform.SetParent(Ruler.transform.parent);

        }
    }

    public void SafeLocatePlaces(List<ShipmentNode> nodes, List<ShipmentEdge> edges)
    {
        iterations = 0;
        ResetIds();
        float edge = Places[0].GetComponent<RectTransform>().sizeDelta.x / 2;
        float distanceMin = GetMinDistance();
        Vector2 mapSize = Map.GetComponent<RectTransform>().sizeDelta / 2;
        float xMax = (mapSize.x  - edge) * 0.95f;
        float yMax = (mapSize.y - edge) *0.95f;
        List<MapPlace> locatedPlaces = new List<MapPlace>(Places.Count);
        Utils.Shuffle(Places);
        nodes.Sort((node1, node2) => node1.Type == ShipmentNodeType.Start ? -1 : node2.Type == ShipmentNodeType.Start ? 1 : ShipmentsView.instance.Model.GetEdgesByIdNode(node2.Id).Count - ShipmentsView.instance.Model.GetEdgesByIdNode(node1.Id).Count);
        SetFirstPlace(nodes[0], xMax, yMax);
        locatedPlaces.Add(Places[0]);
        Places[0].gameObject.SetActive(true);
        int i = 1;
        for (; i < nodes.Count; i++)
        {
            MapPlace mapPlace = Places[i];
            mapPlace.gameObject.SetActive(true);
            ShipmentNode node = nodes[i];
            mapPlace.SetData(node.Id, PlacesSprites[node.Id], CrosSprites[node.Type == ShipmentNodeType.Other ? 1 : 0], node.Type);
            if (iterations >= 100)
            {
                iterations = 0;

                locatedPlaces.Clear();
                foreach (ShipmentEdge a in edges)
                {
                    a.Length = 0;
                }
                SafeLocatePlaces(nodes, edges);
                return;
            }
            SafeLocatePlace(mapPlace, locatedPlaces, ShipmentsView.instance.Model.GetEdgesByIdNode(node.Id), xMax, yMax, distanceMin);
        }

        // Oculto los places no usados
        for (int j = Places.Count - 1; j >= i; j--)
        {
            Places[j].gameObject.SetActive(false);
        }

        foreach (ShipmentEdge shipmentEdge in edges)
        {
            if (shipmentEdge.Length == 0)
            {
                MapPlace place = Places.Find(e => e.Id == shipmentEdge.IdNodeA);
                MapPlace mapPlace = Places.Find(e => e.Id == shipmentEdge.IdNodeB);
                float distance = Vector2.Distance(place.transform.position, mapPlace.transform.position)/
                                 Ruler.GetUnityDistances();
                shipmentEdge.Length = (int) distance;
            }
        }
    }

    private float GetMinDistance()
    {
        return Mathf.Sqrt(2) * (Places[0].GetComponent<RectTransform>().sizeDelta.x / 2);
    }


    private void ResetIds()
    {
        foreach (MapPlace mapPlace in Places.GetRange(1, Places.Count - 1))
        {
            mapPlace.Id = -1;
        }
    }


    private void SafeLocatePlace(MapPlace toLocate, List<MapPlace> locatedPlaces, List<ShipmentEdge> toLocateEdges, float xMax, float yMax, float distanceMin)
    {
        List<MapPlace> prevPlaces = ObtainLocatedNodesPrev(locatedPlaces, toLocateEdges);
/*
        if(prevPlaces.Count > 2) throw new Exception("Mmmhhh tiene muchas restricciones previas. Revisar la generación del grafo.");
*/
        switch (prevPlaces.Count)
        {
            case 2:
                LocatePlaceWithTwoRestricitons(toLocate, prevPlaces, locatedPlaces, xMax, yMax, distanceMin);
                break;
            case 1:
                LocatePlaceWithOneRestriction(toLocate, prevPlaces, locatedPlaces, xMax, yMax, distanceMin);
                break;
            default:
                do
                {
                    toLocate.transform.localPosition = new Vector2((Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0, xMax), (Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0, yMax));
                } while (EdgeIsIncorrect(toLocate, locatedPlaces, xMax, yMax, distanceMin, 5));
                break;
        }
        locatedPlaces.Add(toLocate);


        /*

        Vector2 otherPosition = located.CrossReference.transform.position;

        float f;
        do
        {
            toLocate.transform.localPosition = new Vector2((Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0, xMax), (Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0, yMax));
            var referenceDistance = Vector2.Distance(toLocate.CrossReference.transform.position, otherPosition);
            f = referenceDistance / Ruler.GetUnityDistances();
        } while (f < 1.8 || Math.Abs(f % 1) > 0.1);

        ShipmentsView.instance.measuresList.Add((int)f);*/
    }

    private void LocatePlaceWithOneRestriction(MapPlace toLocate, List<MapPlace> prevPlaces, List<MapPlace> locatedPlaces, float xMax, float yMax, float distanceMin)
    {

        MapPlace located = prevPlaces[0];
        Vector2 otherPosition = located.transform.position;
        ShipmentEdge edge = ShipmentsView.instance.Model.GetEdgesByIdNode(toLocate.Id).Find(e =>
            e.IdNodeA == located.Id || e.IdNodeB == located.Id);
        float f;
        do
        {
            toLocate.transform.localPosition =
                new Vector2((Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0.1f * xMax, 0.9f * xMax),
                    (Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0.1f * yMax, 0.9f * yMax));
            float a = Ruler.GetUnityDistances();
            float distance = edge.Length == 0 ? Random.Range(3, 9) : edge.Length;

            float x;
            float y;
            if (Randomizer.RandomBoolean())
            {
                // x random
                x = toLocate.transform.position.x;
                float f1 = distance * a;
                float f2 = x - otherPosition.x;
                while (Mathf.Abs(f1) < Mathf.Abs(f2))
                {
                    if (edge.Length != 0)
                    {
/*
                        throw new Exception("Edge have to b resized but the node is located");
*/
                    }
                    if (distance < 10)
                    {
                        distance++;
                    }
                    else
                    {
                        toLocate.transform.localPosition =
                            new Vector2((Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0, x), 0);

                        x = toLocate.transform.position.x;
                        f2 = x - otherPosition.x;
                    }
                    f1 = distance * a;
                }
                float sqrt = Mathf.Sqrt(Mathf.Pow(f1, 2) - Mathf.Pow(f2, 2));
                y = -sqrt + otherPosition.y;
            }
            else
            {
                // y random
                y = toLocate.transform.position.y;

                float f1 = distance * a;
                float f2 = y - otherPosition.y;
                while (Mathf.Abs(f1) < Mathf.Abs(f2))
                {
                    if (edge.Length != 0)
                    {
/*
                        throw new Exception("Edge have to b resized but the node is located");
*/
                    }

                    if (distance < 10)
                    {
                        distance++;
                    }
                    else
                    {
                        toLocate.transform.localPosition = new Vector2(0,
                            (Randomizer.RandomBoolean() ? 1 : -1) * Random.Range(0, yMax));

                        y = toLocate.transform.position.y;
                        f2 = y - otherPosition.y;
                    }
                    f1 = distance * a;
                }
                float sqrt = Mathf.Sqrt(Mathf.Pow(f1, 2) - Mathf.Pow(f2, 2));
                x = -sqrt + otherPosition.x;
            }


            toLocate.transform.position = new Vector2(x, y);
            var referenceDistance = Vector2.Distance(toLocate.transform.position, otherPosition);

            f = referenceDistance / a;
        } while (EdgeIsIncorrect(toLocate, locatedPlaces, xMax, yMax, distanceMin, f));

        edge.Length = (int)f;

    }

    private bool EdgeIsIncorrect(MapPlace toLocate, List<MapPlace> locatedPlaces, float xMax, float yMax, float distanceMin, float f)
    {
       
        iterations++;
        if (iterations > 100)
        {
            return false;

        }
        return f < 1.9 || Math.Abs(f % 1) > 0.08 || f > 9.1 || Math.Abs(toLocate.transform.localPosition.y) >= yMax || Math.Abs(toLocate.transform.localPosition.x) >= xMax || !CheckMinDistances(toLocate, locatedPlaces, distanceMin);


       

    }

    private void LocatePlaceWithTwoRestricitons(MapPlace toLocate, List<MapPlace> prevPlaces, List<MapPlace> locatedPlaces, float xMax, float yMax, float distanceMin)
    {
        float x;
        float y;
        MapPlace placed1 = prevPlaces[0];
        MapPlace placed2 = prevPlaces[1];

        int edgeLength1;
        int edgeLength2;
        do
        {
            edgeLength1 = Random.Range(3, 10);
            edgeLength2 = Random.Range(3, 10);
            float d1 = edgeLength1 * Ruler.GetUnityDistances();
            float d2 = edgeLength2 * Ruler.GetUnityDistances();
            float a = placed1.transform.position.x;
            float b = placed1.transform.position.y;
            float c = placed2.transform.position.x;
            float d = placed2.transform.position.y;
            

            // greg is a fun name, it is only an abbreviation of a calculus 
            var f1 = Mathf.Pow(d2, 2);
            float greg = Mathf.Pow(d1, 2) - f1 - Mathf.Pow(a, 2) - Mathf.Pow(d - b, 2) + Mathf.Pow(c, 2);

            float bascaraA = 1 + Mathf.Pow(a - c, 2) / (Mathf.Pow(d - b, 2));
            float bascaraB = (greg * (a - c) / (Mathf.Pow(d - b, 2))) - 2 * c;
            float bascaraC = Mathf.Pow(greg, 2) / (4 * Mathf.Pow(d - b, 2)) - f1 + Mathf.Pow(c, 2);
            if (Randomizer.RandomBoolean())
            {
                x = (-bascaraB + Mathf.Sqrt(Mathf.Pow(bascaraB, 2) - 4 * bascaraA * bascaraC)) / (2 * bascaraA);

            }
            else
            {
                x = (-bascaraB - Mathf.Sqrt(Mathf.Pow(bascaraB, 2) - 4 * bascaraA * bascaraC)) / (2 * bascaraA);
            }
            float f2 = Mathf.Pow((x - c), 2);
            y = Mathf.Sqrt(f1 - f2) + d;
            if(!float.IsNaN(x) && !float.IsNaN(y)) toLocate.transform.position = new Vector2(x, y);
 
        } while (float.IsNaN(x) || float.IsNaN(y) || EdgeIsIncorrect(toLocate, locatedPlaces, xMax, yMax, distanceMin, edgeLength1 > edgeLength2 ? edgeLength1 : edgeLength2 ));
        List<ShipmentEdge> edges = ShipmentsView.instance.Model.GetEdgesByIdNode(toLocate.Id);
        edges.Find(e => e.IdNodeA == placed1.Id || e.IdNodeB == placed1.Id).Length = edgeLength1;
        edges.Find(e => e.IdNodeA == placed2.Id || e.IdNodeB == placed2.Id).Length = edgeLength2;
    }

    private List<MapPlace> ObtainLocatedNodesPrev(List<MapPlace> locatedPlaces, List<ShipmentEdge> toLocateEdges)
    {
        return locatedPlaces.FindAll(e => toLocateEdges.Exists(f => f.IdNodeA == e.Id || f.IdNodeB == e.Id));
    }

    private MapTriangle FindTriangle(MapPlace toLocate, List<MapPlace> locatedPlaces, List<ShipmentEdge> allEdges)
    {
        MapTriangle mapTriangle = new MapTriangle();

        List<ShipmentEdge> edgesByIdNode = ShipmentsView.instance.Model.GetEdgesByIdNode(toLocate.Id);

        List<MapPlace> vertices = locatedPlaces.FindAll(e => edgesByIdNode.Exists(f => f.IdNodeA == e.Id || f.IdNodeB == e.Id));
        if (vertices.Count < 2) return null;
/*
        if (vertices.Count > 2) throw new Exception("mmmh many vertexes");
*/

        ShipmentEdge edge = allEdges.Find(
            e =>
                (e.IdNodeA == vertices[0].Id && e.IdNodeB == vertices[1].Id) ||
                (e.IdNodeB == vertices[0].Id && e.IdNodeA == vertices[1].Id));
        mapTriangle.Nodes = new List<int>(2);
        mapTriangle.Edges = new List<ShipmentEdge>(3);
        mapTriangle.Nodes.AddRange(new []{ toLocate.Id, vertices[0].Id, vertices[1].Id });
        mapTriangle.Edges.AddRange(new []
        {
            edge, edgesByIdNode.Find(e => e.IdNodeA == vertices[0].Id || e.IdNodeB == vertices[0].Id),
            edgesByIdNode.Find(e => e.IdNodeA == vertices[1].Id || e.IdNodeB == vertices[1].Id)
        });
        return mapTriangle;
    }

    public bool CheckAllDistances(List<ShipmentEdge> allEdges)
    {
        List<MapPlace> locatedPlaces = Places.FindAll(e => e.Id != -1);
        foreach (MapPlace place in locatedPlaces)
        {
            if (!CheckMinDistances(place, locatedPlaces, GetMinDistance()))
            {
                return false;
            }
        }

        foreach (ShipmentEdge edge in allEdges)
        {
            MapPlace a = locatedPlaces.Find(e => e.Id == edge.IdNodeA);
            MapPlace b = locatedPlaces.Find(e => e.Id == edge.IdNodeB);

            float f = Vector2.Distance(a.transform.position, b.transform.position) / Ruler.GetUnityDistances();
            if (Math.Abs(f % edge.Length) > 0.1) return false;
        }

        foreach (var mapPlace in locatedPlaces)
        {
            if(!IsInMapLimits(mapPlace)) return false;
        }
        return true;

    }

    private bool IsInMapLimits(MapPlace mapPlace)
    {
        float edge = Places[0].GetComponent<RectTransform>().sizeDelta.x / 2;
        Vector2 mapSize = Map.GetComponent<RectTransform>().sizeDelta / 2;
        float xMax = (mapSize.x - edge) * 0.95f;
        float yMax = (mapSize.y - edge) * 0.95f;
        Vector3 localPosition = mapPlace.transform.localPosition;
        return Mathf.Abs(localPosition.x) <= xMax && Mathf.Abs(localPosition.y) <= yMax;
    }
}

class MapTriangle
{
    public List<ShipmentEdge> Edges { get; set; }

    public List<int> Nodes { get; set; }
}
