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
		public Button redoBtn, undoBtn, okBtn, compassToggle;

		private List<VectorLine> lines;
		private List<GameObject> stars;
		private int currentInstruction;

		public const int HEIGHT_STEPS = 10, WIDTH_STEPS = 10;

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

			SetSomeStars(lvl.GetStars());
			SetSomeStars(lvl.GetFakeStars());

			SetInstruction();
		}

		void SetSomeStars(List<Vector2> starsModel) {
			starsModel.ForEach(starModel => {
				float widthStep = mapPanel.GetComponent<RectTransform>().rect.width / WIDTH_STEPS;
				float heightStep = mapPanel.GetComponent<RectTransform>().rect.height / HEIGHT_STEPS;

				GameObject star = ViewController.GetController().GetStarPrefab(mapPanel);
				star.transform.localPosition = new Vector2(starModel.x * widthStep, starModel.y * heightStep);
				stars.Add(star);
			});
		}

		void ResetBoard() {
			if(lines != null) VectorLine.Destroy(lines);
			if(stars != null) stars.ForEach(Destroy);
			lines = new List<VectorLine>();
			stars = new List<GameObject>();
		}

		public void ToggleCompass(){
			compass.ToggleCompassVisibility(!compass.gameObject.activeSelf);
		}

		public void RedoClick(){
			
		}

		public void UndoClick(){

		}

		public void OkClick(){

		}

		void SetInstruction() {
			instruction.text = model.CurrentLvl().GetInstructions()[currentInstruction];

			CheckButtons();
		}

		void CheckButtons() {
			
		}

		public void PreviousInstruction(){
			currentInstruction--;
		}

		public void NextInstruction(){
			currentInstruction++;
		}

		public void Point(Vector3 from, Vector3 to){
			VectorLine line = new VectorLine("Curve", new List<Vector2> { from, to }, null, 8.0f, LineType.Continuous);

			line.SetCanvas(FindObjectOfType<Canvas>());

			line.textureScale = 1f;
			line.Draw();
			lines.Add(line);
			//line.rectTransform.transform.SetParent(Ruler.transform.parent);
		}

		public void RestartGame(){
			ResetBoard();
			Start();
		}
	}
}