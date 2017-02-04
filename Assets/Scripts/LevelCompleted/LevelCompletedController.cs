using UnityEngine;
using Assets.Scripts.Sound;
using Assets.Scripts.App;

namespace Assets.Scripts.LevelCompleted
{
    public class LevelCompletedController : MonoBehaviour
    {
        private static LevelCompletedController levelCompletedController;

        void Awake()
        {
            if (levelCompletedController == null) levelCompletedController = this;
            else if (levelCompletedController != this) Destroy(gameObject);
            SoundController.GetController().PlayLevelCompleteSound();
        }     

        internal void RetryLvl()
        {
            AppController.GetController().PlayCurrentGame();
        }

        internal void MainMenu()
        {
            ViewController.GetController().LoadMainMenu();
        }

        internal void PlayClikSound()
        {
            SoundController.GetController().PlayClickSound();
        }

        internal static LevelCompletedController GetController()
        {
            return levelCompletedController;
        }
    }
}
