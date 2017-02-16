using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common;


namespace Assets.Scripts.Games.RompecabezasActivity {
	public class RompecabezasActivityView : LevelView {
		public Text clock;
		public Button okBtn;

		public List<Image> tiles;
		public List<Part> draggers;

		bool timerActive;
		private List<Sprite> parts;
		private RompecabezasActivityModel model;
		public GameObject placaClock;
		private bool firstTimeLevel;

		public const int EMPTY_TILE = 26;

		public void Start(){
			model = new RompecabezasActivityModel();
			parts = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/MuellesActivity/tiles"));
			placaClock.SetActive (false);
			firstTimeLevel = true;
			Begin();
		}

		public void Begin(){
			ShowExplanation();
		}

		override public void Next(bool first = false){
			if(model.GameEnded()) {
				EndGame(60, 0, 1250);
			} else {
				ResetTiles();
				SetCurrentLevel();
			}
		}

		void SetCurrentLevel() {
			if(model.HasTime()){
				if (firstTimeLevel) {
					firstTimeLevel = false;
					Invoke("ShowNextLevelAnimation",0.3f);
				} else {
					TimeLevel(model.CurrentLvl());
					model.WithTime();
				}

			} else {
				NormalLevel(model.CurrentLvl());
			}
		}

		void NormalLevel(RompecabezasLevel lvl) {
			clock.gameObject.SetActive(false);

			SetStartParts(lvl.StartParts());
			SetEndParts(lvl.EndParts());

			SetDraggers(lvl.DraggerParts());
		}

		override public void OnNextLevelAnimationEnd(){
			base.OnNextLevelAnimationEnd ();
			TimeLevel(model.CurrentLvl());
			model.WithTime();
		}

		void SetDraggers(List<PartModel> draggerParts) {
			for(int i = 0; i < draggers.Count; i++) {
				draggers[i].gameObject.SetActive(i < draggerParts.Count);
				if(i < draggerParts.Count){
					draggers[i].SetModel(draggerParts[i], parts);
				}
			}

		}

		void SetEndParts(List<PartModel> parts) {
			foreach(PartModel end in parts) {
				tiles[TileNumber(end.row, end.col)].GetComponent<RompecabezasSlot>().SetEnd(end);
			}
		}

		void SetStartParts(List<PartModel> parts) {
			foreach(PartModel start in parts) {
				tiles[TileNumber(start.row, start.col)].GetComponent<RompecabezasSlot>().SetStart(start);
			}
		}

		public int TileNumber(int row, int col){
			return (row * RompecabezasLevel.GRID) + col;
		}

		void TimeLevel(RompecabezasLevel lvl) {
			clock.gameObject.SetActive(true);
			SetClock();
			StartTimer(true);

			SetStartParts(lvl.StartParts());
			SetEndParts(lvl.EndParts());

			SetDraggers(lvl.DraggerParts());
		}

		void StartTimer(bool first = false) {
			if (first)
				PlayTimeLevelMusic ();
			
			placaClock.SetActive (true);
			StartCoroutine(TimerFunction(first));
			timerActive = true;

		}

		public IEnumerator TimerFunction(bool first = false) {
			yield return new WaitForSeconds(1);
			Debug.Log("segundo");

			UpdateView();

			if(timerActive) StartTimer();
		}

		void SetClock() {
			clock.text = model.GetTimer().ToString();
		}

		void UpdateView() {
			model.DecreaseTimer();

			SetClock();

			if(model.IsTimerDone()){
				timerActive = false;
				EndGame(60, 0, 1250);
			}
		}

		void ResetTiles() {
			tiles.ForEach(t => {
				t.GetComponent<RompecabezasSlot>().Reset();
				t.GetComponent<RompecabezasSlot>().EndSlot(false);
				t.GetComponent<RompecabezasSlot>().StartSlot(false);
			});
		}

		public void OkClick() {
			if(model.HasTime()){
				TimeOkClick();
			} else {
				NoTimeOkClick();
			}
		}

		void TimeOkClick() {
			timerActive = false;
			clock.gameObject.SetActive(false);
			if(IsCorrect()){
				//correct
				model.NextLvl();
				ShowRightAnswerAnimation();
				model.Correct();
				SetClock();
			} else {
				PlayWrongSound();
				model.Wrong();
				EndGame(60, 0, 1250);
			}
		}

