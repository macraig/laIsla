
using Assets.Scripts.App;
using Assets.Scripts.Metrics;
using Assets.Scripts.Metrics.Model;
using Assets.Scripts.Common;

namespace Assets.Scripts.Games
{
    public abstract class LevelModel
    {
        // These three instance variables are used to the metrics and have to be
        // initialized in each child class
        // deprecated
        protected int minSeconds;
        protected int pointsPerSecond;
        protected int pointsPerError;


        protected int currentSubLevel;
        protected MetricsTable MetricsTable;

   
	



		/*
            You must call this method when the user realized an answer
            param isCorrect indicates if the answers is correct or no
        */
		public void LogAnswer(bool isCorrect){
			if (isCorrect)
			{
				MetricsController.GetController().AddRightAnswer();
			}
			else { MetricsController.GetController().AddWrongAnswer(); }       
		}



		/*
   You must call this method at the end of the game
   params minSeconds, pointsPerSecond and pointsPerError are defined in LevelModel
    */
		public void EndGame(int minSeconds, int pointsPerSecond, int pointsPerError){
			MetricsController.GetController().GameFinished(minSeconds, pointsPerSecond, pointsPerError);
//			ViewController.GetController().LoadLevelCompleted();
		}




		/*
            You must call this method to know how many right answers the player did
            and decide if the game is ended or will continue
        */
		public GameMetrics GetCurrentMetrics(){
			return MetricsController.GetController().GetCurrentMetrics();
		}


        public LevelModel() { }

        public int GetPointsPerError(){
             return pointsPerError;
        }

        public int GetPointsPerSecond(){
            return pointsPerSecond;
        }

        public int GetMinSeconds()
        {
            return minSeconds;
        }

        public int GetCurrentSubLevel()
        {
            return currentSubLevel;
        }

        public void SetCurrentSubLevel(int sublevel)
        {          
            currentSubLevel = sublevel;
        }

        public MetricsTable GetMetricsTable()
        {
            return MetricsTable;
        }

    }
}
