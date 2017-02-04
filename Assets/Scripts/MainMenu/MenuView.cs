using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Settings;
using Assets.Scripts.Sound;
using Assets.Scripts.App;
using Assets.Scripts.Common;

namespace Assets.Scripts.MainMenu {
    public class MenuView : MonoBehaviour {


		public GameObject metricsPanel, detailsPanel;
      

        private List<GameButton> currentGames;


        void Start() {
            
            currentGames = new List<GameButton>();
           
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Utils.MinimizeApp();
            }
        }

       

        public void OnClickGame(int game)
        {
            
            //ClickSound();
            //MainMenuController.GetController().ShowPreviewGame(0, game);
            //gameObject.SetActive(false);
            /* 
             GameObject child = Instantiate(buttonGamePrefab);
             child.GetComponent<GameButton>().SetAttributes(area, game, "to do", MainMenuController.GetController().GetAreaSprite(area));
             child.transform.SetParent(searchResultsPanel.transform, true);
             child.GetComponent<RectTransform>().offsetMax = Vector2.zero;
             child.GetComponent<RectTransform>().offsetMin = Vector2.zero;
             child.transform.localScale = Vector3.one;
             child.name = "Game " + (game + 1);
             Button b = child.GetComponent<Button>();
             int captured = game+1;
             b.onClick.AddListener(() => OnClickGame(captured));
     */
        }

		public void OnClickBack(){
			ClickSound();
			MainMenuController.GetController().GoBack();
		}

        public void OnClickSettings()
        {
            ClickSound();
            MainMenuController.GetController().ShowSettings();
        }

        public void OnClickMetrics()
        {
            ClickSound();
           ShowMetrics();
        }

		void ShowMetrics ()
		{
			metricsPanel.SetActive (true);
		}

		void HideMetrics(){
			metricsPanel.SetActive (false);
		}

        public void ClickSound()
        {
            SoundController.GetController().PlayClickSound();
        }

        

    }
}
