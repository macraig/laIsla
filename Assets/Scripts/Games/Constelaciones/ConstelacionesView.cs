using System;
using UnityEngine.UI;
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;
using Assets.Scripts.App;

namespace Assets.Scripts.Games.Constelaciones {
	public class ConstelacionesView : LevelView {
		private ConstelacionesModel model;
		public Compass compass;
		public GameObject mapPanel;
		public Text instruction;
		public Button redoBtn, undoBtn, okBtn, compassToggle, prevInstruction, nextInstruction;

		private List<VectorLine> lines, redoLines;
		private List<GameObject> stars, clickedStars, redoStars;
		private int currentInstruction;

		public const int HEIGHT_STEPS = 20, WIDTH_STEPS = 20;

		public void Start(){
			model = new ConstelacionesModel();
			Begin();
		}

		public void Begin(){
			ShowExplanation();
		}

		override public void Next(bool first = false){
			if(model.GameEnded()) {
				EndGame(60, 0, 1250);
			} else {
				currentInstruction = 0;
				ResetBoard();
				compass.ToggleCompassVisibility(false);
				SetCurrentLevel();
			}
		}

		void SetCurrentLevel() {
			ConstelacionesLevel lvl = model.CurrentLvl();

			SetSomeStars(lvl.GetStars(), true);
			SetSomeStars(lvl.GetFakeStars());

			SetInstruction();
		}

		void SetSomeStars(List<Vector2> starsModel, bool first = false) {
			float widthStep = mapPanel.GetComponent<RectTransform>().rect.width / WIDTH_STEPS;
			float heightStep = mapPanel.GetComponent<RectTransform>().rect.height / HEIGHT_STEPS;
			Vector2 middle = mapPanel.transform.position;

			int i = 0;
			starsModel.ForEach(starModel => {
				GameObject star = ViewController.GetController().GetStarPrefab(mapPanel);
				if(first && i == 0) star.transform.GetChild(0).GetComponent<Image>().color = Color.red;
				if(first && i == 1) star.transform.GetChild(0).GetComponent<Image>().color = Color.green;
				float starX = starModel.x * widthStep;
				float starY = starModel.y * heightStep;
				Debug.Log("X: " + starX + " Y: " + starY + " jsonX: " + starModel.x + " jsonY: " + starModel.y);
				star.transform.localPosition = new Vector3(starX, starY);

				star.GetComponent<Button>().onClick.AddListener(() => StarClick(star));

				stars.Add(star);
				i++;
			});
		}

		void StarClick(GameObject star) {
			if(clickedStars.Count > 0 && clickedStars[clickedStars.Count - 1] == star) return;

			if(lines.Count != 0 && IsLineAdded(clickedStars[clickedStars.Count - 1].transform.position, star.transform.position)) return;

			clickedStars.Add(star);

			if(clickedStars.Count < 2) return;

			Line(clickedStars[clickedStars.Count - 2].transform.position, clickedStars[clickedStars.Count - 1].transform.position);
			Debug.Log("Line star");

			VectorLine.Destroy(redoLines);
			redoLines = new List<VectorLine>();
			redoStars = new List<GameObject>();
			CheckButtons();
		}

		bool IsLineAdded(Vector2 star, Vector2 otherStar) {
			foreach(VectorLine line in lines) {
				Vector2 pointOne = line.points2[0];
				Vector2 pointTwo = line.points2[1];

				if((pointOne == star && pointTwo == otherStar) || (pointOne == otherStar && pointTwo == star)){
					Debug.Log("Line already drawn!!!!!!!!");
					return true;
				}
			}
			return false;
		}

		void ResetBoard() {
			redoLines = new List<VectorLine>();
			redoStars = new List<GameObject>();
			clickedStars = new List<GameObject>();

			if(lines != null) VectorLine.Destroy(lines);
			if(stars != null) stars.ForEach(Destroy);
			lines = new List<VectorLine>();
			stars = new List<GameObject>();
		}

		public void ToggleCompass(){
			compass.ToggleCompassVisibility(!compass.gameObject.activeSelf);
		}

		public void RedoClick(){
			VectorLine lastLine = redoLines[redoLines.Count - 1];
			redoLines.Remove(lastLine);
			lines.Add(lastLine);
			lastLine.active = true;

			GameObject star = redoStars[redoStars.Count - 1];
			redoStars.Remove(star);
			clickedStars.Add(star);

			CheckButtons();
		}

		public void UndoClick(){
			VectorLine lastLine = lines[lines.Count - 1];
			lines.Remove(lastLine);
			redoLines.Add(lastLine);
			lastLine.active = false;

			GameObject star = clickedStars[clickedStars.Count - 1];
			clickedStars.Remove(star);
			redoStars.Add(star);

			CheckButtons();
		}

		public void OkClick(){
			if(IsCorrect()){
				model.LogAnswer(true);
				model.NextLvl();
				ShowRightAnswerAnimation();
			} else {
				model.LogAnswer(false);
				ShowWrongAnswerAnimation();
			}
		}

		bool IsCorrect() {
			if(clickedStars.Count != model.CurrentLvl().GetStars().Count) return false;

			for(int i = 0; i < clickedStars.Count; i++) {
				if(clickedStars[i].transform.position != stars[i].transform.position) return false;
			}

			return true;
		}

		void SetInstruction() {
			instruction.text = model.CurrentLvl().GetInstructions()[currentInstruction];

			CheckButtons();
		}

		void CheckButtons() {
			redoBtn.interactable = redoLines.Count != 0;
			undoBtn.interactable = lines.Count != 0;

			okBtn.interactable = lines.Count > 0;

			prevInstruction.interactable = currentInstruction != 0;
			nextInstruction.interactable = currentInstruction != (model.CurrentLvl().GetInstructions().Count - 1);
		}

		public void PreviousInstruction(){
			currentInstruction--;
			SetInstruction();
		}

		public void NextInstruction(){
			currentInstruction++;
			SetInstruction();
		}

		public void Line(Vector3 from, Vector3 to){
			VectorLine line = new VectorLine("Curve", new List<Vector2> { from, to }, null, 8.0f, LineType.Continuous);

			line.SetCanvas(FindObjectOfType<Canvas>());

			line.textureScale = 1f;
			line.Draw();
			lines.Add(line);
			line.rectTransform.transform.SetParent(mapPanel.transform);
		}

		public void RestartGame(){
			ResetBoard();
			Start();
		}
	}
}