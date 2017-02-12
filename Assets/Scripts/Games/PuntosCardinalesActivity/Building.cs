using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Games.PuntosCardinalesActivity {
	public class Building : IEquatable<Building> {
		string name, textNameStart,textNameEnd;
		bool isDouble;
		bool isStreet;
		bool isLeft;

		private Building(string name, string textNameStart,string textNameEnd, bool isDouble = false, bool isStreet = false, bool isLeft = false) {
			this.isLeft = isLeft;
			this.isStreet = isStreet;
			this.isDouble = isDouble;
			this.name = name;
			this.textNameStart = textNameStart;
			this.textNameEnd = textNameEnd;
		}

		public static Building B(string name,string textNameStart,string textNameEnd, bool isDouble = false, bool isStreet = false, bool isLeft = false){
			return new Building(name,textNameStart, textNameEnd, isDouble, isStreet, isLeft);
		}

		public string GetName(){
			return name;
		}

		public string GetTextNameStart(){
			return textNameStart;
		}

		public string GetTextNameEnd(){
			return textNameEnd;
		}

		public bool IsDouble(){
			return isDouble;
		}

		public bool IsStreet(){
			return isStreet;
		}

		public bool IsLeft(){
			return isLeft;
		}

		public Sprite GetSprite(Dictionary<string, Sprite> spriteDictionary) {
			return spriteDictionary[GetName() + (isDouble ? (isLeft ? "Left" : "Right") : "")];
		}

		#region IEquatable implementation

		public bool Equals(Building other) {
			if(other == null) return false;
			return name == other.name && isDouble == other.isDouble && isStreet == other.isStreet;
		}

		#endregion
	}
}