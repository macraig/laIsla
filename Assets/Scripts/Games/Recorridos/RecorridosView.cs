using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecorridosView : MonoBehaviour {

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

    public Text nutTextCounter;

    private Image playerImage;
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
    }

    internal void ShowVictory()
    {
        centralPuppetImage.sprite = puppetVictory;
    }

    internal void ShowDefeat()
    {
        centralPuppetImage.sprite = puppetDefeat;
    }

    public void ResetGame()
    {
        centralPuppetImage.sprite = puppetNeutral;
        nutTextCounter.text = "0";
        MovingDown();
    }

    public void SetNutTextCounter(int newValue)
    {
        nutTextCounter.text = newValue.ToString();
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
}
