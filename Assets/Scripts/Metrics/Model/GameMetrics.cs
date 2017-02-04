using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.Scripts.Metrics.Model;

namespace Assets.Scripts.Metrics
{

    [Serializable]
    public class GameMetrics{
        private const int VERSION = 1;
        private int lapsedSeconds, rightAnswers, wrongAnswers, stars, score;   
        private string date;
		private int gameId;

        [NonSerialized]
        private int _realCorrectExercises;

       
        [OptionalField(VersionAdded = 2)]
        private List<bool> _answerBools;


        [OptionalField(VersionAdded = 2)] private int _bonusTime;

        [OptionalField(VersionAdded = 2)] private Range _range; 

		public GameMetrics(int gameId)
        {
			this.gameId = gameId;
            stars = 0;
            lapsedSeconds = 0;
            rightAnswers = 0;
            wrongAnswers = 0;
            score = 0;
            date = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
          
         
            _answerBools = new List<bool>();
        }

        internal void Reset(){
            stars = 0;
            lapsedSeconds = 0;
            rightAnswers = 0;
            wrongAnswers = 0;
            score = 0;
        }    

        internal int GetStars(){
            return stars;
        }

        internal void AddWrongAnswer()
        {
            _answerBools.Add(false);
            this.wrongAnswers++;
        }

        internal void SetStars(int stars)
        {
            this.stars = stars;
        }

        internal void AddRightAnswer()
        {
            _answerBools.Add(true);
            this.rightAnswers++;
        }     

        internal int GetScore()
        {
            return score;
        }

		public int GetGameId()
		{
			return gameId;
		}

	

        internal void SetLapsedSeconds(int lapsedSeconds)
        {
            this.lapsedSeconds = lapsedSeconds;
        }

        internal int GetWrongAnswers()
        {
            return wrongAnswers;
        }

        internal void SetScore(int score)
        {
            this.score = score;
        }

        internal int GetRightAnswers()
        {
            return rightAnswers;
        }

        internal string GetDate()
        {
            return date;
        }

        internal void SetDate(string date)
        {
            this.date = date;
        }

        internal int GetLapsedSeconds()
        {
            return lapsedSeconds;
        }

        internal void SetRightAnswers(int rightAnswers)
        {
            this.rightAnswers = rightAnswers;
        }

        internal void SetWrongAnswers(int wrongAnswers)
        {
            this.wrongAnswers = wrongAnswers;
        }

     

       

        public void SetBonusTime(int bonusTime)
        {
            _bonusTime = bonusTime;
        }

        public void SetRange(Range range)
        {
            _range = range;
        }

        public Range GetRange()
        {
            return _range;
        }

        public int GetBonusTime()
        {
            return _bonusTime;
        }
    }
}