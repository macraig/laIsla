using UnityEngine;
using Assets.Scripts.Settings;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using Assets.Scripts.App;
using Assets.Scripts.Metrics.Model;
using Assets.Scripts.Games;

namespace Assets.Scripts.Metrics.Model
{
    public class MetricsController : MonoBehaviour
    {
        private static MetricsController metricsController;
        public MetricsModel metricsModel;
        // this list is to register the answers of the currentSublevel. It's is restarted
        // when a sublevel changes 
        private List<List<bool>> actualBuffer;

        void Awake()
        {
            if (metricsController == null) metricsController = this;
            else if (metricsController != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
			
            // list to first exercise is added
            actualBuffer = new List<List<bool>> {new List<bool>()};
        }

		public void SetMetricsModel(MetricsModel metricsModel){
			this.metricsModel = metricsModel;
		}

		internal GameMetrics GetLastMetricOf(Game game)
        {
			List<GameMetrics> currentMetrics = metricsModel.SearchMetricsByGame(game.GetId());
			return currentMetrics[currentMetrics.Count-1];
        }

		public List<GameMetrics> GetGameMetrics (int idGame)
		{
			return metricsModel.SearchMetricsByGame (idGame);
		}

		internal GameMetrics GetMetricByIndex(int gameId, int metricIndex)
		{
			List<GameMetrics> currentMetrics = metricsModel.SearchMetricsByGame(gameId);
			return currentMetrics[metricIndex];
		}

		internal List<List<GameMetrics>> GetMetrics()
        {
            return metricsModel.GetMetrics();
        }

        internal GameMetrics GetCurrentMetrics()
        {
            return metricsModel.GetCurrentMetrics();
        }



        public void GameStart()
        {
            metricsModel.GameStarted();
            RestartActualBuffer();
            Timer.GetTimer().InitTimer();
        }

        private void RestartActualBuffer()
        {
            actualBuffer.Clear();
            actualBuffer.Add(new List<bool>());
        }

        public void GameFinished(int minSeconds, int pointsPerSecond, int pointsPerError)
        {
            Timer.GetTimer().FinishTimer();
            metricsModel.GameFinished(Timer.GetTimer().GetLapsedSeconds(), minSeconds, pointsPerSecond, pointsPerError);
            saveToDisk();
        }

        

        internal GameMetrics GetBestMetric(int game)
        {
            return metricsModel.GetBestMetric(game);
        }


        private void saveToDisk()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + SettingsController.GetController().GetUsername() + ".dat");
            bf.Serialize(file, metricsModel);
            file.Close();
        }

        public void LoadFromDisk()
        {
            if (File.Exists(Application.persistentDataPath + "/" + SettingsController.GetController().GetUsername() + ".dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + SettingsController.GetController().GetUsername() + ".dat", FileMode.Open);
                metricsModel = (MetricsModel)bf.Deserialize(file);
                file.Close();

				/*Could be implemented to wipe data from deprecated games*/
//                metricsModel.UpdateGames(AppController.GetController().GetGames());
//            } else
//            {
//                metricsModel = new MetricsModel(AppController.GetController().GetGames());
//				AppController.GetController ().GetMetricsController ().SetMetricsModel (metricsModel);
            }
        }

        internal static MetricsController GetController()
        {
            return metricsController;
        }

        public void AddRightAnswer()
        {
            GetCurrentMetrics().AddRightAnswer();
            actualBuffer[actualBuffer.Count - 1].Add(true);
			           
            // always the next list is initialized
            actualBuffer.Add(new List<bool>());
        }

        public void AddWrongAnswer()
        {
            GetCurrentMetrics().AddWrongAnswer();
            actualBuffer[actualBuffer.Count - 1].Add(false);

         
        
        }

//        private bool CheckSurrenderPossibility()
//        {
//            return actualBuffer[actualBuffer.Count - 1].Count == 2;
//
//        }

       

        public void OnSurrender()
        {
            actualBuffer.Add(new List<bool>());
        }
    }
}