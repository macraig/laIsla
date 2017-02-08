using System;
using SimpleJSON;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Games.RompecabezasActivity;
using UnityEngine;

public class RompecabezasLevel {
	public bool hasTwoRoads, hasFork;
	public int partQuantity, distractionParts;
	public const int GRID = 6;
	private List<PartModel> parts;

	public RompecabezasLevel(JSONClass source, bool withTime) {
		hasTwoRoads = source["hasTwoRoads"].AsBool;
		hasFork = source["hasFork"].AsBool;
		partQuantity = source["partQuantity"].AsInt;
		distractionParts = source["distractionParts"].AsInt;

		Debug.Log("START");
		if(hasTwoRoads)
			BuildTwoRoadLevel();
		else if(hasFork)
			BuildForkLevel();
		else BuildLevel();
		Debug.Log("END");
	}

	void BuildForkLevel() {
		parts = new List<PartModel>();
		Direction[] dirs = new Direction[] {Direction.DOWN, Direction.LEFT, Direction.RIGHT, Direction.UP};
		Direction middleFork = RandomDirection(dirs);
		List<Direction> wheelDirections = new List<Direction>(dirs);
		wheelDirections.Remove(OppositeDir(middleFork));
		//Double part will be in any of the 4 middles tiles. This could be improved by moving the whole puzzle when ready.
		int doubleRow = Randomizer.New(3, 2).Next();
		int doubleCol = Randomizer.New(3, 2).Next();

		Debug.Log("Fork part: col " + doubleCol + " row " + doubleRow);

		PartModel forkPart = new PartModel(Direction.NULL, Direction.NULL, doubleCol, doubleRow, false, false, true, middleFork);
		parts.Add(forkPart);

		//The algorithm will consist in adding a part to each direction of the double part until i'm out 
		//of parts to add, the we will add de start and end parts.
		Direction wheelDir = RandomDirection(wheelDirections.ToArray());
		Debug.Log(wheelDir);

		//We need to keep track of which direction we are adding the parts so we can flatten them afterwards and add start and end parts.
		Dictionary<Direction, List<PartModel>> wheel = new Dictionary<Direction, List<PartModel>>();
		Direction dir = wheelDir;
		bool done = false;

		while(parts.Count < partQuantity){
			if(done){
				wheelDir = NextDir(wheelDirections.ToArray(), wheelDir);
				dir = wheelDir;
				Debug.Log(wheelDir);
				done = false;
			}

			if(!wheel.ContainsKey(wheelDir)) {
				doubleCol = DirectionPlusCol(dir, forkPart.col);
				doubleRow = DirectionPlusRow(dir, forkPart.row);

				//first part doesnt cross anything, ever.
				Randomizer dirRand = Randomizer.New(dirs.Length - 1);
				Direction oldDir = dir;
				for(int i = 0; i < dirs.Length; i++) {
					dir = dirs[dirRand.Next()];
					if(isApplicable(dir, doubleCol, doubleRow) && !IsSameWay(oldDir, dir)){
						PartModel newPart = new PartModel(dir, oldDir, doubleCol, doubleRow);
						if(!parts.Contains(newPart)) {
							parts.Add(newPart);
							wheel[wheelDir] = new List<PartModel>();
							wheel[wheelDir].Add(newPart);
							done = true;
							break;
						}
					}
				}
			} else {
				PartModel lastPart = wheel[wheelDir][wheel[wheelDir].Count - 1];
				dir = lastPart.direction;

				doubleCol = DirectionPlusCol(dir, lastPart.col);
				doubleRow = DirectionPlusRow(dir, lastPart.row);

				Randomizer dirRand = Randomizer.New(dirs.Length - 1);
				Direction oldDir = dir;
				bool doneInner = false;
				for(int i = 0; i < dirs.Length; i++) {
					dir = dirs[dirRand.Next()];
					if(isApplicable(dir, doubleCol, doubleRow) && !IsSameWay(oldDir, dir)){
						PartModel newPart = new PartModel(dir, oldDir, doubleCol, doubleRow);
						PartModel wouldBeEndPart = new PartModel(Direction.NULL, Direction.NULL, DirectionPlusCol(dir, doubleCol), DirectionPlusRow(dir, doubleRow));
						if(!parts.Contains(newPart) && !parts.Contains(wouldBeEndPart)) {
							parts.Add(newPart);
							wheel[wheelDir].Add(newPart);
							doneInner = true;
							break;
						}
					}
				}
				if(!doneInner) {
					//If I reach this segment it means that I have no way out. In that case, restart BuildLevel(). We should use a path algorithm.
					BuildForkLevel();return;
				}
			}
		}

		if(!AddDoubleEndStartParts(wheel, forkPart)) { BuildForkLevel();return; }

		AddDistractors();

		parts = Randomizer.RandomizeList(parts);
	}

