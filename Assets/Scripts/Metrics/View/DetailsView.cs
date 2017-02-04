using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.App;
using System;
using Assets.Scripts.Sound;
using Assets.Scripts.Settings;

namespace Assets.Scripts.Metrics.View
{
    public class DetailsView : MonoBehaviour
    {
        public int MAX_COLUMNS = 10;

        public Text activity;
        public Text username;
        public Text date;
//        public Text score;
//        public Text time;
//        public Text correctQuantity;
//        public Text incorrectQuantity;
//        public Text note;
        public List<Toggle> points;
        [SerializeField]
        private List<Image> levelImages;
        private List<GameMetrics> metricsPoints;
        private float MAX_Y;
        private float MIN_Y;         


        void Awake()
        {
            MIN_Y = points[0].transform.position.y;
            MAX_Y = points[1].transform.position.y;    
        }

        internal void ShowDetailsOf(string activity, string username, List<GameMetrics> metrics)
        {
            if (metricsPoints == null) metricsPoints = new List<GameMetrics>();
            else metricsPoints.Clear();
             
            this.activity.text = activity;
            this.username.text = username.ToUpper();
//            groupGames(metrics);
            makeChart();
//            SetLevel(metricsPoints[0].GetLevel());
            //joinPoints(points, metricsPoints.Count);
        }

      
//
//        private void joinPoints(List<Image> points, int count)
//        {
//            for(int i = 0; i < count && i - 1 < points.Count - 1 ; i++)
//            {
//                points[i].GetComponent<LineRenderer>().SetVertexCount(2);
//                Vector3 pos = points[i].transform.position;
//                pos.z = -10;
//
//                Vector3 pos1 = points[i+1].transform.position;
//                pos1.z = -10;
//
//
//                points[i].GetComponent<LineRenderer>().SetPosition(0, pos);
//                points[i].GetComponent<LineRenderer>().SetPosition(1, pos1);
//            }
//        }    

        private void makeChart()
        {         
            for (int i = 0; i < metricsPoints.Count; i++){
                Vector3 pos = points[i].transform.position;
				pos.y = calculateY(metricsPoints[i].GetStars(), MAX_Y, MIN_Y);
                points[i].transform.position = pos;
                points[i].gameObject.SetActive(true);
            }

            for (int i = metricsPoints.Count; i < MAX_COLUMNS; i++){
                points[i].gameObject.SetActive(false);
            }

            points[metricsPoints.Count - 1].isOn = false;
            points[metricsPoints.Count - 1].isOn = true;



        }

        private float calculateY(int stars, float MAX_Y, float MIN_Y)
        {
            float SCORE_MAX = 5;
            float SCORE_MIN = 0;
			float t = MIN_Y + (((stars - SCORE_MIN) * (MAX_Y - MIN_Y)) / (SCORE_MAX - SCORE_MIN));
            return t;
        }               

//        private void groupGames(List<GameMetrics> metrics){
//            int gruopSize = (int)Math.Ceiling((metrics.Count + 0.0f) / (MAX_COLUMNS + 0.0f));
//            this.note.text = gruopSize + (SettingsController.GetController().GetLanguage() == 0 ? " en " : " in ") + "1";
//
//            for(int i = 0; i < Math.Ceiling((metrics.Count + 0.0f) / (gruopSize + 0.0f)); i++)
//            {
//                List<GameMetrics> currentGroup = metrics.GetRange(i * gruopSize, (i * gruopSize + gruopSize) <= metrics.Count ? gruopSize : metrics.Count - metricsPoints.Count * gruopSize);
//                GameMetrics currentMetric = new GameMetrics(currentGroup[0].GetIndex());
//                currentMetric.SetDate(currentGroup[0].GetDate());
//                for(int j = 0; j < currentGroup.Count; j++)
//                {
//                    currentMetric.SetScore(currentMetric.GetScore() + currentGroup[j].GetScore());
//                    currentMetric.SetLapsedSeconds(currentMetric.GetLapsedSeconds() + currentGroup[j].GetLapsedSeconds());
//                    currentMetric.SetRightAnswers(currentMetric.GetRightAnswers() + currentGroup[j].GetRightAnswers());
//                    currentMetric.SetWrongAnswers(currentMetric.GetWrongAnswers() + currentGroup[j].GetWrongAnswers());
//                }
//                currentMetric.SetScore(Average(currentMetric.GetScore(), currentGroup.Count));
//                currentMetric.SetLapsedSeconds(Average(currentMetric.GetLapsedSeconds(), currentGroup.Count));
//                currentMetric.SetRightAnswers(Average(currentMetric.GetRightAnswers(), currentGroup.Count));
//                currentMetric.SetWrongAnswers(Average(currentMetric.GetWrongAnswers(), currentGroup.Count));
//
//                metricsPoints.Add(currentMetric);
//            }
//        }

        private int Average(int number, int count)
        {
            return (int)Math.Round((number + 0f) / (count + 0f));
        }

        public void ShowInfoOf(int pointNumber)
        {          
            GameMetrics currentMetric = metricsPoints[pointNumber];
            date.text = currentMetric.GetDate();
//            score.text = "" +currentMetric.GetScore();
//            time.text = "" + currentMetric.GetLapsedSeconds() + " s";
//            correctQuantity.text = "" + currentMetric.GetRightAnswers();
//            incorrectQuantity.text = "" + currentMetric.GetWrongAnswers();
        }

        public void OnClickCrossBtn()
        {
            PlaySoundClick();
//            MetricsView.GetMetricsView().ShowResults();
            gameObject.SetActive(false);
        }

        private void PlaySoundClick()
        {
            SoundController.GetController().PlayClickSound();
        }
    }
}