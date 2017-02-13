using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.Scripts.Common;
using Assets.Scripts.Sound;

namespace Assets.Scripts.Games.Recorridos
{
public class RecorridosView : LevelView {

		#region implemented abstract members of LevelView

		public override void Next(bool first = false) {
			throw new NotImplementedException();
		}

		#endregion

	public GameObject instructionsStack;
		public List<GameObject> instructions;

    private List<GameObject> stackImages;
    private int currentAvailableInstructionSpot;

    public Image centralPuppetImage;

    public Sprite puppetVictory;
    public Sprite puppetDefeat;
    public Sprite puppetNeutral;

    public Sprite movingRight;
    public Sprite movingLeft;
    public Sprite movingUp;
    public Sprite movingDown;

    public Text nutTextCounter,clock;

    private Image playerImage;
	public GameObject bombAnimation,clockPlaca;
	private bool timerActive;

	private AudioClip bombSound, nutSound, fallSound, fireSound;
		public List<Text> pointsTexts,rosesTexts;
		public GameObject cardinalRose;
		private int roseRotation;

		private Sprite[] directionSprites;


    private void Start()
    {
        stackImages = new List<GameObject>();
        for(int i = 0; i < instructionsStack.transform.childCount; i++)
        {
            stackImages.Add(instructionsStack.transform.GetChild(i).gameObject);
            stackImages[i].SetActive(false);
            stackImages[i].GetComponent<RecorridosAction>().indexInList = i;
        }
        currentAvailableInstructionSpot = 0;
			clockPlaca.SetActive (false);

			bombSound = Resources.Load<AudioClip> ("Audio/RecorridosActivity/bomba");
			nutSound = Resources.Load<AudioClip> ("Audio/RecorridosActivity/coin");
			fallSound = Resources.Load<AudioClip> ("Audio/RecorridosActivity/fall");
			fireSound = Resources.Load<AudioClip> ("Audio/RecorridosActivity/fire");

			directionSprites = Resources.LoadAll<Sprite> ("Sprites/RecorridosActivity/direcciones");

			roseRotation = 0;
    }

		public void PlayFallSound(){
			SoundController.GetController ().PlayClip (fallSound);
		}

		public void PlayFireSound(){
			SoundController.GetController ().PlayClip (fireSound);
		}

		public void PlayNutSound(){
			SoundController.GetController ().PlayClip (nutSound);
		}

	override public void HideExplanation(){
		PlaySoundClick ();
		explanationPanel.SetActive (false);
		menuBtn.enabled = true;
	}

		public void ShowPlayer ()
		{
			playerImage.gameObject.SetActive (true);
		}

  

		public void AddInstruction (string dir)
		{
			foreach (GameObject dirImg in instructions) {
				if (!dirImg.activeSelf) {
					dirImg.SetActive (true);
					dirImg.GetComponent<Image>().sprite = GetDirectionSprite(dir);
					break;
				}
			}
		}

		Sprite GetDirectionSprite (string dir)
		{
			switch (dir) {
			case "NORTH":
				return directionSprites [1];
			case "EAST":
				return directionSprites [3];
			case "SOUTH":
				return directionSprites [5];
			case "WEST":
				return directionSprites [7];
			default:
				return directionSprites [0];
			}
		}

    public void RemoveInstruction(int instructionIndex)
    {

        if (instructionIndex == stackImages.Count - 1)
        {
            currentAvailableInstructionSpot = instructionIndex;
            stackImages[instructionIndex].SetActive(false);
        }
        else if (!stackImages[instructionIndex + 1].activeSelf)
        {
            currentAvailableInstructionSpot = instructionIndex;
            stackImages[instructionIndex].SetActive(false);

        }
        else
        {

            while (stackImages[instructionIndex+1].activeSelf)
            {
                stackImages[instructionIndex].GetComponent<Image>().sprite = stackImages[instructionIndex + 1].GetComponent<Image>().sprite;
                instructionIndex++;
                if(instructionIndex == stackImages.Count-1)
                {
                    break;
                }
            }

            currentAvailableInstructionSpot = instructionIndex;
            stackImages[instructionIndex].SetActive(false);
        }




    }

    internal void SetPlayerImage(Image playerImage)
    {
        this.playerImage = playerImage;
    }

    public void LightValue(int currentValueAnalyzed, bool on, Color color)
    {
        if (on)
        {
            stackImages[currentValueAnalyzed].GetComponent<Image>().color = color;
        }else
        {
            stackImages[currentValueAnalyzed].GetComponent<Image>().color = Color.white;
        }
    }

