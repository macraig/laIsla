using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Sound;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Games.Shipments
{
    public class ShipmentsView : LevelView
    {
        public Text clock;
        public Image clockImage;

        public static ShipmentsView instance; 

        public MapGenerator MapGenerator;
        public GameObject[] AnswerRowGameObjects;
        public Sprite[] AnswerCellSprites;
        public Button OkButton;
        public Button NextButton;
        public Button[] NumberButtons;
        public Color FocusedColor;
        public Color UnfocusedColor;
        public Text ScaleText;
        private int _currentFocus;
        public Image StartPlace;
        public Image FinishPlace;

        public GameObject Player;
        public Sprite[] PlayeSprites;
        private float _durationPerUnity = 0.5f;

        public AudioClip BoatSound;

        public AudioClip GoldSound;
        public AudioClip DropGoldSound;
        private int _totalGold;
        private int _currentGold;

        public Text TotalGoldText;

        public ShipmentsModel Model { get; set; }

        private int attempsToGenerate;

        private List<ShipmentEdge> _edgesAnswers;
        private int indexToCorrect;
        bool timerActive, switchTime;


        void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this) Destroy(gameObject);
        }
        
        // Use this for initialization
        void Start ()
        {
            _totalGold = 0;

            AddCellLiseners();
            GetAnswerCells()[0].Value = 0;
            Model = new ShipmentsModel();
            _currentFocus = 1;
            HighlightCurrentFocus();
            menuBtn.onClick.AddListener(OnClickMenuBtn);
            attempsToGenerate = 0;
            _edgesAnswers = new List<ShipmentEdge>();
            ShowExplanation();
        }

        public override void RestartGame()
        {
            base.RestartGame();
            Start();
        }


        void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeFocusCell(_currentFocus + 1); }        
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeFocusCell(_currentFocus - 1); }        
            if (Input.GetKeyDown(KeyCode.UpArrow)) { ChangeFocusCell(_currentFocus - 3); }        
            if (Input.GetKeyDown(KeyCode.DownArrow)) { ChangeFocusCell(_currentFocus + 3); }
            else if(!GetCurrentAnswerCell().GetComponent<Button>().enabled) return;
            else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) { OnClickNumberBtn(0); }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) { OnClickNumberBtn(1); }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) { OnClickNumberBtn(2); }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) { OnClickNumberBtn(3); }
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) { OnClickNumberBtn(4); }
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) { OnClickNumberBtn(5); }
            else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) { OnClickNumberBtn(6); }
            else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) { OnClickNumberBtn(7); }
            else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) { OnClickNumberBtn(8); }
            else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) { OnClickNumberBtn(9); }
            else if (Input.GetKeyDown(KeyCode.Backspace)) { OnClickClear(); }
        }

        

        public void OnClickNumberBtn(int number)
        {
            ShipmentsAnswerCell cell = GetCurrentAnswerCell();
            if (cell.Type == AnswerCellType.Numeric)
            {
                Text uiText = cell.GetComponentInChildren<Text>();
                if (uiText.text.Length > 2)
                {
                    SoundController.GetController().PlayDropSound();
                }
                else
                {
                    if (cell.Value == -1)
                    {
                        uiText.text = "";
                    }
                    SoundController.GetController().PlayTypingSound();
                    uiText.text += number;
                    cell.Value = int.Parse(uiText.text);
                }
            }
            
        }

        public void OnClickClear()
        {
            SoundController.GetController().PlayTypingSound();
            GetCurrentAnswerCell().Clear();
        }

        private ShipmentsAnswerCell GetCurrentAnswerCell()
        {
            return GetAnswerCells()[_currentFocus];
        }

        public void ChangeFocusCell(int cell)
        {
            if(cell < 1 || cell > GetAnswerCells().Count - 1) return;
            PlaySoundClick();
            UnhighlightCurrentFocus();
            _currentFocus = cell;
            HighlightCurrentFocus();
        }

        private void HighlightCurrentFocus()
        {
            GetCurrentAnswerCell().GetComponent<Button>().image.color = FocusedColor;
        }

        private void UnhighlightCurrentFocus()
        {
            GetCurrentAnswerCell().Unpaint();
        }

        public override void Next(bool first = false)
        {

            attempsToGenerate = 0;
            if (!first) PlaySoundClick();
            EnableComponents(true);
            OkButton.gameObject.SetActive(true);
            OkButton.enabled = true;
            NextButton.gameObject.SetActive(false);
            Model.NextExercise();
            MapGenerator.SafeLocatePlaces(Model.Nodes, Model.Edges);

            while (!MapGenerator.CheckAllDistances(Model.Edges))
            {
                if (attempsToGenerate > 10)
                {
                    Model.RemainExercises++;
                    
                    Model.NextExercise();
                } else if (attempsToGenerate > 50)
                {
                    throw new Exception("Dificil de generar");

                }
                foreach (ShipmentEdge edge in Model.Edges)
                {
                    edge.Length = 0;
                }
                MapGenerator.SafeLocatePlaces(Model.Nodes, Model.Edges);
                attempsToGenerate++;
            }


            MapGenerator.TraceEdges(Model.Edges);
            ScaleText.text = Model.Scale + " km";
            ClearAnswers();
            StartPlace.sprite =
                MapGenerator.Places.Find(e => e.Type == ShipmentNodeType.Start).GetComponent<Image>().sprite;
            FinishPlace.sprite =
                MapGenerator.Places.Find(e => e.Type == ShipmentNodeType.Finish).GetComponent<Image>().sprite;
            _currentFocus = 1;
            ChangeFocusCell(1);
            SetPlayerToFirstPlace();
            EnableGameButtons(true);
            Player.transform.SetAsLastSibling();
            MapGenerator.Ruler.transform.SetAsLastSibling();
            _currentGold = 0;
            TotalGoldText.text = "" + _totalGold;
            UpdateTryButton();
    
        }

        private void SetPlayerToFirstPlace()
        {
            MapPlace place = MapGenerator.Places.Find(e => e.Type == ShipmentNodeType.Start);
            Player.transform.position = place.transform.position;
            Player.GetComponent<Image>().sprite = GetNeutralBoatSprite();
        }

        private Sprite GetNeutralBoatSprite()
        {
            return PlayeSprites[0];
        }

        private void ClearAnswers()
        {
            List<ShipmentsAnswerCell> cells = GetAnswerCells();
            for (int i = cells.Count - 1; i > 0; i--)
            {
                cells[i].Clear();

            }
            
        }


        private List<ShipmentsAnswerCell> GetAnswerCells()
        {
            List<ShipmentsAnswerCell> answerCells = new List<ShipmentsAnswerCell>();
            foreach (GameObject answerRowGameObject in AnswerRowGameObjects)
            {
                answerCells.AddRange(answerRowGameObject.GetComponentsInChildren<ShipmentsAnswerCell>());
            }
            return answerCells;
        }

        private bool AnsweStateIsValid()
        {
            // Me fijo todas las respuestas
            int i = 0;
            for (; i < AnswerRowGameObjects.Length; i++)
            {
                GameObject answerRowGameObject = AnswerRowGameObjects[i];
                List<ShipmentsAnswerCell> answerCells = answerRowGameObject.GetComponentsInChildren<ShipmentsAnswerCell>().ToList();
                List<ShipmentsAnswerCell> shipmentsAnswerCells = answerCells.FindAll(e => e.Value == -1);
                // si hay entr 1 y 2 erroneas ya esta mal, xq esta incompleta
                if (shipmentsAnswerCells.Count > 0 && shipmentsAnswerCells.Count < answerCells.Count) return false;
                // si hay mas de 0 aca quiere decir que son todos erroneas, entonces si todas las que siguen
                // son erroneas puede ser una respuesta valida
                if (shipmentsAnswerCells.Count > 0) break;
            }

            // me fijo las siguientes, que en caso de que haya deberian estar vacias
            for (; i < AnswerRowGameObjects.Length; i++)
            {
                GameObject answerRowGameObject = AnswerRowGameObjects[i];
                List<ShipmentsAnswerCell> answerCells = answerRowGameObject.GetComponentsInChildren<ShipmentsAnswerCell>().ToList();
                List<ShipmentsAnswerCell> shipmentsAnswerCells = answerCells.FindAll(e => e.Value == -1);
                // como se que hay al menos una erronea, tienen que ser todas xq sino esta mal
                if (shipmentsAnswerCells.Count < answerCells.Count) return false;
            }

            return true;
        }

        public void OnClickMapPlace(int id, ShipmentNodeType type)
        {
            ShipmentsAnswerCell cell = GetCurrentAnswerCell();
            if (cell.Type == AnswerCellType.Numeric)
            {
                SoundController.GetController().PlayDropSound();
            }
            else
            {
                SoundController.GetController().PlayTypingSound();
                cell.Value = id;
                cell.GetComponent<Image>().sprite = AnswerCellSprites[id];
                List<ShipmentsAnswerCell> cells = GetAnswerCells();
                if (type == ShipmentNodeType.Other && _currentFocus + 2 < cells.Count && _currentFocus%3 == 1)
                {
                    ShipmentsAnswerCell cell2 = cells[_currentFocus + 2];
                    cell2.Value = id;
                    cell2.GetComponent<Image>().sprite = AnswerCellSprites[id];
                }
                else if (type == ShipmentNodeType.Other && _currentFocus -2 > 0 && _currentFocus % 3 == 0)
                {
                    ShipmentsAnswerCell cell2 = cells[_currentFocus - 2];
                    cell2.Value = id;
                    cell2.GetComponent<Image>().sprite = AnswerCellSprites[id];
                }
                FocusNextEmptyCell();

            }


            
        }

        private void FocusNextEmptyCell()
        {
            List<ShipmentsAnswerCell> cells = GetAnswerCells();
            for (int i = _currentFocus + 1; i < cells.Count; i++)
            {
                if (cells[i].Value != -1) continue;
                ChangeFocusCell(i);
                return;
            }
            for (int i = 1; i < _currentFocus; i++)
            {
                if (cells[i].Value != -1) continue;
                ChangeFocusCell(i);
                return;
            }
        }

        public void UpdateTryButton()
        {
            OkButton.interactable = AnsweStateIsValid();
        }

        private void AddCellLiseners()
        {
            List<ShipmentsAnswerCell> cells = GetAnswerCells();
            for (int i = 1; i < cells.Count; i++)
            {
                var i1 = i;
                cells[i].Value = -1;
                cells[i].GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        MapPlace mapPlace = MapGenerator.Places.Find(e => e.Id == cells[i1].Value);
                        if(cells[i1].Value != 1 && mapPlace != null)
                        {
                            if (mapPlace.Type == ShipmentNodeType.Other && i1 + 2 < cells.Count && i1 % 3 == 1) cells[i1 + 2].Clear();
                            else if (mapPlace.Type == ShipmentNodeType.Other && i1 - 2 > 0 && i1 % 3 == 0) cells[i1 - 2].Clear();
                        }
                        
                        cells[i1].Clear();
                        ChangeFocusCell(i1);
                    }

                    );
            }
        }

        public void OnClickOk()
        {
            EnableComponents(false);
            _edgesAnswers.Clear();

            OkButton.enabled = false;

            PlaySoundClick();
            EnableGameButtons(false);

            
            for (int i = 0; i < AnswerRowGameObjects.Length; i++)
            {
                GameObject answerRowGameObject = AnswerRowGameObjects[i];
                List<ShipmentsAnswerCell> answerCells = answerRowGameObject.GetComponentsInChildren<ShipmentsAnswerCell>().ToList();
                if (answerCells[0].Value == -1) break;
                ShipmentEdge shipmentEdge = new ShipmentEdge
                {
                    IdNodeA = answerCells[0].Value,
                    IdNodeB = answerCells[1].Value,
                    Length = answerCells[2].Value
                };
                _edgesAnswers.Add(shipmentEdge);
            }

            ContinueCorrection(_edgesAnswers, 0);

        
        }

        private void EnableGameButtons(bool enable)
        {
            foreach (ShipmentsAnswerCell cell in GetAnswerCells())
            {
                cell.GetComponent<Button>().enabled = enable;
            }

            foreach (Button numberButton in NumberButtons)
            {
                numberButton.enabled = enable;
            }
            MapGenerator.Ruler.GetComponent<Button>().enabled = enable;
        }

        private void CheckEdgeAnswer(ShipmentEdge realEdge, List<ShipmentEdge> edgeAnswers, int index)
        {
            if(index > 0) Invoke("PlayBoatSond", 0.2f);
            else { PlayBoatSond();}
            // no hay arista
            if (realEdge == null)
            {
                AnswerEdgeNotExists(
                    MapGenerator.Places.Find(e => e.Id == edgeAnswers[index].IdNodeB).transform.position, 2, edgeAnswers);
                return;
            };

            var answerEdge = edgeAnswers[index];
            MapPlace destine = MapGenerator.Places.Find(e => e.Id == answerEdge.IdNodeB);

            int answerValue = answerEdge.Length / (Model.Scale);
            int rest = answerEdge.Length % Model.Scale;

            // caso correcto
            if (answerValue == realEdge.Length && rest == 0)
            {
                Player.transform.DOMove(destine.transform.position, GetDuration(realEdge.Length))
                    .OnComplete(() =>
                    {
                        AddGold();
                        index++;
                        indexToCorrect = index;
                        Invoke("CorrectAnswerContinueCorrection", 1);
                    });
            }
            // se pasó
            else if (answerValue >= realEdge.Length) AnswerEdgeTooLong(destine.transform.position, realEdge.Length, edgeAnswers);

            // se quedó corto
            else AnswerEdgeTooShort(destine.transform.position, realEdge.Length);
        }

        private void CorrectAnswerContinueCorrection()
        {
            ContinueCorrection(_edgesAnswers, indexToCorrect);

        }
        private void PlayBoatSond()
        {
            SoundController.GetController().PlayClip(BoatSound);
        }

        private void AddGold()
        {
            _currentGold += 10;
            _totalGold += 10;
            TotalGoldText.text = "" + _totalGold;
            SoundController.GetController().PlayClip(GoldSound);
        }

        private void AnswerEdgeNotExists(Vector3 destine, int length, List<ShipmentEdge> edgeAnswers)
        {
            Player.transform.DOMove(destine - (destine - Player.transform.position) * 0.5f, GetDuration(length)).OnComplete(
               BadEdgeAnswerEnd);
        }

        private void ContinueCorrection(List<ShipmentEdge> edgeAnswers, int index)
        {
            if (index == edgeAnswers.Count)
            {
                FinalCheckAnswer();
            }
            else
            {
                ShipmentEdge edge =
                   Model.Edges.Find(
                       e =>
                       {
                           var shipmentEdge = edgeAnswers[index];
                           return (e.IdNodeA == shipmentEdge.IdNodeA && e.IdNodeB == shipmentEdge.IdNodeB) ||
                                  (e.IdNodeB == shipmentEdge.IdNodeA && e.IdNodeA == shipmentEdge.IdNodeB);
                       });
                CheckEdgeAnswer(edge, edgeAnswers, index);
            }
        }

        private void FinalCheckAnswer()
        {
            if (Model.IsCorrectAnswer(_edgesAnswers))
            {
                if (timerActive)
                {
                    Model.CorrectTimer();
                    SetClock();
                }

                ShowRightAnswerAnimation();

            }
            else
            {
                DropGold();
                ShowWrongAnswerAnimation();
            }
          

        }

        private void AnswerEdgeTooShort(Vector3 destine, int length)
        {
            Player.transform.DOMove(destine - (destine - Player.transform.position) * 0.5f, GetDuration(length)).OnComplete(
                BadEdgeAnswerEnd);
        }

        private void BadEdgeAnswerEnd()
        {
            Player.GetComponent<Image>().sprite = GetBrokenBoatSprite();
            DropGold();
            Invoke("FinalCheckAnswer", 1.5f);

        }


        private void AnswerEdgeTooLong(Vector3 destine, int length, List<ShipmentEdge> edgeAnswers)
        {
            Player.transform.DOMove(destine + (destine - Player.transform.position) * 0.2f, GetDuration(length)).OnComplete(
                BadEdgeAnswerEnd                  
            );
        }

        private void DropGold()
        {
            _totalGold -= _currentGold;
            _currentGold = 0;
            TotalGoldText.text = "" + _totalGold;
            SoundController.GetController().PlayClip(DropGoldSound);
        }


        private Sprite GetBrokenBoatSprite()
        {
            return PlayeSprites[1];
        }

        private float GetDuration(int length)
        {
            return length*_durationPerUnity;
        }

        public override void OnRightAnimationEnd()
        {

            if (Model.GameEnd())
            {
                ShowNextLevelAnimation();
            }
            else
            {
                OkButton.gameObject.SetActive(false);
                NextButton.gameObject.SetActive(true);
            }
        }

        public override void OnWrongAnimationEnd()
        {
            
            if (timerActive)
            {
                EndGame(60, 0, 1250);
            }
            else
            {
                base.OnWrongAnimationEnd();
                SetPlayerToFirstPlace();
                OkButton.enabled = true;
                EnableGameButtons(true);
            }

        }
        void SetClock()
        {
            clock.text = Model.GetTimer().ToString();
        }

        public override void OnNextLevelAnimationEnd()
        {
            base.OnNextLevelAnimationEnd();
            PlayTimeLevelMusic();
            menuBtn.interactable = false;
            NextTimeExercise();
        }

        void NextTimeExercise()
        {
            clock.gameObject.SetActive(true);
            clockImage.gameObject.SetActive(true);
            SetClock();
            StartTimer(true);
            Next();
/*
            SetRule();
*/
        }

        void StartTimer(bool first = false)
        {
            StartCoroutine(TimerFunction(first));
            timerActive = true;
        }

        public IEnumerator TimerFunction(bool first = false)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("segundo");

            UpdateView();

            if (timerActive) StartTimer();
        }

        void UpdateView()
        {
            Model.DecreaseTimer();

            SetClock();

            if (Model.IsTimerDone())
            {
                timerActive = false;
                EndGame(60, 0, 1250);
            }
        }

    }
}
