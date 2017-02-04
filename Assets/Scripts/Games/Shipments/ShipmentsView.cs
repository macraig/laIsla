using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Games.Shipments
{
    public class ShipmentsView : LevelView
    {

        public static ShipmentsView instance; 

        public MapGenerator MapGenerator;
        public GameObject[] AnswerRowGameObjects;
        public Sprite[] AnswerCellSprites;
        public Button TryButton;
        public Color FocusedColor;
        public Color UnfocusedColor;
        public Text ScaleText;
        private int _currentFocus;
        public List<int> measuresList;

        public ShipmentsModel Model { get; set; }

        void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this) Destroy(gameObject);
        }
        
        // Use this for initialization
        void Start ()
        {
            AddCellLiseners();
            GetAnswerCells()[0].Value = 0;
            Model = new ShipmentsModel();
            _currentFocus = 1;
            HighlightCurrentFocus();
            measuresList = new List<int>();
            Next(true);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) { OnClickAnswerCell(_currentFocus + 1); }        
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { OnClickAnswerCell(_currentFocus - 1); }        
            if (Input.GetKeyDown(KeyCode.UpArrow)) { OnClickAnswerCell(_currentFocus - 3); }        
            if (Input.GetKeyDown(KeyCode.DownArrow)) { OnClickAnswerCell(_currentFocus + 3); }
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
            ShipmentsAnswerCell cell = GetCurrentAnswerCell();
            cell.Value = -1;
            if (cell.Type == AnswerCellType.Numeric)
            {
                cell.GetComponentInChildren<Text>().text = "-";
            }
            cell.Clear();
        }

        private ShipmentsAnswerCell GetCurrentAnswerCell()
        {
            return GetAnswerCells()[_currentFocus];
        }

        public void OnClickAnswerCell(int cell)
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
/*
            GetAnswerCells()[_currentFocus].GetComponent<Button>().image.color = UnfocusedColor;
*/
        }

        public override void Next(bool first = false)
        {
            measuresList.Clear();
            Model.NextExercise();
            MapGenerator.SafeLocatePlaces(Model.Nodes, Model.Edges);

          /*  if (first)
            {
                MapGenerator.LocatePlaces(Model.Nodes, Model.Edges);
            }
            else
            {
            }*/
           
            
            MapGenerator.TraceEdges(Model.Edges);
            ScaleText.text = Model.Scale + " kg";
            ClearAnswers();


        }

        private void ClearAnswers()
        {
            foreach (ShipmentsAnswerCell shipmentsAnswerCell in GetAnswerCells())
            {
                if(shipmentsAnswerCell.Value == 0) continue;
                shipmentsAnswerCell.Clear();
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

        public void OnClickMapPlace(int id, bool isIntermediate)
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
                OnClickAnswerCell(_currentFocus + 1);
              /*  if (isIntermediate && _currentFocus%2 == 1)
                {
                    ShipmentsAnswerCell cell2 = GetAnswerCells()[_currentFocus + 2];
                    cell2.Value = id;
                    cell2.GetComponent<Image>().sprite = AnswerCellSprites[id];


                }*/
            }


            
        }

        public void UpdateTryButton()
        {
            TryButton.interactable = AnsweStateIsValid();
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
                        OnClickAnswerCell(i1);
                        
                    }

                    );
            }
        }

        public void OnClickOk()
        {
            PlaySoundClick();
            List<ShipmentEdge> edgeAnswers = new List<ShipmentEdge>();

            int i = 0;
            for (; i < AnswerRowGameObjects.Length; i++)
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
                edgeAnswers.Add(shipmentEdge);
            }

            if (Model.IsCorrectAnswer(edgeAnswers))
            {
                ShowRightAnswerAnimation();
            }
            else
            {
                ShowWrongAnswerAnimation();
            }
        }

        public override void OnRightAnimationEnd()
        {
            base.OnRightAnimationEnd();
        }

        public override void OnWrongAnimationEnd()
        {
            base.OnWrongAnimationEnd();
        }
    }
}
