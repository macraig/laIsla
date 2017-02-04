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

		public const int EMPTY_TILE = 26;

		public void Start(){
			model = new RompecabezasActivityModel();
			parts = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/RompecabezasActivity/tiles"));
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
				TimeLevel(model.CurrentLvl());
				model.WithTime();
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
		}

		void StartTimer(bool first = false) {
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
				t.GetComponent<RompecabezasSlot>().ResetSprite();
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
			if(IsCorrect()){
				//correct
				ShowRightAnswerAnimation();
				model.Correct();
				model.CorrectTimer();
				SetClock();
				model.NextLvl();
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

					if(currentPart.Model().direction == dir) {
						dir = currentPart.Model().previousDir;
					} else if(currentPart.Model().previousDir == dir) {
						dir = currentPart.Model().direction;
					} else
						return false;
				}
			}
			
			return true;
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

		public void RestartGame(){
			ResetTiles();
			Start();
		}
	}
}