using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Metrics.Model{
public class MetricsRaw {

    private GameObject fullObject;
	//Index if images to be replaced in each Raw
	public static int STAR_START_INDEX = 3;
	public static int ICON_INDEX = 8;
	private Sprite star;


	public MetricsRaw(GameObject fullObject,Sprite star )
    {
        this.fullObject = fullObject;
		this.star = star;
    }

	// Use this for initialization
	void Start () {
       
    }
	
    public void setActivity(string activityName)
    {
        fullObject.GetComponentsInChildren<Text>(true)[0].text = activityName;
    }

    public void setScore(int currentScore)
    {
        fullObject.GetComponentsInChildren<Text>(true)[1].text = (currentScore == 0 ? "-" : "" + currentScore);
    }

    public void setStars(int currentStars)
    {
			int endIndex = STAR_START_INDEX + currentStars;
			for(int i = STAR_START_INDEX; i < endIndex; i++)
        {            
				Image starImage = fullObject.GetComponentsInChildren<Image> (true) [i];
				starImage.sprite=star;
        }
    }

		public void SetIcon (Sprite icon)
		{
			fullObject.GetComponentsInChildren<Image> (true) [ICON_INDEX].sprite=icon;
		}

    public Button getViewDetailsBtn(){
        return fullObject.GetComponentInChildren<Button>();
    }

    internal void Hide(){
        fullObject.SetActive(false);
    }

    internal void Show(){
        fullObject.SetActive(true);
    }
}
}