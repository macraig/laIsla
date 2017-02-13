using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Common;
using SimpleJSON;
namespace Assets.Scripts.Games.Recorridos
{
public class RecorridosBoard {

	public static int ROWS = 6;
	public static int COLS = 6;

	public static int P_PATH = 25;
	public static int P_WALL = 25;
	public static int P_FIRE= 25;
	public static int P_HOLE = 25;
	public static int P_NEXT_WALL = 60;


	private List<List<int>> boardMatrix;
	private List<List<RecorridosTile>>  tiles;
	private Vector2 startPosition;
	private Vector2 finishPosition;
	private List<Vector2> path;
	List<RecorridosLevel> lvls;

	private Vector2 currentPosition;

		public RecorridosBoard(){
			lvls = new List<RecorridosLevel>();
			JSONArray lvlsJson = JSON.Parse(Resources.Load<TextAsset>("Jsons/RecorridosActivity/levels").text).AsObject["levels"].AsArray;
			foreach(JSONNode lvlJson in lvlsJson) {
				lvls.Add(new RecorridosLevel(lvlJson.AsObject));
			}
		}

		public List<RecorridosLevel> GetLevels(){
			return lvls;
		}

		private void ResetBoard (){
		/*
         Tile frames
         1:white
         2:black
         3:wall
         4:bomb
         5:hole
         6:fire
         7:nut
         8:start
         9:finish
         * */
			boardMatrix = new List<List<int>>();
			for (int i = 0; i < ROWS; i++) {
				boardMatrix.Add(new List<int> ());
				for (int j = 0; j < COLS; j++) {
					boardMatrix [i].Add(0);
				}
			}
				
		}

		public Vector2 GetStartPosition(){ return startPosition;}
		public Vector2 GetFinishPosition(){ return finishPosition;}

		public List<List<int>> GenerateBoard(RecorridosLevel level) {
			ResetBoard ();
			/*Start position*/
			GenerateStartPosition();

			/*Path and finish*/
			bool pathGenerated =false;
			while(!pathGenerated){
				pathGenerated= GeneratePath(level.GetPath());
			}

			/*Nuts: locates them along the path*/
			GenerateNuts(level.GetNuts());
			path=GetPath();

			/*Bombs: Random bombs on board*/
			GenerateBombs (level.GetBombs());

			/*Walls, fire, holes*/
			List<int> probabilityArray = BuildProbabilityArray();

			for (int j = 0;j<ROWS;j++){
				for (int k = 0;k<COLS;k++){
					if(boardMatrix[j][k]==0){
						int randomTile = probabilityArray[Randomizer.RandomInRange(probabilityArray.Count)];
						if(randomTile==3){
							BuildWalls(j,k,P_NEXT_WALL,3);
						}else{
							boardMatrix[j][k]=randomTile;
						}
					}
				}
			}
			PrintBoard();
//			paintTiles();

			return boardMatrix;


		}

		private void GenerateStartPosition() {
			int randomRow = Randomizer.RandomBoolean () ? Randomizer.RandomInRange (1) : Randomizer.RandomInRange (ROWS - 1, ROWS - 2);
			int randomCol = Randomizer.RandomBoolean () ? Randomizer.RandomInRange (1) : Randomizer.RandomInRange (COLS - 1, COLS - 2);

			startPosition = new Vector2(randomRow,randomCol);
			boardMatrix[(int)startPosition.x][(int)startPosition.y] = 8;
		}

		private bool GeneratePath(int pathLength) {
			int currentX = (int)startPosition.x;
			int currentY = (int)startPosition.y;

			for (int i = 0;i<pathLength;i++){
				Vector2 tile = GeneratePathTile(currentX,currentY);
				if(tile.x!=-1){

					boardMatrix[(int)tile.x][(int)tile.y]=1;
					currentX = (int)tile.x;
					currentY = (int)tile.y;
				}else{
					//No path found, start over
					ResetBoard();
					GenerateStartPosition();
					return false;
				}
			}

			finishPosition = GeneratePathTile(currentX,currentY);

			//Make sure finish position is far from start
			//Check ROWS
			if ((startPosition.x < 2 && finishPosition.x > 2)||(startPosition.x > 2 && finishPosition.x < ROWS -2)) {
				
				boardMatrix [(int)finishPosition.x] [(int)finishPosition.y] = 9;
					return true;
			} else {
				//Check COLS 
				if ((startPosition.y < 2 && finishPosition.y > 2) || (startPosition.y > COLS - 3 && finishPosition.y < COLS - 3)) {
					boardMatrix [(int)finishPosition.x] [(int)finishPosition.y] = 9;
					return true;
				} else {
					ResetBoard ();
					GenerateStartPosition ();
					return false;
				}
			}
				

		}