	void BuildTwoRoadLevel() {
		parts = new List<PartModel>();
		Direction[] dirs = new Direction[] {Direction.DOWN, Direction.LEFT, Direction.RIGHT, Direction.UP};
		bool isCross = Randomizer.RandomBoolean();
		//Double part will be in any of the 4 middles tiles. This could be improved by moving the whole puzzle when ready.
		int doubleRow = Randomizer.New(3, 2).Next();
		int doubleCol = Randomizer.New(3, 2).Next();

		Debug.Log("Double part: col " + doubleCol + " row " + doubleRow);

		PartModel doublePart = new PartModel(Direction.NULL, Direction.NULL, doubleCol, doubleRow, true, isCross);
		parts.Add(doublePart);

		//The algorithm will consist in adding a part to each direction of the double part until i'm out 
		//of parts to add, the we will add de start and end parts.
		Direction wheelDir = RandomDirection(dirs);
		Debug.Log(wheelDir);

		//We need to keep track of which direction we are adding the parts so we can flatten them afterwards and add start and end parts.
		Dictionary<Direction, List<PartModel>> wheel = new Dictionary<Direction, List<PartModel>>();
		Direction dir = wheelDir;
		bool done = false;

		while(parts.Count < partQuantity){
			if(done){
				wheelDir = NextDir(dirs, wheelDir);
				dir = wheelDir;
				Debug.Log(wheelDir);
				done = false;
			}

			if(!wheel.ContainsKey(wheelDir)) {
				doubleCol = DirectionPlusCol(dir, doublePart.col);
				doubleRow = DirectionPlusRow(dir, doublePart.row);

				//first part doesnt cross anything, ever.
				Randomizer dirRand = Randomizer.New(dirs.Length - 1);
				Direction oldDir = dir;
				for(int i = 0; i < dirs.Length; i++) {
					dir = dirs[dirRand.Next()];
					if(isApplicable(dir, doubleCol, doubleRow) && !IsSameWay(oldDir, dir)){
						PartModel newPart = new PartModel(dir, oldDir, doubleCol, doubleRow);
						if(!parts.Contains(newPart)) {
							parts.Add(newPart);
							wheel[wheelDir] = new List<PartModel>();
							wheel[wheelDir].Add(newPart);
							done = true;
							break;
						}
					}
				}
			} else {
				PartModel lastPart = wheel[wheelDir][wheel[wheelDir].Count - 1];
				dir = lastPart.direction;

				doubleCol = DirectionPlusCol(dir, lastPart.col);
				doubleRow = DirectionPlusRow(dir, lastPart.row);

				Randomizer dirRand = Randomizer.New(dirs.Length - 1);
				Direction oldDir = dir;
				bool doneInner = false;
				for(int i = 0; i < dirs.Length; i++) {
					dir = dirs[dirRand.Next()];
					if(isApplicable(dir, doubleCol, doubleRow) && !IsSameWay(oldDir, dir)){
						PartModel newPart = new PartModel(dir, oldDir, doubleCol, doubleRow);
						PartModel wouldBeEndPart = new PartModel(Direction.NULL, Direction.NULL, DirectionPlusCol(dir, doubleCol), DirectionPlusRow(dir, doubleRow));
						if(!parts.Contains(newPart) && !parts.Contains(wouldBeEndPart)) {
							parts.Add(newPart);
							wheel[wheelDir].Add(newPart);
							doneInner = true;
							break;
						}
					}
				}
				if(!doneInner) {
					//If I reach this segment it means that I have no way out. In that case, restart BuildLevel(). We should use a path algorithm.
					BuildTwoRoadLevel();return;
				}
			}
		}

		if(!AddDoubleEndStartParts(wheel, doublePart)) { BuildTwoRoadLevel();return; }

		AddDistractors();

		parts = Randomizer.RandomizeList(parts);
	}

