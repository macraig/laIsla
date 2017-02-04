using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Settings;
using Assets.Scripts.Metrics;
//using ProgressBar;
using Assets.Scripts.App;

namespace Assets.Scripts.LevelCompleted
{

    public class LevelCompletedView : MonoBehaviour
    {

        public Text title;
        public Text subTitle;
        public Text score;
        public List<Image> stars;
        public Text correct;
        public Text incorrect;
        public Text RangeText;
        public Text BonusTimeText;
//        public ProgressBarBehaviour progressBar;

        private int scorePercentage;



        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickMainMenu();
            }
        }

        void OnEnable()
        {
            UpdateTexts();
            SetValues(AppController.GetController().GetCurrentMetrics());
        }      

        private void UpdateTexts()
        {
            switch (SettingsController.GetController().GetLanguage())
            {
                case 0:
                    title.text = "PUNTAJE";
                    subTitle.text = "Porcentaje de puntos obtenidos";
                    break;
                case 1:
                    title.text = "SCORE";
                    subTitle.text = "Percentage of obtained points";
                    break;
            }
        }

        public void SetValues(GameMetrics gameMetrics)
        {
            for (int i = 0; i < 3; i++)
            {
                stars[i].gameObject.SetActive(i < gameMetrics.GetStars());
            }
            score.text = "" + gameMetrics.GetScore();
//            scorePercentage = (int)(((gameMetrics.GetScore() + 0f) / (MetricsController.GetController().GetMaxScore() + 0f)) * 100);
            correct.text = "" + gameMetrics.GetRightAnswers();
            incorrect.text = "" + gameMetrics.GetWrongAnswers();
            Invoke("IncrementProgressBar", 0.1f);
            RangeText.text = gameMetrics.GetRange().ToString();
            BonusTimeText.text = "" + gameMetrics.GetBonusTime();
        }

        void IncrementProgressBar()
        {
//            progressBar.IncrementValue(scorePercentage);

        }

        public void OnClickRetry()
        {
            PlayClickSound();
            LevelCompletedController.GetController().RetryLvl();
        }

        public void OnClickMainMenu()
        {
            PlayClickSound();
            LevelCompletedController.GetController().MainMenu();
        }

        private void PlayClickSound()
        {
            LevelCompletedController.GetController().PlayClikSound();
        }
    }
}
