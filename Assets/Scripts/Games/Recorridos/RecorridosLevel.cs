using System;
using SimpleJSON;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Games.Recorridos;
using UnityEngine;
namespace Assets.Scripts.Games.Recorridos
{
public class RecorridosLevel {

	private int bombs,path,nuts;

	public RecorridosLevel(JSONClass source) {
			bombs = source["bombs"].AsInt;
			path = source["path"].AsInt;
			nuts = source["nuts"].AsInt;
	}

		public int GetPath(){
			return path;
		}

		public int GetBombs(){
			return bombs;
		}

		public int GetNuts(){
			return nuts;
		}

	
}
}