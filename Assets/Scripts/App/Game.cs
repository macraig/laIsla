using UnityEngine;
using Assets.Scripts.Metrics;
using Assets.Scripts.Games;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.App
{
	[System.Serializable]
    public class Game 
    {
       
		public int id;
		public Sprite icon;    
		public string name;
		public string description; 
		public string prefabName;


        public int GetId()
        {
            return id;
        }

        public string GetPrefabName()
        {
            return prefabName;
        }

		public void SetIcon(Sprite icon)
		{
			this.icon = icon;
		}

		public Sprite GetIcon()
		{
			return icon;
		}

		public string GetName()
		{
			return name;
		}

		public void SetName(string name)
		{
			 this.name=name;
		}

		public string GetDescription()
		{
			return description;
		}

		public void SetDescription(string description)
		{
			this.description = description;
		}

		public void SetId(int id)
		{
			this.id = id;
		}

		public void SetPrefabName(String prefabName)
		{
			this.prefabName = prefabName;
		}

   
    }
}
