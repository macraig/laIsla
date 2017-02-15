using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using Assets.Scripts.Sound;

namespace Assets.Scripts.Games.PuntosCardinalesActivity {
	public class PuntosCardinalesLevel {
		Building correct;
		int row, column;
		Possibilities p;
		List<AudioClip> audios;

		private PuntosCardinalesLevel(Building correct, int row, int column, Possibilities p, Dictionary<string, AudioClip> audios, List<List<Building>> grid){
			this.column = column;
			this.row = row;
			this.correct = correct;
			this.p = p;
			SetAudios(audios, grid);
		}

		public List<AudioClip> GetAudios() {
			return audios;
		}

		void SetAudios(Dictionary<string, AudioClip> a, List<List<Building>> grid) {
			audios = new List<AudioClip>();

			//primer audio -> "la laguna"
			audios.Add(a[correct.GetName()]);

			//switch con todas las posibilidades
			switch(p) {
			case Possibilities.BEHIND:
				audios.Add(a["detras"]);
				audios.Add(a[grid[row + 1][column].GetName() + "Final"]);

				break;
			case Possibilities.BEHIND_STREET:
				audios.Add(a["detras"]);
				audios.Add(a[grid[row + 2][column].GetName() + "Final"]);
				audios.Add(a["cruzandoLaCalle"]);

				break;
			case Possibilities.BETWEEN_VERTICAL:
				Building upperB = grid[row - 1][column];
				Building downB = grid[row + 1][column];

				audios.Add(a["entre"]);
				audios.Add(a[upperB.GetName()]);
				audios.Add(a["y"]);
				audios.Add(a[downB.GetName()]);

				break;
			case Possibilities.BETWEEN_HORIZONTAL:
				Building leftB = grid[row][column - 1];
				Building rightB = grid[row][column + 1];

				audios.Add(a["entre"]);
				audios.Add(a[leftB.GetName()]);
				audios.Add(a["y"]);
				audios.Add(a[rightB.GetName()]);

				break;
			case Possibilities.IN_FRONT:
				audios.Add(a["frente"]);
				audios.Add(a[grid[row - 1][column].GetName() + "Final"]);

				break;
			case Possibilities.IN_FRONT_POND_STREET:
				audios.Add(a["frenteALaLaguna"]);
				audios.Add(a["cruzandoLaCalle"]);

				break;
			case Possibilities.IN_FRONT_STREET:
				audios.Add(a["frente"]);
				audios.Add(a[grid[row - 2][column].GetName() + "Final"]);
				audios.Add(a["cruzandoLaCalle"]);

				break;
			case Possibilities.LEFT:
				audios.Add(a["izquierda"]);
				audios.Add(a[grid[row][column + 1].GetName() + "Final"]);
				break;
			case Possibilities.LEFT_POND_STREET:
				audios.Add(a["izquierdaDeLaLaguna"]);
				audios.Add(a["cruzandoLaCalle"]);
				break;
			case Possibilities.LEFT_STREET:
				audios.Add(a["izquierda"]);
				audios.Add(a[grid[row][column + 2].GetName() + "Final"]);
				audios.Add(a["cruzandoLaCalle"]);
				break;
			case Possibilities.RIGHT:
				audios.Add(a["derecha"]);
				audios.Add(a[grid[row][column - 1].GetName() + "Final"]);
				break;
			case Possibilities.RIGHT_POND_STREET:
				audios.Add(a["derechaDeLaLaguna"]);
				audios.Add(a["cruzandoLaCalle"]);
				break;
			case Possibilities.RIGHT_STREET:
				audios.Add(a["derecha"]);
				audios.Add(a[grid[row][column - 2].GetName() + "Final"]);
				audios.Add(a["cruzandoLaCalle"]);

				break;
			}
		}