		void NoTimeOkClick() {
			if(IsCorrect()){
				//correct
				ShowRightAnswerAnimation();
				model.Correct();
				model.NextLvl();
			} else {
				PlayWrongSound();
				model.Wrong();
			}
		}

		bool IsCorrect() {
			List<PartModel> startParts = model.CurrentLvl().StartParts();

			foreach(PartModel startPart in startParts) {
				int newRow = startPart.row;
				int newCol = startPart.col;

				Direction dir = startPart.direction;

				while(true){
					newRow = DirectionPlusRow(dir, newRow);
					newCol = DirectionPlusCol(dir, newCol);

					RompecabezasSlot slot = tiles[TileNumber(newRow, newCol)].GetComponent<RompecabezasSlot>();

					if(slot.IsEnd()) break;

					Part currentPart = slot.GetCurrent();

					if(currentPart == null) return false;

					PartModel m = currentPart.Model();
					if(m.isFork){
						List<Direction> forkDirs = GetForkDirs(m.middleFork);
						//remove the direction where the start part comes from
						forkDirs.Remove(OppositeDir(dir));

						//look for end parts following the other paths.
						foreach(Direction forkDir in forkDirs) {
							if(!IsForkDirectionCorrect(forkDir, newRow, newCol)) return false;
						}
						return true;
					} else if(m.isDouble){
						if(!m.isCross){
							dir = NotCrossDouble(dir, m.isLeftUp);
						}
					} else {
						dir = GetNextDirection(m, dir);

						if(dir == Direction.NULL) return false;
					}
				}
			}
			
			return true;
		}

		bool IsForkDirectionCorrect(Direction forkDir, int row, int col) {
			int newRow = row;
			int newCol = col;
			Direction dir = forkDir;
			while(true){
				newRow = DirectionPlusRow(dir, newRow);
				newCol = DirectionPlusCol(dir, newCol);

				RompecabezasSlot slot = tiles[TileNumber(newRow, newCol)].GetComponent<RompecabezasSlot>();

				if(slot.IsEnd()) break;

				Part currentPart = slot.GetCurrent();

				if(currentPart == null) return false;

				PartModel m = currentPart.Model();

				//as it is exclusive for fork directions, we cant find another complex part
				if(m.isDouble || m.isFork) return false;

				dir = GetNextDirection(m, dir);
				if(dir == Direction.NULL) return false;
			}
			return true;
		}

		List<Direction> GetForkDirs(Direction middleFork) {
			if(middleFork == Direction.DOWN) return new List<Direction> { Direction.DOWN, Direction.LEFT, Direction.RIGHT };
			if(middleFork == Direction.UP) return new List<Direction> { Direction.UP, Direction.LEFT, Direction.RIGHT };
			if(middleFork == Direction.LEFT) return new List<Direction> { Direction.DOWN, Direction.LEFT, Direction.UP };
			if(middleFork == Direction.RIGHT) return new List<Direction> { Direction.DOWN, Direction.UP, Direction.RIGHT };
			return new List<Direction>();
		}

		Direction GetNextDirection(PartModel m, Direction dir) {
			//vamos a cablearlo.....

			//if(m.direction == m.previousDir && (m.direction == dir || m.direction == OppositeDir(dir))) return dir;

			if(m.previousDir == dir)
				return m.direction;
			else if(OppositeDir(m.direction) == dir)
				return OppositeDir(m.previousDir);
			else
				return Direction.NULL;
		}

		Direction OppositeDir(Direction direction) {
			if(direction == Direction.UP) return Direction.DOWN;
			if(direction == Direction.DOWN) return Direction.UP;
			if(direction == Direction.LEFT) return Direction.RIGHT;
			if(direction == Direction.RIGHT) return Direction.LEFT;
			return Direction.NULL;
		}

		Direction NotCrossDouble(Direction dir, bool isLeftUp) {
			if(dir == Direction.RIGHT) return isLeftUp ? Direction.UP : Direction.DOWN;
			if(dir == Direction.LEFT) return isLeftUp ? Direction.DOWN : Direction.UP;
			if(dir == Direction.UP) return isLeftUp ? Direction.RIGHT : Direction.LEFT;
			if(dir == Direction.DOWN) return isLeftUp ? Direction.LEFT : Direction.RIGHT;
			return Direction.NULL;
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

		public Sprite PartSprite(int index){
			return parts[index];
		}

		override public void RestartGame(){
			base.RestartGame ();
			placaClock.SetActive (false);
			timerActive = false;
			firstTimeLevel = true;
			ResetTiles();
			Start();
		}
	}
}