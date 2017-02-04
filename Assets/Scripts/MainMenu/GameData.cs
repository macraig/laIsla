using UnityEngine;
using System.Collections;

namespace Assets.Scripts.MainMenu
{

    public class GameData
    {
        private string name;
        private int area;
        private int index;

        public GameData(string name, int area, int index)
        {
            this.name = name;
            this.area = area;
            this.index = index;
        }

        public string GetName()
        {
            return name;
        }

        public int GetArea()
        {
            return area;
        }

        public int GetIndex()
        {
            return index;
        }
    }
}
