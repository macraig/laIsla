using System;
using UnityEngine.UI;
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;
using Assets.Scripts.App;

namespace Assets.Scripts.Games.Constelaciones {
	public class ConstelacionesView : LevelView {
		public static Color32 LINE_COLOR = new Color32 (250,255,208,255);

		private ConstelacionesModel model;
		public Compass compass;
		public GameObject mapPanel;
		public Text instruction;
		public Button redoBtn, undoBtn, okBtn, nextBtn,compassToggle, prevInstruction, nextInstruction;

		private List<VectorLine> lines, redoLines;
		private List<GameObject> stars, clickedStars, redoStars;
		private int currentInstruction;
		private Sprite starImage,blueStar, starSelectedImage,blueStarSelected;
		public Image endImage;
		public Material dottedLineMateriallMaterial;

		public const int HEIGHT_STEPS = 20, WIDTH_STEPS = 30;

		public void Start(){
			model = new ConstelacionesModel();
			starImage = Resources.Load<Sprite> ("Sprites/Constelaciones/star");
			starSelectedImage  = Resources.Load<Sprite> ("Sprites/Constelaciones/starSelected");
			blueStar = Resources.Load<Sprite> ("Sprites/Constelaciones/starBlue");
			blueStarSelected = Resources.Load<Sprite> ("Sprites/Constelaciones/starBlueSelected");
//			pinkStar = Resources.Load<Sprite> ("Sprites/Constelaciones/starPink");
//			pinkStarSelected = Resources.Load<Sprite> ("Sprites/Constelaciones/starPinkSelected");

			Begin();
		}

		public void Begin(){
			ShowExplanation();
		}

		override public void Next(bool first = false){
			if(model.GameEnded()) {
				EndGame(0, 0, 800);
			} else {
				currentInstruction = 0;
				ResetBoard();
				compass.ToggleCompassVisibility(false);
				endImage.gameObject.SetActive (false);
				nextBtn.gameObject.SetActive (false);
				SetCurrentLevel();
			}
		}

		void SetCurrentLevel() {
			ConstelacionesLevel lvl = model.CurrentLvl();

			SetSomeStars(lvl.GetStars(), true);
			SetSomeStars(lvl.GetFakeStars());
			SetEndImage (lvl.GetImage ());
			SetInstruction();
		}

		void SetEndImage (Sprite image)
		{
			endImage.sprite = image;
		}

		void SetSomeStars(List<Vector2> starsModel, bool correctStars = false) {
			float widthStep = mapPanel.GetComponent<RectTransform>().rect.width / WIDTH_STEPS;
			float heightStep = mapPanel.GetComponent<RectTransform>().rect.height / HEIGHT_STEPS;
			Vector2 middle = mapPanel.transform.position;

			for(int i = 0;i<starsModel.Count;i++){
				//If constelation ends in the same star as it started, add it again at the end of the list
				if (correctStars && i == starsModel.Count - 1) {
					if (starsModel [i].x == starsModel [0].x && starsModel [i].y == starsModel [0].y) {
						stars.Add(stars[0]);
						break;
					}
		
				}
				GameObject star = ViewController.GetController().GetStarPrefab(mapPanel);
				if(correctStars && i == 0) 
					star.transform.GetChild(0).GetComponent<Image>().sprite = blueStar;	
				

//				if(correctStars && i == 1) 
//					star.transform.GetChild(0).GetComponent<Image>().sprite = blueStar;
				

				float starX = starsModel[i].x * widthStep;
				float starY = starsModel[i].y * heightStep;
				Debug.Log("X: " + starX + " Y: " + starY + " jsonX: " + starsModel[i].x + " jsonY: " + starsModel[i].y);
				star.transform.localPosition = new Vector3(starX, starY);

				star.GetComponent<Button>().onClick.AddListener(() => StarClick(star));

				stars.Add(star);
			}

		}