	bool AddDoubleEndStartParts(Dictionary<Direction, List<PartModel>> wheel, PartModel forkOrDoublePart) {
		bool firstCross = Randomizer.RandomBoolean();
		bool secondCross = Randomizer.RandomBoolean();

		if(forkOrDoublePart.isFork){
			List<Direction> keys = new List<Direction>(wheel.Keys);
			Randomizer wheelKeyRandomizer = Randomizer.New(keys.Count - 1);
			if(!AddStart(wheel, keys[wheelKeyRandomizer.Next()]) ||
				!AddEnd(wheel, keys[wheelKeyRandomizer.Next()]) ||
				!AddEnd(wheel, keys[wheelKeyRandomizer.Next()]))
				return false;
		} else if(forkOrDoublePart.isCross){
			if(!AddStart(wheel, firstCross ? Direction.LEFT : Direction.RIGHT) ||
				!AddEnd(wheel, firstCross ? Direction.RIGHT : Direction.LEFT) ||
				!AddStart(wheel, secondCross ? Direction.UP : Direction.DOWN) ||
				!AddEnd(wheel, secondCross ? Direction.DOWN : Direction.UP))
				return false;
		} else if(forkOrDoublePart.isLeftUp){
			if(!AddStart(wheel, firstCross ? Direction.LEFT : Direction.UP) ||
				!AddEnd(wheel, firstCross ? Direction.UP : Direction.LEFT) ||
				!AddStart(wheel, secondCross ? Direction.RIGHT : Direction.DOWN) ||
				!AddEnd(wheel, secondCross ? Direction.DOWN : Direction.RIGHT))
				return false;
		} else {
			if(!AddStart(wheel, firstCross ? Direction.LEFT : Direction.DOWN) ||
				!AddEnd(wheel, firstCross ? Direction.DOWN : Direction.LEFT) ||
				!AddStart(wheel, secondCross ? Direction.RIGHT : Direction.UP) ||
				!AddEnd(wheel, secondCross ? Direction.UP : Direction.RIGHT))
				return false;
		}

		return true;
	}

	bool AddStart(Dictionary<Direction, List<PartModel>> wheel, Direction direction) {
		PartModel lastPart = wheel[direction][wheel[direction].Count - 1];

		PartModel start = new PartModel(OppositeDir(lastPart.direction), Direction.NULL, DirectionPlusCol(lastPart.direction, lastPart.col), DirectionPlusRow(lastPart.direction, lastPart.row));

		if(parts.Contains(start)) return false;
		else {
			parts.Add(start);
			return true;
		}
	}

	bool AddEnd(Dictionary<Direction, List<PartModel>> wheel, Direction direction) {
		PartModel lastPart = wheel[direction][wheel[direction].Count - 1];

		PartModel end = new PartModel(Direction.NULL, lastPart.direction, DirectionPlusCol(lastPart.direction, lastPart.col), DirectionPlusRow(lastPart.direction, lastPart.row));

		if(parts.Contains(end)) return false;
		else {
			parts.Add(end);
			return true;
		}
	}

	Direction OppositeDir(Direction direction) {
		if(direction == Direction.UP) return Direction.DOWN;
		if(direction == Direction.DOWN) return Direction.UP;
		if(direction == Direction.LEFT) return Direction.RIGHT;
		if(direction == Direction.RIGHT) return Direction.LEFT;
		return Direction.NULL;
	}

	Direction NextDir(Direction[] dirs, Direction dir) {
		int idx = Array.IndexOf(dirs, dir);
		if(dirs.Length - 1 == idx)
			idx = 0;
		else
			idx++;
		return dirs[idx];
	}

