using System;
using Assets.Scripts.Common;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Games.RompecabezasActivity {
	public class PartModel : IEquatable<PartModel> {
		public Direction direction, previousDir, middleFork;
		public int col, row;
		public bool isDouble, isCross, isLeftUp, isFork;

		public PartModel(Direction d, Direction previousDir, int column, int row, bool isDouble = false, bool isCross = false, bool isFork = false, Direction middleFork = Direction.NULL) {
			direction = d;
			this.previousDir = previousDir;
			col = column;
			this.row = row;
			this.isDouble = isDouble;
			this.isCross = isCross;
			isLeftUp = Randomizer.RandomBoolean();
			this.isFork = isFork;
			this.middleFork = middleFork;
		}

		public Sprite GetSprite(List<Sprite> parts) {
			//FORK
			Debug.Log("fork: " + isFork + " middle: " + middleFork);
			if(isFork && middleFork == Direction.LEFT) return parts[8];
			if(isFork && middleFork == Direction.RIGHT) return parts[9];
			if(isFork && middleFork == Direction.UP) return parts[10];
			if(isFork && middleFork == Direction.DOWN) return parts[11];

			//DOUBLE
			if(isDouble && isCross)
				return parts[16];
			if(isDouble && !isLeftUp)
				return parts[20];
			if(isDouble && isLeftUp)
				return parts[21];

			//REGULAR
			if((previousDir == Direction.LEFT || previousDir == Direction.RIGHT) && (direction == Direction.LEFT || direction == Direction.RIGHT))
				return parts[24];
			if((previousDir == Direction.UP || previousDir == Direction.DOWN) && (direction == Direction.UP || direction == Direction.DOWN))
				return parts[23];
			if((previousDir == Direction.RIGHT && direction == Direction.UP) || (previousDir == Direction.DOWN && direction == Direction.LEFT))
				return parts[15];
			if((previousDir == Direction.LEFT && direction == Direction.UP) || (previousDir == Direction.DOWN && direction == Direction.RIGHT))
				return parts[14];
			if((previousDir == Direction.LEFT && direction == Direction.DOWN) || (previousDir == Direction.UP && direction == Direction.RIGHT))
				return parts[13];
			if((previousDir == Direction.RIGHT && direction == Direction.DOWN) || (previousDir == Direction.UP && direction == Direction.LEFT))
				return parts[12];
			

			return null;
		}

		#region IEquatable implementation

		public bool Equals(PartModel other) {
			return col == other.col && row == other.row;
		}

		#endregion
	}
}