    internal void ResetStackView()
    {
        foreach(GameObject stackImage in stackImages)
        {
            stackImage.SetActive(false);
            stackImage.GetComponent<Image>().color = Color.white;
        }
        currentAvailableInstructionSpot = 0;
        MovingDown();
		EnableComponents (true);
		RotateCardinalPoints ();
    }

    internal void ShowVictory()
    {
//        centralPuppetImage.sprite = puppetVictory;
		ShowRightAnswerAnimation ();
    }

    internal void ShowDefeat()
    {
//        centralPuppetImage.sprite = puppetDefeat;
			ShowWrongAnswerAnimation ();
    }

    public void ResetGame()
    {
		timerActive = false;
//        centralPuppetImage.sprite = puppetNeutral;
        nutTextCounter.text = "0";
		
        MovingDown();
    }

	override public void RestartGame(){
			base.RestartGame ();
			clockPlaca.SetActive(false);
			ResetGame ();

					
	}

    public void SetNutTextCounter(int newValue)
    {
        nutTextCounter.text = newValue.ToString();
    }

		public void ShowBombAnimation ()
		{


			playerImage.gameObject.SetActive (false);
			bombAnimation.transform.SetAsLastSibling ();
			bombAnimation.GetComponent<BombAnimationScript>().ShowAnimation();

			SoundController.GetController ().PlayClip (bombSound);
		}

    internal void MovingLeft()
    {
        playerImage.sprite = movingLeft;
    }

    internal void MovingRight()
    {
        playerImage.sprite = movingRight;
    }

    internal void MovingUp()
    {
        playerImage.sprite = movingUp;
    }

    internal void MovingDown()
    {
        playerImage.sprite = movingDown;

    }

		override public void OnNextLevelAnimationEnd(){
			base.OnNextLevelAnimationEnd ();
			PlayTimeLevelMusic ();
//			menuBtn.interactable = false;
			PlayTimeLevel ();
			StartTimer (true);
		}

		public void PlayTimeLevel(){
			clockPlaca.SetActive(true);
			SetClock();
			RecorridosController.instance.ResetGame ();
		}

		void SetClock() {
			clock.text = RecorridosController.instance.GetTimer().ToString();
		}

		void StartTimer(bool first = false) {
			StartCoroutine(TimerFunction(first));
			timerActive = true;
		}

		public IEnumerator TimerFunction(bool first = false) {
			yield return new WaitForSeconds(1);
		
			UpdateView();
			if(timerActive) StartTimer();
		}

		void UpdateView() {
			RecorridosController.instance.DecreaseTimer();

			SetClock();

			if(RecorridosController.instance.IsTimerDone()){
				timerActive = false;
				EndGame(60, 0, 1250);
			}
		}

		override public void OnRightAnimationEnd(){
			EnableComponents (true);

		}

		public void RotateCardinalPoints(){
			//Make grid point texts dissappear
			pointsTexts.ForEach ((Text t ) => t.gameObject.SetActive(false));

			//Rotate rose components
			roseRotation+= 90;
			Debug.Log (roseRotation);
			if (roseRotation == 360)
				roseRotation = 0;
			
			cardinalRose.transform.DOLocalRotate(new Vector3(0,0,roseRotation),2).OnComplete(ShowPointTexts);
			rosesTexts.ForEach ((Text t ) => t.gameObject.transform.DOLocalRotate(new Vector3(0,0,-roseRotation),2));

		}

		private void ShowPointTexts(){
			string firstString = pointsTexts [0].text;
			for (int i=0; i < pointsTexts.Count-1; i++) {
				pointsTexts [i].text = pointsTexts [i+1].text;
			}
			pointsTexts [pointsTexts.Count-1].text = firstString;
			pointsTexts.ForEach ((Text t ) => t.gameObject.SetActive(true));
		}

		public void OnClickDirectionImage(Button dirBtn){
			SoundController.GetController ().PlayDropSound ();
			dirBtn.gameObject.SetActive (false);
		}
	
		public void OnClickDirection(string dir){
			PlaySoundClick();
			AddInstruction (dir);
		}

		public List<RecorridosAction> GetActionsToDo ()
		{
			List<RecorridosAction> instructionList = new List<RecorridosAction> ();
			foreach (GameObject dirImg in instructions) {
				if (dirImg.activeSelf) {
					instructionList.Add (ParseAction(dirImg.GetComponent<Image>().sprite));
				}
			}
			return null;
		}

		private RecorridosAction ParseAction (Sprite sprite)
		{
			string direction;
			if (sprite == directionSprites [1]) {
				direction = "N";
			} else if (sprite == directionSprites [3]) {
				direction = "E";
			} else if (sprite == directionSprites [5]) {
				direction = "S"; 
			}else{
				direction = "O"; 

			}
			return new RecorridosAction();


		}

	}
}
