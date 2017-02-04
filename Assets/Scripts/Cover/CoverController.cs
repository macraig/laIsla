using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.App;

namespace Assets.Scripts.Cover{

    public class CoverController : MonoBehaviour{
        private static CoverController coverController;

        void Awake(){
            if (coverController == null){ coverController = this; }            
            else if (coverController != this){ Destroy(gameObject); }               
        }


        internal void StartGame(){
            ViewController.GetController().LoadLogin();
        }

        internal static CoverController GetController(){
            return coverController;
        }        
    }
}
