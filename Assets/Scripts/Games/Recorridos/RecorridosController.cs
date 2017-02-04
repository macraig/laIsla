using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Games
{
    public class RecorridosController : MonoBehaviour
    {
        public float timeBetweenActions;

        public static RecorridosController instance;

        public enum RecorridosTileEnum { Path, Hole, Wall, Start, End, Nut, Bomb, Fire }

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

        private Vector2 initialPupperGridPosition;
        private List<RecorridosTile> pathTiles;
        private int nutCount;
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
            keys = new List<RecorridosTileEnum>();

            keys.Add(RecorridosTileEnum.Path);
            keys.Add(RecorridosTileEnum.Hole);
            keys.Add(RecorridosTileEnum.Bomb);
            keys.Add(RecorridosTileEnum.Wall);
            keys.Add(RecorridosTileEnum.Nut);
            keys.Add(RecorridosTileEnum.Fire);

            tileDictionary = new Dictionary<RecorridosTileEnum, Sprite>();

            tileDictionary.Add(RecorridosTileEnum.Path, pathSprite);
            tileDictionary.Add(RecorridosTileEnum.Hole, holeSprite);
            tileDictionary.Add(RecorridosTileEnum.Bomb, bombSprite);
            tileDictionary.Add(RecorridosTileEnum.Wall, wallSprite);
            tileDictionary.Add(RecorridosTileEnum.Nut, nutSprite);
            tileDictionary.Add(RecorridosTileEnum.Fire, fireSprite);

            gridSpace = new RecorridosTile[7][];
            view = GetComponent<RecorridosView>();
            view.SetPlayerImage(player.GetComponent<Image>());
            ResetGame();
        }

        public void FallInHole()
        {
            int randonNewPath = Random.Range(0, pathTiles.Count);
            puppetGridPosition.x = pathTiles[randonNewPath].GridPositionX;
            puppetGridPosition.y = pathTiles[randonNewPath].GridPositionY;
            StartCoroutine(GoRolling(2, gridSpace[(int)puppetGridPosition.x][(int)puppetGridPosition.y]));
        }

        public void BackToStart()
        {
            puppetGridPosition.x = (int)initialPupperGridPosition.x;
            puppetGridPosition.y = (int)initialPupperGridPosition.y;
            StartCoroutine(GoRolling(2, gridSpace[(int)puppetGridPosition.x][(int)puppetGridPosition.y]));
        }

        public void PickNut(int gridPositionX, int gridPositionY)
        {
            gridSpace[gridPositionX][gridPositionY].Type = RecorridosTileEnum.Path;
            gridSpace[gridPositionX][gridPositionY].Sprite = pathSprite;
            nutCount++;
            view.SetNutTextCounter(nutCount);
        }

        private void ResetGame()
        {
            nutCount = 0;

            int rowCounter = 0;

            pathTiles = new List<RecorridosTile>();


            gridSpace[rowCounter] = new RecorridosTile[7];

            for (int i = 0; i < boardPosition.transform.childCount; i++)
            {
                if (i % 7 == 0 && i!=0)
                {
                    rowCounter++;
                    gridSpace[rowCounter] = new RecorridosTile[7];
                }
                    int randomKey = Random.Range(0, keys.Count);

                    RecorridosTileEnum randomTile = keys[randomKey];

                    Sprite spriteToPut;

                    tileDictionary.TryGetValue(randomTile, out spriteToPut);

                    gridSpace[rowCounter][i % 7] = new RecorridosTile(randomTile,
                        boardPosition.transform.GetChild(i).transform.position, spriteToPut, boardPosition.transform.GetChild(i).GetComponent<Image>(),rowCounter,i%7);
                
            }

            SetStartAndEndPosition();
            EnableButtonState(true);
            view.ResetGame();
            player.transform.position = gridSpace[(int)initialPupperGridPosition.x][(int)initialPupperGridPosition.y].Position;
            actionsToDo = new List<RecorridosAction>();

        }


        //ESTE METODO DA ASCO, LO VOY A MEJORAR EL FINDE
        private void SetStartAndEndPosition()
        {

            int randomStartQuarterGrid = Random.Range(0, 4);
            int randomPositionInQuarterGrid = Random.Range(0, 9);

            int row;

            if (randomPositionInQuarterGrid < 3)
            {
                row = 0;
            }
            else if (randomPositionInQuarterGrid < 6)
            {
                row = 1;
            }
            else
            {
                row = 2;
            }

            int finalRowPosition;
            int finalColumnPosition;

            if (randomStartQuarterGrid == 0)
            {
                finalRowPosition = row;
                finalColumnPosition = randomPositionInQuarterGrid % 3;
            }
            else if (randomStartQuarterGrid == 1)
            {
                finalRowPosition = row;
                finalColumnPosition = 4 + randomPositionInQuarterGrid % 3;
            }
            else if (randomStartQuarterGrid == 2)
            {
                finalRowPosition = 4 + row;
                finalColumnPosition = randomPositionInQuarterGrid % 3;
            }
            else
            {
                finalRowPosition = 4 + row;
                finalColumnPosition = 4 + randomPositionInQuarterGrid % 3;
            }

            gridSpace[finalRowPosition][finalColumnPosition].Type = RecorridosTileEnum.Start;
            gridSpace[finalRowPosition][finalColumnPosition].Sprite = startSprite;
            puppetGridPosition = new Vector2(finalRowPosition, finalColumnPosition);
            initialPupperGridPosition = new Vector2(finalRowPosition, finalColumnPosition);

            
            if(randomStartQuarterGrid == 0)
            {
                randomStartQuarterGrid = 3;
            }else if(randomStartQuarterGrid == 1)
            {
                randomStartQuarterGrid = 2;
            }else if(randomStartQuarterGrid == 2)
            {
                randomStartQuarterGrid = 1;
            }
            else
            {
                randomStartQuarterGrid = 0;
            }
            
            randomPositionInQuarterGrid = Random.Range(0, 9);

            if (randomPositionInQuarterGrid < 3)
            {
                row = 0;
            }
            else if (randomPositionInQuarterGrid < 6)
            {
                row = 1;
            }
            else
            {
                row = 2;
            }

            if (randomStartQuarterGrid == 0)
            {
                finalRowPosition = row;
                finalColumnPosition = randomPositionInQuarterGrid % 3;
            }
            else if (randomStartQuarterGrid == 1)
            {
                finalRowPosition = row;
                finalColumnPosition = 4 + randomPositionInQuarterGrid % 3;
            }
            else if (randomStartQuarterGrid == 2)
            {
                finalRowPosition = 4 + row;
                finalColumnPosition = randomPositionInQuarterGrid % 3;
            }
            else
            {
                finalRowPosition = 4 + row;
                finalColumnPosition = 4 + randomPositionInQuarterGrid % 3;
            }


            gridSpace[finalRowPosition][finalColumnPosition].Type = RecorridosTileEnum.End;
            gridSpace[finalRowPosition][finalColumnPosition].Sprite = endSprite;

            int xDifference = Mathf.Abs(finalRowPosition - (int)initialPupperGridPosition.x);
            int yDifference = Mathf.Abs(finalColumnPosition - (int)initialPupperGridPosition.y);

            for(int i = 0; i < (xDifference + yDifference)-1; i++)
            {
                int randomRowOrColumn;

                if(Mathf.Abs(finalRowPosition - (int)initialPupperGridPosition.x) == 0)
                {
                    randomRowOrColumn = 1;
                }else if(Mathf.Abs(finalColumnPosition - (int)initialPupperGridPosition.y) == 0)
                {
                    randomRowOrColumn = 0;
                }else
                {
                    randomRowOrColumn = Random.Range(0, 2);
                }

               

                if(randomRowOrColumn == 0)
                {
                    if (initialPupperGridPosition.x > finalRowPosition)
                    {
                        finalRowPosition += 1;
                    }
                    else if (initialPupperGridPosition.x < finalRowPosition)
                    {
                        finalRowPosition -= 1;
                    }
                }
                else
                {
                    if (initialPupperGridPosition.y > finalColumnPosition)
                    {
                        finalColumnPosition += 1;
                    }
                    else if (initialPupperGridPosition.y < finalColumnPosition)
                    {
                        finalColumnPosition -= 1;
                    }
                }
                pathTiles.Add(gridSpace[finalRowPosition][finalColumnPosition]);

                gridSpace[finalRowPosition][finalColumnPosition].Type = RecorridosTileEnum.Path;
                gridSpace[finalRowPosition][finalColumnPosition].Sprite = pathSprite;
            }


            



        }

        // Update is called once per frame
        void Update()
        {

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
            if (positionToAnalyze.x < 7  && positionToAnalyze.x > -1 && 
                positionToAnalyze.y < 7 && positionToAnalyze.y > -1 &&
                gridSpace[(int)positionToAnalyze.x][(int)positionToAnalyze.y].Type != RecorridosTileEnum.Wall)
            {
                view.LightValue(currentValueAnalyzed, true, Color.yellow);
                puppetGridPosition.x = (int)positionToAnalyze.x;
                puppetGridPosition.y = (int)positionToAnalyze.y;
                StartCoroutine(DoMove(timeBetweenActions, gridSpace[(int)positionToAnalyze.x][(int)positionToAnalyze.y]));

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

        public void EndGame(bool result)
        {
            if (result)
            {
                view.ShowVictory();

            }
            else
            {
                view.ShowDefeat();

            }
            StartCoroutine(ResetList(0, false));
            Invoke("ResetGame", 3);
        }

        public void EnableButtonState(bool state)
        {
            foreach(Button button in buttons)
            {
                button.enabled = state;
            }
        }
            



    }
}
