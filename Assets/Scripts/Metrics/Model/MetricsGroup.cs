using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Metrics
{


    [Serializable]
    public class MetricsGroup {

        private int gameId;
        private List<List<GameMetrics>> gameMetrics;

        public MetricsGroup(int gameId)
        {
            this.gameId = gameId;
            gameMetrics = new List<List<GameMetrics>>(3);
            for (int i = 0; i < gameMetrics.Capacity; i++)
            {
                gameMetrics.Add(new List<GameMetrics>());
            }
        }


        public int GetGameId()
        {
            return gameId;
        }

        public List<List<GameMetrics>> GetMetrics()
        {
            return gameMetrics;
        }
    }
}
