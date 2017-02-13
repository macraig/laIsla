using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Sound;

namespace Assets.Scripts.Games.Recorridos
{
    public class RecorridosController : MonoBehaviour
    {
		public const int START_TIME = 99, CORRECT_SCENE_TIME = 20,GAMES_BEFORE_TIME = 5;
		private int timer, currentStartTime;

		public float timeBetweenActions;

        public static RecorridosController instance;
		private RecorridosBoard board;
		private List<List<int>> boardMatrix;
	
		public enum RecorridosTileEnum { Path, Wall, Bomb, Hole, Fire, Nut, Start, End }

        public GameObject boardPosition;
        public GameObject player;

        public Sprite pathSprite;

        public Sprite holeSprite;
        public Sprite wallSprite;
        public Sprite startSprite;
        public Sprite endSprite;
        public Sprite nutSprite;
        public Sprite bombSprite;
        public Sprite fireSprite;

        private RecorridosTile[][] gridSpace;

        private List<RecorridosAction> actionsToDo;

        private RecorridosView view;
        private Vector2 puppetGridPosition;

        private int currentValueAnalyzed;

        public List<Button> buttons;

        private Dictionary<RecorridosTileEnum, Sprite> tileDictionary;
        private List<RecorridosTileEnum> keys;

        private Vector2 initialPuppetGridPosition;
        private List<RecorridosTile> pathTiles;
        private int nutCount;
		private bool first = true;
		private int currentLevel, gameCounter;
		private List<RecorridosLevel> lvls;
		private bool withTime;
       
