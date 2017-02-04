using UnityEngine;

namespace Assets.Scripts.Metrics
{
    public class Timer
    {
        private static Timer timer;

        private float lapsedSecods;
        private float initTime;   
        
        public Timer() { }    

        public void InitTimer()
        {
            lapsedSecods = 0;
            initTime = Time.time;
        }

        public void Pause(){
            lapsedSecods += Time.time - initTime;
        }

        public void Resume()
        {
            initTime = Time.time;
        }

        public void FinishTimer() {
            lapsedSecods += Time.time - initTime;
        }

        public int GetLapsedSeconds(){
            SOUT();
            return (int)lapsedSecods;
        }


        private void SOUT(){
            Debug.Log("Current time: " + lapsedSecods);
        }

        public static Timer GetTimer(){
            if (timer == null) timer = new Timer();
            return timer;
        }
        
    }
}
