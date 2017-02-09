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
	private const int WATER_SPRITE = 0, START_MONSTER_SPRITES = 1, START_SHIP_SPRITES = 5, DIFFERENT_SHIP_SPRITES = 4, DIFFERENT_MONSTER_SPRITES = 2;

	private TravesiaActivityModel model;

	override public void Next(bool first = false){
		if(first) model.NewSend();
		ResetLetterNumber();
		ResetActions();
		keyboardActive = true;
		EnableActions(true);
		CheckOk();

		model.CheckSend();

		SetSend(model.SendCol(), model.SendRow());

		if(model.GameEnded()) {
			EndGame(60, 0, 1250);
		}
	}

	void EnableActions(bool enabled) {
		actions.ForEach(a => a.enabled = enabled);
	}

	void SetSend(int col, int row) {
		if(col == -1 && row == -1) board.text = "No hay envíos disponibles.";
		else {
			board.text = "Enviar provisiones al puerto " + model.GetColumnString(col) + model.GetRowString(row) + ".";
		}
	}

	void ResetActions() {
		actions.ForEach(t => t.isOn = false);
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

		okBtn.interactable = false;
		keyboardActive = false;
		EnableActions(false);

		StartCoroutine(ExecuteActions(row, col, action));
	}

	IEnumerator ExecuteActions(int row, int col, TravesiaAction action){
		yield return new WaitForSeconds(0.5f);
		//First execute user action.
		if(model.ExecuteAction(row, col, action)) {
			if(action != TravesiaAction.SEND) SetTile(model.GetEvent(row, col));
		} else {
			model.Wrong();
			PlayWrongSound();
		}

		//update each row with a two second delay.
		List<List<TravesiaEvent>> rows = model.GetRows();
		for(int i = 0; i < rows.Count; i++) {
			if(!model.IsRowEmpty(i)) {
				yield return new WaitForSeconds(1f);

				List<TravesiaEvent> rowEvents = rows[i];

				model.AdvanceRow(rowEvents);
				model.CleanRow(i);

				TravesiaEvent ship = model.GetAndRemoveDoneShipsFromRow(i);
				if(ship != null) DoSomethingWithDoneShip(ship);

				ResetRow(i);
				SetRow(model.GetRows()[i]);
			}
		}

		TravesiaEvent newRandomEvent = model.CheckForRandomEvents();

		if(newRandomEvent != null){
			yield return new WaitForSeconds(1f);
			SetTile(newRandomEvent);
		}

		if(action == TravesiaAction.SEND) model.NewSend();

		Next();
	}

	void SetRow(List<TravesiaEvent> rowEvents) {
		foreach(TravesiaEvent rowEvent in rowEvents) {
			SetTile(rowEvent);
		}
	}

	void SetTile(TravesiaEvent rowEvent) {
		int slot = model.GetSlot(rowEvent.row, rowEvent.col);

		switch(rowEvent.GetState()) {
		case TravesiaEventState.SHIP:
			tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + (rowEvent.isGoingLeft ? 0 : 1)];
			clocks[slot].gameObject.SetActive(true);
			clocks[slot].GetComponentInChildren<Text>().text = rowEvent.GetProvisions().ToString();
			break;
		case TravesiaEventState.WRECKED_SHIP:
			tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + 2];
			clocks[slot].gameObject.SetActive(true);
			clocks[slot].GetComponentInChildren<Text>().text = rowEvent.GetProvisions().ToString();
			break;
		case TravesiaEventState.SUNK_SHIP:
			tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + 3];
			break;
		case TravesiaEventState.MONSTER:
			tiles[slot].sprite = tileSprites[START_MONSTER_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_MONSTER_SPRITES)];
			break;
		case TravesiaEventState.DEAD_MONSTER:
			tiles[slot].sprite = tileSprites[START_MONSTER_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_MONSTER_SPRITES) + 1];
			break;
		}
	}

	void DoSomethingWithDoneShip(TravesiaEvent doneShip) {
		if(doneShip.isShipCorrect){
			Debug.Log("Correct ship arrived.");
			//TODO correct.
			model.Correct();
		} else {
			Debug.Log("Wrong ship arrived.");
			//TODO wrong.
			model.Wrong();
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
			ResetRow(row);
		}
	}

	void ResetRow(int rowNumber) {
		for (int col = 1; col < TravesiaActivityModel.GRID_COLS - 1; col++) {
			tiles[model.GetSlot(rowNumber, col)].sprite = tileSprites[WATER_SPRITE];
			clocks[model.GetSlot(rowNumber, col)].gameObject.SetActive(false);
		}
	}

	public void Start(){
		model = new TravesiaActivityModel();
		tileSprites = Resources.LoadAll<Sprite>("Sprites/TravesiaActivity/tiles");
		ResetTiles();
		ClocksActive(false);
		Begin();
	}

	public void Begin(){
		ShowExplanation();
	}

	Sprite GetWreckedShip(Sprite sprite) {
		return tileSprites[Array.IndexOf(tileSprites, sprite) + 1];
	}

	Sprite GetDestroyedShip(Sprite sprite) {
		return tileSprites[Array.IndexOf(tileSprites, sprite) + 2];
	}

	public void LetterClick(string l){
		if(keyboardActive) {
			PlaySoundClick();
			letter.text = l;
			CheckOk();
		}
	}

	public void NumberClick(string n){
		if(keyboardActive) {
			PlaySoundClick();
			number.text = n;
			CheckOk();
		}
	}

	public void CheckOk() {
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

	//LOG:

	public GameObject logPanel;

	public void OpenLog(){
		logPanel.SetActive(true);

		PopulateLog();
	}

	void PopulateLog() {
		List<TravesiaEvent> doneShips = model.GetDoneEvents();
		List<TravesiaEvent> onGoingShips = model.GetOnGoingShipEvents();

		//TODO populate log with this events.
	}

	public void CloseLog(){
		logPanel.SetActive(false);
	}
}