		private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }else
            {
                Destroy(gameObject);
            }
        }


        // Use this for initialization
        void Start()
        {
			board = new RecorridosBoard ();
			lvls = board.GetLevels ();
			keys = new List<RecorridosTileEnum>();

			/*
         Tile frames
         1:white
         2:black
         3:wall
         4:bomb
         5:hole
         6:fire
         7:nut
         8:start
         9:finish
         * */
			keys.Add(RecorridosTileEnum.Path);
			keys.Add(RecorridosTileEnum.Path);
            keys.Add(RecorridosTileEnum.Path);
			keys.Add(RecorridosTileEnum.Wall);
            keys.Add(RecorridosTileEnum.Bomb);
			keys.Add(RecorridosTileEnum.Hole);
			keys.Add(RecorridosTileEnum.Fire);
            keys.Add(RecorridosTileEnum.Nut);
			keys.Add(RecorridosTileEnum.Start);
			keys.Add(RecorridosTileEnum.End);


            tileDictionary = new Dictionary<RecorridosTileEnum, Sprite>();

            tileDictionary.Add(RecorridosTileEnum.Path, pathSprite);
            tileDictionary.Add(RecorridosTileEnum.Hole, holeSprite);
            tileDictionary.Add(RecorridosTileEnum.Bomb, bombSprite);
            tileDictionary.Add(RecorridosTileEnum.Wall, wallSprite);
            tileDictionary.Add(RecorridosTileEnum.Nut, nutSprite);
            tileDictionary.Add(RecorridosTileEnum.Fire, fireSprite);
			tileDictionary.Add(RecorridosTileEnum.Start, startSprite);
			tileDictionary.Add(RecorridosTileEnum.End, endSprite);



			gridSpace = new RecorridosTile[board.Rows()][];
            view = GetComponent<RecorridosView>();
            view.SetPlayerImage(player.GetComponent<Image>());
          
			ShowExplanation ();
        }

		void ShowExplanation(){
			view.ShowExplanation ();
		}

		public void HideExplanation(){
			view.HideExplanation ();
			if (first) {
				first = false;
				withTime = false;
				currentLevel = 0;
				timer = START_TIME;
				currentStartTime = START_TIME;
				gameCounter = 0;
				ResetGame();
			}

		}

        public void FallInHole()
        {
			//Go to random tile
//            int randonNewPath = Random.Range(0, pathTiles.Count);
//            puppetGridPosition.x = pathTiles[randonNewPath].GridPositionX;
//            puppetGridPosition.y = pathTiles[randonNewPath].GridPositionY;
//            StartCoroutine(GoRolling(2, gridSpace[(int)puppetGridPosition.x][(int)puppetGridPosition.y]));
			BackToStart();
			view.PlayFallSound ();
        }

		public void GetBurnt(){
			BackToStart();
			view.PlayFireSound ();
		}

        public void BackToStart()
        {
            puppetGridPosition.x = (int)initialPuppetGridPosition.x;
            puppetGridPosition.y = (int)initialPuppetGridPosition.y;
            StartCoroutine(GoRolling(2, gridSpace[(int)puppetGridPosition.x][(int)puppetGridPosition.y]));
        }

        public void PickNut(int gridPositionX, int gridPositionY)
        {
            gridSpace[gridPositionX][gridPositionY].Type = RecorridosTileEnum.Path;
            gridSpace[gridPositionX][gridPositionY].Sprite = pathSprite;
            nutCount++;
			view.PlayNutSound ();
            view.SetNutTextCounter(nutCount);
        }

		public void Explode()
		{

			view.ShowBombAnimation ();
		}

		public void OnBombAnimationEnd(){
			
			 GameOver(false);
		}

		public void RestartGame(){
			view.HideInGameMenu ();
			first = true;
			view.RestartGame ();


			Start ();
		}


		public void ResetGame()
        {

			view.ShowPlayer ();
			nutCount = 0;
            int rowCounter = 0;
			int cols = board.Cols ();
            pathTiles = new List<RecorridosTile>();
			gridSpace[rowCounter] = new RecorridosTile[cols];

			boardMatrix = board.GenerateBoard (lvls [currentLevel]);


			for (int i = 0; i < boardPosition.transform.childCount; i++) {
				if (i % cols == 0 && i!=0)
                {
                    rowCounter++;
					gridSpace[rowCounter] = new RecorridosTile[cols];
                }
				int key = boardMatrix [rowCounter] [i % cols];
                    RecorridosTileEnum tile = keys[key];

                    Sprite spriteToPut;
					tileDictionary.TryGetValue(tile, out spriteToPut);

				gridSpace[rowCounter][i % cols] = new RecorridosTile(tile,
					boardPosition.transform.GetChild(i).transform.position, spriteToPut, boardPosition.transform.GetChild(i).GetComponent<Image>(),rowCounter,i%cols);
                
            }

			puppetGridPosition = board.GetStartPosition();
			initialPuppetGridPosition = board.GetStartPosition();
            EnableButtonState(true);
            view.ResetGame();
            player.transform.position = gridSpace[(int)initialPuppetGridPosition.x][(int)initialPuppetGridPosition.y].Position;
            actionsToDo = new List<RecorridosAction>();

        }


        public void AddAction(RecorridosAction recorridosButton)
        {
            switch (recorridosButton.currentAction)
            {
                case RecorridosAction.ActionToDo.Start:
                    currentValueAnalyzed = 0;
                    EnableButtonState(false);

                    MovePuppet();
                    break;
                case RecorridosAction.ActionToDo.Remove:
                    actionsToDo.RemoveAt(recorridosButton.indexInList);
                    view.RemoveInstruction(recorridosButton.indexInList);
                    break;
                default:
                    if (actionsToDo.Count < 5)
                    {
                        actionsToDo.Add(recorridosButton);
                        view.AddInstruction(recorridosButton);
                    }
                break;
            }



        }

        public void MovePuppet()
        {
            if(actionsToDo.Count > 0)
            {
                
                RecorridosAction actionToDo = actionsToDo[0];
                switch (actionToDo.currentAction)
                {
                    case RecorridosAction.ActionToDo.Up:
                        EvaluateChange(-1, 0);
                        break;
                    case RecorridosAction.ActionToDo.Down:
                        EvaluateChange(1, 0);
                        break;
                    case RecorridosAction.ActionToDo.Left:
                        EvaluateChange(0, -1);
                        break;
                    case RecorridosAction.ActionToDo.Right:
                        EvaluateChange(0, 1);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                EnableButtonState(true);
                view.ResetStackView();
            }
            
        }

        public void EvaluateChange(int xChange, int yChange)
        {
            Vector2 positionToAnalyze = puppetGridPosition;
            positionToAnalyze.x = positionToAnalyze.x + xChange;
            positionToAnalyze.y = positionToAnalyze.y + yChange;
			if (positionToAnalyze.x < board.Cols()  && positionToAnalyze.x > -1 && 
				positionToAnalyze.y < board.Rows() && positionToAnalyze.y > -1 &&
                gridSpace[(int)positionToAnalyze.x][(int)positionToAnalyze.y].Type != RecorridosTileEnum.Wall)
            {
                view.LightValue(currentValueAnalyzed, true, Color.yellow);
                puppetGridPosition.x = (int)positionToAnalyze.x;
                puppetGridPosition.y = (int)positionToAnalyze.y;
				view.EnableComponents (false);
				StartCoroutine(DoMove(timeBetweenActions, gridSpace[(int)positionToAnalyze.x][(int)positionToAnalyze.y]));
				SoundController.GetController ().PlayDropSound ();
                if(xChange == 1)
                {
                    view.MovingDown();
                }else if(xChange == -1)
                {
                    view.MovingUp();
                }
                else if(yChange == 1)
                {
                    view.MovingRight();
                }
                else if(yChange == -1)
                {
                    view.MovingLeft();
                }
            }
            else
            {
                view.LightValue(currentValueAnalyzed, true, Color.red);
                StartCoroutine(ResetList(2, true));
            }
        }

        IEnumerator ResetList(float timeToReset, bool buttonState)
        {
            yield return new WaitForSeconds(timeToReset);
            actionsToDo = new List<RecorridosAction>();
            view.ResetStackView();
            EnableButtonState(buttonState);
        }

        IEnumerator DoMove(float inTime, RecorridosTile newTile)
        {
            var fromPosition = player.transform.position;
            for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
            {
                player.transform.position = Vector3.Lerp(fromPosition, newTile.Position, t);
                yield return null;
            }
            player.transform.position = newTile.Position;
            view.LightValue(currentValueAnalyzed, false, Color.clear);
            actionsToDo.RemoveAt(0);
            currentValueAnalyzed++;
            yield return new WaitForSeconds(0.5f);
            newTile.RunAction();

        }

        IEnumerator GoRolling(float inTime, RecorridosTile newTile)
        {
            var fromPosition = player.transform.position;
            for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
            {
                player.transform.position = Vector3.Lerp(fromPosition, newTile.Position, t);
                player.transform.Rotate(0, 0, 5, Space.Self);
                yield return null;
            }
            player.transform.eulerAngles = new Vector3(0, 0, 0);
            view.MovingDown();
            StartCoroutine(ResetList(0, true));

            EnableButtonState(true);
        }

        public void GameOver(bool result)
        {
			
            if (result)
            {
                view.ShowVictory();
				if (gameCounter < GAMES_BEFORE_TIME) {
					currentLevel++;
				} else {
					if(timer+CORRECT_SCENE_TIME<100) timer += CORRECT_SCENE_TIME;
				}

            }
            else
            {
                view.ShowDefeat();

            }
            StartCoroutine(ResetList(0, false));
			gameCounter++;
			if (gameCounter >= GAMES_BEFORE_TIME){
				if (gameCounter == GAMES_BEFORE_TIME) {
					withTime = true; 
					Invoke ("ShowNextLevelAnimation", 1);
				} else {
					
					Invoke ("PlayTimeLevel",1);
				}

			}else{
				Invoke("ResetGame", 1);	
			}
            
        }

        public void EnableButtonState(bool state)
        {
            foreach(Button button in buttons)
            {
                button.enabled = state;
            }
        }
            
		private void ShowNextLevelAnimation(){
			view.ShowNextLevelAnimation ();
		}

		public void DecreaseTimer() {
			if(timer > 0) timer--;
		}

		public bool IsTimerDone(){
			return timer == 0;
		}

		public int GetTimer() {
			return timer;
		}

		public void PlayTimeLevel(){
			view.PlayTimeLevel ();
		}


    }
}
