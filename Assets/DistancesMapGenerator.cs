using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class DistancesMapGenerator : MonoBehaviour
{

    public List<GameObject> Places; 
    public Texture dottedLineMaterial; 
    public Material DottedLineMateriallMaterial; 

	// Use this for initialization
	void Start () {
	    foreach (GameObject place in Places)
	    {	        
            foreach (GameObject otherPlace in Places)
	        {
                Vector3 position = place.transform.position;

                if (place.Equals(otherPlace)) continue;
	            if (Random.Range(0f, 1f) < 0.1)
	            {
                    Vector3 otherPosition = otherPlace.transform.position;	           
                    VectorLine line = new VectorLine("Curve", new List<Vector2> { new Vector2(position.x, position.y), new Vector2(otherPosition.x, otherPosition.y) }, null, 8.0f, LineType.Continuous);

	                line.material = DottedLineMateriallMaterial;
                    line.textureScale = 1f;
                    
                    line.Draw();
                    Debug.Log(line.GetLength());
                }
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
