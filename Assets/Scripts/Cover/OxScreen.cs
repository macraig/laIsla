using Assets.Scripts.Settings;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Cover
{

    public class OxScreen : MonoBehaviour
    {
        public CoverView coverView;

        public Text description;
        public Text moreInformationLabel;
        public Text contactLabel;
        
       
        public void OnClickOxLink()
        {
            coverView.ClickSound();
            Application.OpenURL("http://oxed.com.ar");
        }

        public void OnClickOxScreen()
        {
            coverView.ClickSound();
            coverView.ShowCoverScreen();
            gameObject.SetActive(false);
        }
               
    }
}