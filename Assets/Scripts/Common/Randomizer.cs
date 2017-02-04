using System;
using System.Linq;
using System.Collections.Generic;
//using UnityEditor.VersionControl;

namespace Assets.Scripts.Common {
	public class Randomizer {
		
		private int maxValue;
		private int minValue;
		private List<int> values;
		private List<int> excluding = new List<int>();
		Boolean autoRestart;

		public static Random rnd = new Random();

		// Randomizer from min value to max value, both included
		private Randomizer(int maxValue, int minValue = 0, Boolean autoRestart = true) {
			this.autoRestart = autoRestart;
			this.minValue = minValue;
			this.maxValue = maxValue;
			Restart();
		}

		public static Randomizer New(int maxValue, int minValue = 0, Boolean autoRestart = true) {
			return new Randomizer(maxValue, minValue, autoRestart);
		}

		public void Restart() {
			values = Enumerable.Range(minValue, maxValue - minValue + 1).OrderBy(x => rnd.Next()).ToList();
		}

		public Randomizer ExcludeNumbers(List<int> excluding){
			this.excluding = excluding;
			return this;
		}

		public int Next() {
			if(!HasNext()) {
				if(autoRestart) Restart();
				else throw new Exception("Call HasNext(), Restart() or use autoRestart from constructor");
			}
			
			int first = values.First();
			values.RemoveAt(0);
			return excluding.Contains(first) ? Next() : first;
		}

		public bool HasNext() { return values.Count != 0; }

		public static int RandomInRange(int maxValue, int minValue = 0){
			return rnd.Next(minValue, maxValue);
		}

		public static int Random(){
			return rnd.Next();
		}

		public static List<T> RandomizeList<T>(List<T> l){
			return l.OrderBy(x => rnd.Next()).ToList();
		}

		public static bool RandomBoolean(){ return Randomizer.RandomInRange(100) < 50; }
	}
}