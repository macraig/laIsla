using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.LevelCompleted;
using Assets.Scripts.Settings;
using Assets.Scripts.Sound;
using Assets.Scripts.Games;
using Assets.Scripts.Metrics;
using Assets.Scripts.App;
using UnityEngine.UI;

namespace Assets.Scripts.App
{
    public class ViewController : MonoBehaviour
    {
        private static ViewController viewController;

        public GameObject viewPanel;
 
        private GameObject currentGameObject;

        private GameObject inGameMenuScreen;
        private GameObject instructionsScreen;

        void Awake()
        {
            if (viewController == null) viewController = this;
            else if (viewController != this) Destroy(gameObject);      
            DontDestroyOnLoad(this);
        }  

		void Start(){
            LoadCover();
			SoundController.GetController ().PlayMusic ();
		}

        internal void LoadMainMenu()
        {
            ChangeCurrentObject(LoadPrefab("MainMenu"));
        }    

       

        internal void LoadCover()
        {
            ChangeCurrentObject(LoadPrefab("Cover"));
        }

		internal void LoadEndPanel()
		{
			ChangeCurrentObject(LoadPrefab("finalResult"));
		}

     
        internal void LoadMetrics()
        {
           // Destroy(currentGameObject);
           // currentGameObject = Instantiate();
            ChangeCurrentObject(LoadPrefab("Metrics"));
//            SetCanvasScaler(1);

        }

		internal void RestartCurrentGame()
		{
//			ChangeCurrentObject(LoadPrefab("oading");
			ChangeCurrentObject(LoadPrefab("Games/" + AppController.GetController().GetCurrentGame().GetPrefabName()));

		}

		private GameObject LoadPrefab(string name)
		{
			return Resources.Load<GameObject>("Prefabs/" + name);
		}

        private void ChangeCurrentObject(GameObject newObject)
        {
//          SetCanvasScaler(0.5f);
            GameObject child = Instantiate(newObject);
            FitObjectTo(child, viewPanel);
			Destroy(currentGameObject);
			currentGameObject = child;            
        }

        internal void ShowInGameMenu()
        {
            if(inGameMenuScreen == null)
            {
                HideInstructions();
                inGameMenuScreen = Instantiate(LoadPrefab("IngameMenu"));
                FitObjectTo(inGameMenuScreen, viewPanel);
            }
                           
        }

        internal void FitObjectTo(GameObject child, GameObject parent)
        {
            child.transform.SetParent(parent.transform, true);
            child.transform.localPosition = Vector3.zero;
            child.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            child.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            child.transform.localScale = Vector3.one;
        }

       

        internal void LoadSettings()
        {
            ChangeCurrentObject(LoadPrefab("Settings"));
        }

        internal void LoadLogin()
        {
			ChangeCurrentObject(LoadPrefab("NameScreen"));
        }    

        internal void StartGame(Game game)
        {
			ChangeCurrentObject(LoadPrefab("Games/" + game.GetPrefabName()));
//            SetCanvasScalerToCurrentGame();
        }

        internal void ShowInstructions()
        {
            instructionsScreen = Instantiate(LoadPrefab("Instructions"));
            FitObjectTo(instructionsScreen, viewPanel);
        }

        internal void HideInGameMenu(){
            Destroy(inGameMenuScreen);
        }

        internal void HideInstructions()
        {
            Destroy(instructionsScreen);
        }

//        internal void LoadLevelCompleted()
//        {
//            ChangeCurrentObject(LoadPrefab("LevelCompleted"));
//        }


        public static ViewController GetController()
        {
            return viewController;
        }

        public void SetCanvasScaler(float scale)
        {
			transform.parent.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = scale;
        }

        public void SetCanvasScalerToCurrentGame()
        {
              SetCanvasScaler(1);
        }

        public bool IsInGameMenuShowed()
        {
            return inGameMenuScreen != null && inGameMenuScreen.activeSelf;
        }
    }
}