		public List<Building> GetChoices(List<Building> simpleBuildings) {
			Randomizer r = Randomizer.New(simpleBuildings.Count - 1);
			List<Building> choices = new List<Building>();
			choices.Add(correct);
			while(choices.Count < 5){
				Building b = simpleBuildings[r.Next()];
				if(!choices.Contains(b)) choices.Add(b);
			}
			return Randomizer.RandomizeList(choices);
		}

		public bool IsCorrectDragger(Sprite dragger, Dictionary<string, Sprite> buildingSprites) {
			return correct.GetSprite(buildingSprites) == dragger;
		}

		public bool IsCorrectSlot(int row, int column) {
			return row == this.row && column == this.column;
		}

		// TEXT CREATION *******************************************************************************************************************

		public string GetText(List<List<Building>> grid){
			string result = correct.GetTextNameStart();

			switch(p) {
			case Possibilities.BEHIND:
				result = result + " está atrás " + grid[row + 1][column].GetTextNameEnd();
				break;
			case Possibilities.BEHIND_STREET:
				result = result + " está atrás " + grid[row + 2][column].GetTextNameEnd() + ", cruzando la calle";
				break;
			case Possibilities.BETWEEN_VERTICAL:
				Building upperB = grid[row - 1][column];
				Building downB = grid[row + 1][column];
				result = result + " está entre " + upperB.GetTextNameStart() + " y " + downB.GetTextNameStart();
				break;
			case Possibilities.BETWEEN_HORIZONTAL:
				Building leftB = grid[row][column - 1];
				Building rightB = grid[row][column + 1];
				result = result + " está entre " + leftB.GetTextNameStart() + " y " + rightB.GetTextNameStart();
				break;
			case Possibilities.IN_FRONT:
				result = result + " está en frente " + grid[row - 1][column].GetTextNameEnd();
				break;
			case Possibilities.IN_FRONT_POND_STREET:
				result = result + " está frente a la laguna, cruzando la calle";
				break;
			case Possibilities.IN_FRONT_STREET:
				result = result + " está en frente " + grid[row - 2][column].GetTextNameEnd() + ", cruzando la calle";
				break;
			case Possibilities.LEFT:
				result = result + " está a la izquierda " + grid[row][column + 1].GetTextNameEnd();
				break;
			case Possibilities.LEFT_POND_STREET:
				result = result + " está a la izquierda de la laguna, cruzando la calle";
				break;
			case Possibilities.LEFT_STREET:
				result = result + " está a la izquierda " + grid[row][column + 2].GetTextNameEnd() + ", cruzando la calle";
				break;
			case Possibilities.RIGHT:
				result = result + " está a la derecha " + grid[row][column - 1].GetTextNameEnd();
				break;
			case Possibilities.RIGHT_POND_STREET:
				result = result + " está a la derecha de la laguna, cruzando la calle";
				break;
			case Possibilities.RIGHT_STREET:
				result = result + " está a la derecha " + grid[row][column - 2].GetTextNameEnd() + ", cruzando la calle";
				break;
			}

			return result.ToUpper();
		}

		// LEVEL CREATION ******************************************************************************************************************

		public static PuntosCardinalesLevel CreateLevel(List<List<Building>> grid, Building correct, Dictionary<string, AudioClip> audios){
			Possibilities p = Possibilities.EMPTY;
			Array values = Enum.GetValues(typeof(Possibilities));
			List<Vector2> freeSpaces = FreeSpaces(grid);
			Randomizer r = Randomizer.New(freeSpaces.Count - 1);

			while(true){
				Vector2 randomFreeSpace = freeSpaces[r.Next()];
				int row = (int) randomFreeSpace.x, column = (int) randomFreeSpace.y;
				Randomizer enumRandomizer = Randomizer.New(values.Length - 1);
				for(int i = 0; i < values.Length; i++) {
					Possibilities value = (Possibilities) values.GetValue(enumRandomizer.Next());
					if(PossibilityApplies(row, column, grid, value)){
						p = value;
						break;
					}
				}

				if(p != Possibilities.EMPTY) {
					grid[row][column] = correct;
					return new PuntosCardinalesLevel(correct, row, column, p, audios, grid);
				}
			}
		}

