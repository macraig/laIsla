using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.App;
using Assets.Scripts.Settings;
using Assets.Scripts.Common;
using Assets.Scripts.Sound;
using Assets.Scripts.Metrics.Model;
using System;

namespace Assets.Scripts.Metrics.View
{
	public class ResultsView : MonoBehaviour
	{
		private const int MAX_ROWS = 4;

//		public GameObject resultsView;

		public Button nextPageBtn;
		public Button prevPageBtn;
		public List<GameObject> viewRaws;
		public Text username;
		private List<MetricsRaw> raws;
		private int currentPage;
		private List<int> currentRawsToViewDetails;
		public Text noGamesWarning;
		private Sprite star;

		public GameObject detailsView;
		public Button goBackBtn;

		// Use this for initialization
		void Start(){
			currentPage = 0;
			star = Resources.Load<Sprite> ("Sprites/star");
				

			raws = new List<MetricsRaw>(viewRaws.Count);
			for (int i = 0; i < viewRaws.Count; i++){
				raws.Add(new MetricsRaw(viewRaws[i],star));
			}

			currentRawsToViewDetails = new List<int>(6);
			UpdateUsername ();
			updateArrows();
			UpdateMetricRows();
		}

		private void UpdateUsername(){
			username.text = SettingsController.GetController ().GetUsername ().ToUpper ();
		}

		public void OnClicNextPageBtn(){
			PlaySoundClick();
			currentPage++;
			UpdateMetricRows();
			updateArrows();
		}

		public void OnClicPrevPageBtn(){
			PlaySoundClick();
			currentPage--;
			UpdateMetricRows();
			updateArrows();
		}

		public void OnClickCrossBtn(){
			PlaySoundClick();
			gameObject.SetActive (false);
		}

		public void OnClickViewDetailsCrossBtn()
		{
			PlaySoundClick();
			//			detailsView.ResetChart ();
			detailsView.SetActive(false);
//			/resultsView.SetActive(true);
		}

		private void PlaySoundClick()
		{
			SoundController.GetController().PlayClickSound();
		}

		private void UpdateMetricRows(){
			currentRawsToViewDetails.RemoveAll(e => true);
			List<int> activities = MetricsController.GetController().metricsModel.GetLevelIndexes(currentPage, MAX_ROWS);
			currentRawsToViewDetails.AddRange(activities);
			if (activities.Count != 0)
				noGamesWarning.enabled = false;
			int i = 0;
			for (; i < MAX_ROWS && i < activities.Count; i++){
				UpdateRow(MetricsController.GetController().metricsModel.GetBestMetric(activities[i]), i, activities[i]);
				raws[i].Show();
			}
			for(; i < MAX_ROWS; i++){
				raws[i].Hide();
			}        
		}

		private void UpdateRow(GameMetrics gameMetrics, int rowIndex, int activity){
			Game game = AppController.GetController ().GetAppModel ().GetGameById (gameMetrics.GetGameId ());
			raws[rowIndex].setActivity(game.GetName());
			raws [rowIndex].SetIcon (game.GetIcon());

			if (gameMetrics != null){
//				raws[rowIndex].setScore(gameMetrics.GetScore());
				raws[rowIndex].setStars(gameMetrics.GetStars());
				raws[rowIndex].getViewDetailsBtn().enabled = true;

			} else{
//				raws[rowIndex].setScore(0);
				raws[rowIndex].setStars(0);
				raws[rowIndex].getViewDetailsBtn().enabled = false;

			}          
		}

		public void OnClickViewDetails(int index){
			Debug.Log(index +","+ currentRawsToViewDetails.Count);
			PlaySoundClick();
			ViewDetails(currentRawsToViewDetails[index]);
		}

		private void ViewDetails(int v){
			Debug.Log("View details of " + v);
//			resultsView.SetActive(false);
			detailsView.SetActive(true);
//			string activity = SettingsController.GetController().GetLanguage() = AppController.GetController().GetAppModel().GameTitles[v];
//			((DetailsView)detailsView.GetComponent<DetailsView>()).ShowDetailsOf(activity, SettingsController.GetController().GetUsername(), MetricsController.GetController().GetMetricsByLevel(v));
		}

		private void updateArrows(){
			nextPageBtn.gameObject.SetActive((MetricsController.GetController().metricsModel.GetLevelIndexes(currentPage + 1, MAX_ROWS).Count > 0));
			prevPageBtn.gameObject.SetActive((MetricsController.GetController().metricsModel.GetLevelIndexes(currentPage - 1, MAX_ROWS).Count > 0));

		}
	}
}