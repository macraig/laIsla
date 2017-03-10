using System;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Games.Constelaciones {
	public class ConstelacionesLevel {
		private List<Vector2> stars, fakeStars;
		private List<string> instructions;
		private Sprite image;

		public ConstelacionesLevel(JSONClass source) {
			stars = new List<Vector2>();
			fakeStars = new List<Vector2>();
			instructions = new List<string>();

			foreach(JSONNode star in source["stars"].AsArray) {
				stars.Add(new Vector2(star.AsArray[0].AsFloat, star.AsArray[1].AsFloat));
			}
			foreach(JSONNode fakeStar in source["fakeStars"].AsArray) {
				fakeStars.Add(new Vector2(fakeStar.AsArray[0].AsFloat, fakeStar.AsArray[1].AsFloat));
			}
			foreach(JSONNode instruction in source["consignas"].AsArray) {
				instructions.Add(instruction.Value);
			}
			Debug.Log ("Sprites/Constelaciones/"+source["image"].Value);
			image = Resources.Load<Sprite> ("Sprites/Constelaciones/"+source["image"].Value);
			Resources.Load<Sprite> ("Sprites/Constelaciones/starBlueSelected");
		}

		public Sprite GetImage ()
		{
			return image;
		}

		public List<Vector2> GetStars(){
			return stars;
		}
		public List<Vector2> GetFakeStars(){
			return fakeStars;
		}
		public List<string> GetInstructions(){
			return instructions;
		}
	}
}