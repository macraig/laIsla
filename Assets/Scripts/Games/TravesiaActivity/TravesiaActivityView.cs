using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Sound;
using System.Security.Policy;
using Assets.Scripts.Common;
using UnityEngine.EventSystems;
using System.Threading;
using System;

public class TravesiaActivityView : LevelView {
	public Button okBtn;
	public List<Image> tiles, clocks;
	public Text letter, number, board;
	public List<Toggle> actions;

	private Sprite[] tileSprites;
	private bool keyboardActive;
	private const int WATER_SPRITE = 0, START_MONSTER_SPRITES = 6, START_SHIP_SPRITES = 6, DIFFERENT_SHIP_SPRITES = 3;

	private TravesiaActivityModel model;

	override public void Next(bool first = false){
		if(first) model.NewSend();
		UpdateView();
		ResetLetterNumber();
		keyboardActive = true;
		CheckOk();

		model.CheckSend();

		SetSend(model.SendCol(), model.SendRow());

		if(model.GameEnded()) {
			EndGame(60, 0, 1250);
		}
	}

	void SetSend(int col, int row) {
		if(col == -1 && row == -1) board.text = "No hay envíos disponibles.";
		else {
			board.text = "Enviar provisiones al puerto " + model.GetColumnString(col) + model.GetRowString(row) + ".";
		}
	}

	void ResetLetterNumber() {
		letter.text = "";
		number.text = "";
	}

	public void OnNumberClick(){
		PlayDropSound ();
		number.text = "";
		CheckOk ();
	}

	public void OnLetterClick(){
		PlayDropSound ();
		letter.text = "";
		CheckOk ();
	}

	void ClocksActive(bool active) {
		clocks.ForEach((c) => c.gameObject.SetActive(active));
	}

	public void OkClick() {
		int row = model.GetRow(number.text);
		int col = model.GetColumn(letter.text);
		TravesiaAction action = CurrentAction();

		if(!model.ExecuteAction(row, col, action)) {
			model.Wrong();
			PlayWrongSound();
		}

		model.AdvanceOne();

		model.CleanRows();

		List<TravesiaEvent> doneShips = model.GetAndRemoveDoneShips();

		model.CheckForRandomEvents();

		if(action == TravesiaAction.SEND) model.NewSend();

		//TODO do something with done ships and remove Next() call.
		DoSomethingWithDoneShips(doneShips);
		Next();
	}

	void DoSomethingWithDoneShips(List<TravesiaEvent> doneShips) {
		foreach(TravesiaEvent doneShip in doneShips) {
			if(doneShip.isShipCorrect){
				//TODO correct.
				model.Correct();
			} else {
				//TODO wrong.
				model.Wrong();
			}
		}
	}

	TravesiaAction CurrentAction() {
		int idx = actions.FindIndex(t => t.isOn);
		switch(idx) {
		case 0:
			return TravesiaAction.SEND;
		case 1:
			return TravesiaAction.REPAIR;
		case 2:
			return TravesiaAction.PROVISION;
		case 3:
			return TravesiaAction.ATTACK;
		}

		return TravesiaAction.SEND;
	}

	void ResetTiles() {
		for(int row = 0; row < TravesiaActivityModel.GRID_ROWS; row++) {
			for (int col = 1; col < TravesiaActivityModel.GRID_COLS - 1; col++) {
				tiles[model.GetSlot(row, col)].sprite = tileSprites[WATER_SPRITE];
			}
		}
	}

	public void Start(){
		model = new TravesiaActivityModel();
		tileSprites = Resources.LoadAll<Sprite>("Sprites/TravesiaActivity/tiles");
		ResetTiles();
		Begin();
	}

	public void Begin(){
		ShowExplanation();
	}

	//Repaint method.
	void UpdateView() {
		ClocksActive(false);
		ResetTiles();
		List<List<TravesiaEvent>> rows = model.GetRows();

		foreach(List<TravesiaEvent> rowEvents in rows) {
			foreach(TravesiaEvent rowEvent in rowEvents) {
				//for each event, place corresponding sprite and clock.
				int slot = model.GetSlot(rowEvent.row, rowEvent.col);

				switch(rowEvent.GetState()) {
				case TravesiaEventState.SHIP:
					tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES)];
					clocks[slot].gameObject.SetActive(true);
					clocks[slot].GetComponentInChildren<Text>().text = rowEvent.GetProvisions().ToString();
					break;
				case TravesiaEventState.WRECKED_SHIP:
					tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + 1];
					clocks[slot].gameObject.SetActive(true);
					clocks[slot].GetComponentInChildren<Text>().text = rowEvent.GetProvisions().ToString();
					break;
				case TravesiaEventState.SUNK_SHIP:
					tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + 2];
					break;
				case TravesiaEventState.MONSTER:
					tiles[slot].sprite = tileSprites[START_MONSTER_SPRITES + rowEvent.GetObjectNumber()];
					break;
				}
			}
		}
	}

	Sprite GetWreckedShip(Sprite sprite) {
		return tileSprites[Array.IndexOf(tileSprites, sprite) + 1];
	}

	Sprite GetDestroyedShip(Sprite sprite) {
		return tileSprites[Array.IndexOf(tileSprites, sprite) + 2];
	}

	public void LetterClick(string l){
		PlaySoundClick ();
		letter.text = l;
		CheckOk();
	}

	public void NumberClick(string n){
		PlaySoundClick ();
		number.text = n;
		CheckOk();
	}

	void CheckOk() {
		okBtn.interactable = CanSubmit();
	}

	bool CanSubmit() {
		return letter.text.Length == 1 && number.text.Length == 1 && actions.Exists(t => t.isOn);
	}

	override public void RestartGame(){
		base.RestartGame ();
		Start();
	}

	void OnGUI() {
		Event e = Event.current;
		if (e.isKey && e.type == EventType.KeyUp && keyboardActive) {
			if (e.keyCode >= KeyCode.A && e.keyCode <= KeyCode.I) {
				Debug.Log ("Detected key code: " + e.keyCode);
				LetterClick(e.keyCode.ToString());
			} else if (e.keyCode == KeyCode.Return) {
				Debug.Log ("Detected key code: Enter");
				if(okBtn.interactable) OkClick();
			} else if((e.keyCode >= KeyCode.Alpha1 && e.keyCode <= KeyCode.Alpha6) || (e.keyCode >= KeyCode.Keypad1 && e.keyCode <= KeyCode.Keypad6)) {
				NumberClick(e.keyCode.ToString()[e.keyCode.ToString().Length - 1].ToString());
			}
		}
	}
}