using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Games.Shipments
{
    public class ShipmentsAnswerCell : MonoBehaviour
    {

        public AnswerCellType Type;
        public Color UnfilledColor;
        public Sprite UnfilledSprite;
        // si es Place es el id, si es Numeric es el numero
    

        private int _value;

        public int Value
        {
            get { return _value; }
            set
            {
                Debug.Log("Setted");

                _value = value;
                ShipmentsView.instance.UpdateTryButton();

            }
        }


        public void Clear()
        {
            Image image = GetComponent<Image>();
            image.sprite = UnfilledSprite;
            image.color = UnfilledColor;
            _value = -1;
            if (Type == AnswerCellType.Numeric)
            {
                GetComponentInChildren<Text>().text = "-";
            }
        }

        public void Unpaint()
        {
            GetComponent<Image>().color = UnfilledColor;

        }
    }


    public enum AnswerCellType
    {
        Place, Numeric
    }
}