		private Vector2 GeneratePathTile(int currentX,int currentY) {
			List<Vector2> surroundingPoints = GetSurroundingPoints(currentX, currentY);

			List<Vector2> possibleTiles = new List<Vector2>();

			foreach (Vector2 point in surroundingPoints) {
				if (point.x > -1 && point.x < ROWS && point.y > -1 && point.y < COLS) {
					if (boardMatrix[(int)point.x][(int)point.y] == 0) {
						if (HasNoNeighbours(point)) {
							possibleTiles.Add(point);
						}

					}
				}
			}
			if (possibleTiles.Count == 0) {
				foreach(Vector2 point2 in surroundingPoints) {
					if (point2.x > -1 && point2.x < ROWS && point2.y > -1 && point2.y < COLS) {
						if (boardMatrix[(int)point2.x][(int)point2.y] == 0) {
							possibleTiles.Add(point2);
						}
					}
				}

			}
			Vector2 randomTile = possibleTiles[Randomizer.RandomInRange(possibleTiles.Count)];
			return randomTile;
		}

		private List<Vector2> GetSurroundingPoints(int x,int y) {
			return new List<Vector2>(){new Vector2(x-1,y),new Vector2(x+1,y),
				new Vector2(x,y-1),new Vector2(x,y+1)};
		}

		private bool HasNoNeighbours(Vector2 point) {
			int counter = 0;
			List<Vector2> neighbours = GetSurroundingPoints((int)point.x,(int)point.y);

			foreach(Vector2 p in neighbours){
				if(p.x>-1&&p.x<ROWS&&p.y>-1&&p.y<COLS){
					if(boardMatrix[(int)p.x][(int)p.y]!=0){
						counter++;
					}
				}
			}

			return counter<2;
		}

		private void GenerateNuts(int nuts) {
			path = GetPath();
			Randomizer.RandomizeList(path);
			for (int i = 0; i < nuts; i++) {
				boardMatrix[(int)path[i].x][(int)path[i].y]=7;
			}
		}

		private void GenerateBombs(int bombs) {
			for (int i = 0; i < bombs; i++) {
				Vector2 randomTile = new Vector2 (Randomizer.RandomInRange (ROWS), Randomizer.RandomInRange (COLS));
				while (boardMatrix [(int)randomTile.x] [(int)randomTile.y] != 0) {
					randomTile = new Vector2 (Randomizer.RandomInRange (ROWS), Randomizer.RandomInRange (COLS));
				}
				boardMatrix[(int)randomTile.x] [(int)randomTile.y]=4;
			}
		}



		private List<Vector2> GetPath() {
			List<Vector2> pathArray = new List<Vector2>();
			for (int i = 0;i<ROWS;i++){
				for (int j = 0;j<COLS;j++){
					if(boardMatrix[i][j]==1){
						pathArray.Add(new Vector2(i,j));
					}
				}
			}
			return pathArray;
		}

		private List<int> BuildProbabilityArray() {
			List<int> probabilityArray = new List<int> ();
			int[] numsToGenerate = new int[]{1,3,5,6};
			int[] probabilities = new int[]{P_PATH,P_WALL,P_HOLE,P_FIRE};

			for (int i=0;i<numsToGenerate.Length;i++){
				int amountOfNums = Randomizer.RandomInRange(probabilities[i]);
				for(int j = 0;j<amountOfNums;j++){
					probabilityArray.Add(numsToGenerate[i]);
				}
			}
			return probabilityArray;

		}

		private void BuildWalls(int currentX,int currentY,int wallProbability,int numberOfMaxWalls) {

			boardMatrix[currentX][currentY]=3;
			if(Randomizer.RandomInRange(100)<wallProbability){
				Vector2 tile = GenerateWallTile(currentX,currentY);
				if(tile.x!=-1){
					if(numberOfMaxWalls>0) {
						BuildWalls((int)tile.x,(int)tile.y,wallProbability/2,numberOfMaxWalls-1);
					}else{
						BuildWalls((int)tile.x,(int)tile.y,wallProbability/2,numberOfMaxWalls);
					}

				}
			}
		}

		private Vector2 GenerateWallTile(int currentX, int currentY) {
			List<Vector2> surroundingPoints = GetSurroundingPoints(currentX,currentY);

			List<Vector2> possibleTiles = new List<Vector2>();

			foreach (Vector2 point in surroundingPoints){
				if(point.x>-1&&point.x<ROWS&&point.y>-1&&point.y<COLS){
					if(boardMatrix[(int)point.x][(int)point.y]==0){
						possibleTiles.Add(point);
					}
				}
			}

			if(possibleTiles.Count==0){
				return new Vector2(-1,-1);
			}

			Vector2 randomTile = possibleTiles[Randomizer.RandomInRange(possibleTiles.Count)];
			return randomTile;
		}

		private void PrintBoard() {

			string row = "";
			for(int i = 0;i<ROWS;i++){
				for (int j = 0;j<COLS;j++){
					row+=",";
					row+=boardMatrix[i][j];
				}
				Debug.Log(row);
				row="";
			}
		}

		public int Cols(){
			return COLS;
		}

		public int Rows(){
			return ROWS;
		}

}
}