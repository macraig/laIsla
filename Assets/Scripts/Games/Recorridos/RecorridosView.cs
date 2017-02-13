using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public void AddInstruction(RecorridosAction actionToAdd)
    {
        stackImages[currentAvailableInstructionSpot].GetComponent<Image>().sprite = actionToAdd.sprite;
        stackImages[currentAvailableInstructionSpot].SetActive(true);
        //stackImages[currentAvailableInstructionSpot].GetComponent<RecorridosButton>().indexInList = currentAvailableInstructionSpot;
        for(int i = 0; i < stackImages.Count; i++)
        {
            if (!stackImages[i].activeSelf)
            {
                currentAvailableInstructionSpot = i;
                return;
            }
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
	

	}
}
