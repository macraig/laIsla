using System;
using Assets.Scripts.Games;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using I18N;
using System.Collections.Generic;

namespace Assets.Scripts.Common
{
    public static class Utils {

        public static void UpdateTryButton(Toggle[] toggles, Button tryButton1, Button tryButton2 = null)
        {
            bool actualStatus = false;
            foreach (Toggle toggle in toggles)
            {
                actualStatus = toggle.isActiveAndEnabled && toggle.isOn;
                if (actualStatus) break;
            }

            UpdateTryButton(tryButton1, actualStatus);
            if(tryButton2) UpdateTryButton(tryButton2, actualStatus);

            
        }

        private static void UpdateTryButton(Button tryButton, bool actualStatus)
        {
            bool prevStatus = tryButton.enabled;
            if (prevStatus != actualStatus)
            {
                tryButton.enabled = actualStatus;
            }
        }

        public static void MinimizeApp()
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }

        public static string GetNumberText(double number)
        {
            var decimalSplit = number.ToString(CultureInfo.CreateSpecificCulture("en-US")).Split('.');

            var decimals = decimalSplit.Length > 1 ? TrimLastZeroes(decimalSplit[1]).Length : 0;
            return number.ToString("N" + decimals, CultureInfo.CreateSpecificCulture(I18n.GetLocale()));
        }
        public static string TrimLastZeroes(string decimalPart)
        {
            if (decimalPart.EndsWith("0")) return TrimLastZeroes(decimalPart.Substring(0, decimalPart.Length - 1));
            return decimalPart;
        }

        public static bool IsPrime(int number)
        {

            if (number == 1) return false;
            if (number == 2) return true;

            if (number % 2 == 0) return false; //Even number     

            for (int i = 3; i < number; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;

        }

        
        public static List<T> Shuffle<T>(List<T> listToShuffle)
        {
            List<int> messyIndexesList = new List<int>();
            T[] messyArray = new T[listToShuffle.Count];
            int randomIndex;

            for (int i = 0; i < listToShuffle.Count; i++)
            {
                messyIndexesList.Add(i);
            }

            for (int i = 0; i < listToShuffle.Count; i++)
            {
                randomIndex = messyIndexesList[UnityEngine.Random.Range(0, messyIndexesList.Count)];
                messyArray[randomIndex] = listToShuffle[i];
                messyIndexesList.Remove(randomIndex);
            }
            messyIndexesList.Clear();
            for (int i = 0; i < listToShuffle.Count; i++)
            {
                listToShuffle[i] = messyArray[i];
            }
            return listToShuffle;
        }

    }
}
