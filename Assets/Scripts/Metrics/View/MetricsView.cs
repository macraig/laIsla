using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Scripts.Settings;
using Assets.Scripts.App;
using Assets.Scripts.Metrics.Model;

namespace Assets.Scripts.Metrics.View
{

    public class MetricsView : MonoBehaviour
    {

        private static MetricsView metricsView;

        public DetailsView details;
//        public ResultsView results;
        [SerializeField]
        private List<Sprite> areaIcons;

        void Awake()
        {
            if (metricsView == null) metricsView = this;
            else if (metricsView != this) Destroy(this);
        }

        void Start()
        {
			//UNCOMMENT
//            ShowResults();
            HideDetails();
        }
		//UNCOMMENT
//        void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.Escape))
//            {
//                if (details.isActiveAndEnabled) details.OnClickCrossBtn();            
//                else results.OnClickCrossBtn();                          
//            }
//        }

        private void HideDetails()
        {
            details.gameObject.SetActive(false);
        }               

		internal void ShowDetailsOf(int idGame)
        {
            details.gameObject.SetActive(true);
            Debug.Log("id: " + idGame);
			details.ShowDetailsOf(AppController.GetController().GetGameName(idGame),SettingsController.GetController().GetUsername(), MetricsController.GetController().GetGameMetrics(idGame));
        }

		//UNCOMMENT
//        internal void ShowResults()
//        {
//            results.gameObject.SetActive(true);
//        }

        public static MetricsView GetMetricsView()
        {
            return metricsView;
        }

        internal Sprite GetAreaIcon(int area)
        {
            return areaIcons[area];
        }
    }
}