		static bool PossibilityApplies(int row, int column, List<List<Building>> grid, Possibilities p) {
			Building spot, streetSpot;
			switch(p) {
			case Possibilities.BEHIND:
				if(row == 5)
					return false;
				spot = grid[row + 1][column];
				if(spot != null && !spot.IsStreet() && spot.GetName() != "laguna")
					return true;
				break;
			case Possibilities.BEHIND_STREET:
				if(row >= 4)
					return false;
				streetSpot = grid[row + 1][column];
				spot = grid[row + 2][column];
				if(spot != null && streetSpot != null && !spot.IsStreet() && streetSpot.IsStreet() && !spot.IsDouble())
					return true;
				break;
			case Possibilities.BETWEEN_VERTICAL:
				if(row >= 1 && row <= 4) {
					Building upperB = grid[row - 1][column];
					Building downB = grid[row + 1][column];
					if(upperB != null && !upperB.IsStreet() && downB != null && !downB.IsStreet())
						return true;
				}
				break;
			case Possibilities.BETWEEN_HORIZONTAL:
				if(column >= 1 && column <= 4){
					Building leftB = grid[row][column - 1];
					Building rightB = grid[row][column + 1];
					if(leftB != null && !leftB.IsStreet() && rightB != null && !rightB.IsStreet()) return true;
				}
				break;
			case Possibilities.IN_FRONT:
				if(row == 0)
					return false;
				spot = grid[row - 1][column];
				if(spot != null && !spot.IsStreet())
					return true;
				break;
			case Possibilities.IN_FRONT_POND_STREET:
				//si estoy en frente de la laguna y el del costado mio no esta vacio.
				if(row == 5 && (column == 2 || column == 3)) {
					return grid[5][column == 2 ? 3 : 2] != null;
				}
				break;
			case Possibilities.IN_FRONT_STREET:
				if(row <= 1)
					return false;
				streetSpot = grid[row - 1][column];
				spot = grid[row - 2][column];
				if(spot != null && streetSpot != null && !spot.IsStreet() && streetSpot.IsStreet() && spot.GetName() != "laguna" && !spot.IsDouble())
					return true;
				break;
			case Possibilities.LEFT:
				//this means i'm left of something
				if(column == 5)
					return false;
				spot = grid[row][column + 1];
				if(column < 5 && spot != null && !spot.IsStreet() && !spot.IsDouble())
					return true;
				break;
			case Possibilities.LEFT_POND_STREET:
				return row == 3 && column == 0;
			case Possibilities.LEFT_STREET:
				if(column >= 4)
					return false;
				streetSpot = grid[row][column + 1];
				spot = grid[row][column + 2];
				if(spot != null && streetSpot != null && !spot.IsStreet() && streetSpot.IsStreet() && spot.GetName() != "laguna")
					return true;
				break;
			case Possibilities.RIGHT:
				if(column == 0)
					return false;
				spot = grid[row][column - 1];
				if(spot != null && !spot.IsStreet() && spot.GetName() != "laguna")
					return true;
				break;
			case Possibilities.RIGHT_POND_STREET:
				return row == 3 && column == 5;
			case Possibilities.RIGHT_STREET:
				if(column <= 1)
					return false;
				streetSpot = grid[row][column - 1];
				spot = grid[row][column - 2];
				if(spot != null && streetSpot != null && !spot.IsStreet() && streetSpot.IsStreet() && spot.GetName() != "laguna")
					return true;
				break;
			}

			return false;
		}

		static List<Vector2> FreeSpaces(List<List<Building>> grid) {
			List<Vector2> result = new List<Vector2>();
			for(int row = 0; row < grid.Count; row++) {
				List<Building> r = grid[row];
				for(int column = 0; column < r.Count; column++) {
					if(r[column] == null)
						result.Add(new Vector2(row, column));
				}
			}

			return result;
		}
	}
}