		void StarClick(GameObject star) {
			//Assign a letter to the star if it doesn't have one.
			if (star.GetComponentInChildren<Text> ().text == "")
				star.GetComponentInChildren<Text> ().text = model.GetFirstStarLetter ();
			
			//Clicked the same star again
			if(clickedStars.Count > 0 && clickedStars[clickedStars.Count - 1] == star) return;

			//Check if the line has already been drawn
			if(lines.Count != 0 && IsLineAdded(clickedStars[clickedStars.Count - 1].transform.position, star.transform.position)) return;

			//Unselect last star and select current star
			if (clickedStars.Count > 0)
				SelectStar (clickedStars[clickedStars.Count-1],false);
			SelectStar (star, true);

			//Add star to clickedStars list
			clickedStars.Add(star);
			Debug.Log ("star added");

			//Check if it's the first star you add. In that case, return without drawing a line
			if(clickedStars.Count < 2){
				CheckButtons ();
				return;
			} 
			//Draw star line
			Line(clickedStars[clickedStars.Count - 2].transform.position, clickedStars[clickedStars.Count - 1].transform.position);
			Debug.Log("Line star");
			//Destroy redo lines as the future changed :o
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
					Debug.Log("Line already drawn!");
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
			SelectStar (star, true);
			SelectStar (clickedStars[clickedStars.Count-1],false);

			//Add letter if the star has no selections
			if(!clickedStars.Contains(star)){
				star.GetComponentInChildren<Text> ().text = model.GetFirstStarLetter ();
			}
			clickedStars.Add(star);

			redoStars.RemoveAt(redoStars.Count-1);

			CheckButtons();
		}

		public void UndoClick(){
			if (clickedStars.Count > 1) {
				//Remove last line added
				VectorLine lastLine = lines [lines.Count - 1];
				lines.Remove (lastLine);
				redoLines.Add (lastLine);
				lastLine.active = false;


				//Select last star selected
				SelectStar (clickedStars [clickedStars.Count - 2], true);
			} else {
				//Destroy redo lines
				VectorLine.Destroy(redoLines);
				redoLines = new List<VectorLine>();
				redoStars = new List<GameObject>();
			}


			//Remove last star clicked
			GameObject star = clickedStars[clickedStars.Count - 1];
			SelectStar (star, false);
			redoStars.Add(star);
			//IMPORTANT: Remove by index, removing by object won't work when selecting the same star more than once
			clickedStars.RemoveAt (clickedStars.Count-1);
			//Remove letter if the star has no more selections
			if(!clickedStars.Contains(star)){
				model.ReturnLetterToList (star.GetComponentInChildren<Text> ().text);
				star.GetComponentInChildren<Text> ().text = "";	
			}
		

			CheckButtons();
		}



		void SelectStar (GameObject star, bool select)
		{
			Sprite starSprite = star.transform.GetChild (0).GetComponent<Image> ().sprite;
			Sprite newSprite;
			Debug.Log ("Label"+star.GetComponentInChildren<Text> ().text);
			if (select) {
				if (starSprite == starImage)
					newSprite = starSelectedImage;
//				else if (starSprite == pinkStar)
//					newSprite = pinkStarSelected;
				else
					newSprite = blueStarSelected;

			} else {
				if (starSprite == starSelectedImage)
					newSprite = starImage;
//				else if (starSprite == pinkStarSelected)
//					newSprite = pinkStar;
				else
					newSprite = blueStar;
			}
			star.transform.GetChild (0).GetComponent<Image> ().sprite = newSprite;
		}

		public void OkClick(){
			if(IsCorrect()){
				model.LogAnswer(true);
				EndLevel ();


			} else {
				model.LogAnswer(false);
				ShowWrongAnswerAnimation();
			}
		}

		void EndLevel(){
			undoBtn.interactable = false;
			redoBtn.interactable = false;
			ShowEndImage ();
			nextBtn.gameObject.SetActive (true);
		}

		void ShowEndImage ()
		{
			endImage.gameObject.SetActive (true);

		}

		public void OnClickNextButton(){
			model.NextLvl();
			ShowRightAnswerAnimation();

		}

		//Checks how many stars were clicked and if their position is the same as the correct stars
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
			undoBtn.interactable = clickedStars.Count != 0;

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
			VectorLine line = new VectorLine("Curve", new List<Vector2> { from, to }, null, 6.0f, LineType.Continuous);

			line.SetCanvas(FindObjectOfType<Canvas>());
			line.material = dottedLineMateriallMaterial;
			line.textureScale = 1f;
			line.color = LINE_COLOR;
			line.Draw();
			lines.Add(line);
			line.rectTransform.transform.SetParent(mapPanel.transform);
		}

		override public void RestartGame(){
			base.RestartGame ();
			ResetBoard();
			Start();
		}
	}
}