	void BuildLevel() {
		parts = new List<PartModel>();
		Direction[] dirs = new Direction[] {Direction.DOWN, Direction.LEFT, Direction.RIGHT, Direction.UP};
		Randomizer r = Randomizer.New(GRID - 1);

		//Set start wall and direction. Set start part.
		Direction wall = RandomDirection(dirs);
		int startColumn = wall == Direction.LEFT || wall == Direction.RIGHT ? 0 : r.Next();
		int startRow = wall == Direction.LEFT || wall == Direction.RIGHT ? r.Next() : 0;
		Direction startDirection = StartDirection(dirs, startColumn, startRow);

		parts.Add(new PartModel(startDirection, Direction.NULL, startColumn, startRow));

		//Set middle and final parts. Chaos.

		int newCol = startColumn, newRow = startRow;
		Direction newDir = startDirection;

		while(parts.Count < (partQuantity + 2)){
			newCol = DirectionPlusCol(newDir, newCol);
			newRow = DirectionPlusRow(newDir, newRow);

			Randomizer dirRand = Randomizer.New(dirs.Length - 1);
			Direction oldDir = newDir;
			bool done = false;
			for(int i = 0; i < dirs.Length; i++) {
				newDir = dirs[dirRand.Next()];
				if(isApplicable(newDir, newCol, newRow) && !IsSameWay(oldDir, newDir)){
					PartModel newPart = new PartModel(newDir, oldDir, newCol, newRow);
					if(!parts.Contains(newPart)) {
						parts.Add(newPart);
						done = true;
						break;
					}
				}
			}
			if(!done) {
				//If I reach this segment it means that I have no way out. In that case, restart BuildLevel(). We should use a path algorithm.
				BuildLevel();return;
			}
		}

		//End part doesn't go anywhere.
		parts[parts.Count - 1].direction = Direction.NULL;

		AddDistractors();

		parts = Randomizer.RandomizeList(parts);
	}

	void AddDistractors() {
		
	}

	bool IsSameWay(Direction oldDir, Direction newDir) {
		return (oldDir == Direction.UP && newDir == Direction.DOWN) || (oldDir == Direction.DOWN && newDir == Direction.UP)
			|| (oldDir == Direction.LEFT && newDir == Direction.RIGHT) || (oldDir == Direction.RIGHT && newDir == Direction.LEFT);
	}

	int DirectionPlusRow(Direction d, int row) {
		if(d == Direction.UP) return row - 1;
		if(d == Direction.DOWN) return row + 1;
		return row;
	}

	int DirectionPlusCol(Direction d, int col) {
		if(d == Direction.LEFT) return col - 1;
		if(d == Direction.RIGHT) return col + 1;
		return col;
	}

	Direction StartDirection(Direction[] dirs, int startColumn, int startRow) {
		Randomizer dirRand = Randomizer.New(dirs.Length - 1);
		for(int i = 0; i < dirs.Length; i++) {
			Direction d = dirs[dirRand.Next()];
			if(isApplicable(d, startColumn, startRow)) return d;
		}
		return Direction.NULL;
	}

	bool isApplicable(Direction d, int col, int row) {
		if(d == Direction.LEFT && col <= 0) return false;
		if(d == Direction.RIGHT && col >= 5) return false;
		if(d == Direction.UP && row <= 0) return false;
		if(d == Direction.DOWN && row >= 5) return false;
		return true;
	}

	Direction RandomDirection(Direction[] dirs) {
		return dirs[Randomizer.New(dirs.Length - 1).Next()];
	}

	public List<PartModel> StartParts(){
		return parts.FindAll(p => p.previousDir == Direction.NULL && p.direction != Direction.NULL);
	}
	public List<PartModel> EndParts(){
		return parts.FindAll(p => p.direction == Direction.NULL && p.previousDir != Direction.NULL);
	}

	public List<PartModel> DraggerParts() {
		return parts.FindAll(p => p.isDouble || (p.direction != Direction.NULL && p.previousDir != Direction.NULL));
	}
}