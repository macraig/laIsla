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
using Assets.Scripts.App;

public class TravesiaActivityView : LevelView {
	public Button okBtn;
	public List<Image> tiles, clocks;
	public Text letter, number, board;
	public List<Toggle> actions;
	private int setSailRow;
	private Sprite[] tileSprites,shieldSprites,logSprites;
	private AudioClip shipSails, shipArrives, shipSailing, shipBreaks, shipRepair, krakenAppears, krakenDead, 
	snakeAppears, snakeDead, eatProvisions, cannonSplash, cannonHitsShip;
	private bool keyboardActive;
	private const int WATER_SPRITE = 0, START_MONSTER_SPRITES = 1, START_SHIP_SPRITES = 5, DIFFERENT_SHIP_SPRITES = 5, DIFFERENT_MONSTER_SPRITES = 2,
	START_PORT_SPRITES = 20;
	public Text shipCounter;
	private int correctShips;
	public GameObject timerPlaca;
	public Image instructionsImage;

	private TravesiaActivityModel model;

	override public void Next(bool first = false){
		if (first) {
			model.NewSend();
			timerPlaca.SetActive (false);
			correctShips = 0;
			shipCounter.text = correctShips.ToString ();;
		}

		ResetLetterNumber();
		ResetActions();
		keyboardActive = true;
		EnableActions(true);
		CheckOk();
		setSailRow = -1;
		if (model.CheckSend ()) {
			SetSend (model.SendCol (), model.SendRow ());
		} else {
			HideInstructions ();
		}
			

		if(model.GameEnded()) {
			EndGame(60, 0, 1250);
		}
	}

	public void Start(){
		model = new TravesiaActivityModel();
		tileSprites = Resources.LoadAll<Sprite>("Sprites/TravesiaActivity/tiles");
		shieldSprites =Resources.LoadAll<Sprite>("Sprites/TravesiaActivity/escudos");
		logSprites = Resources.LoadAll<Sprite>("Sprites/TravesiaActivity/logOptions");

		shipSails = Resources.Load<AudioClip> ("Audio/TravesiasActivity/shipSails");
		shipArrives = Resources.Load<AudioClip> ("Audio/TravesiasActivity/shipArrives");
		shipBreaks = Resources.Load<AudioClip> ("Audio/TravesiasActivity/shipBreaks");
		shipSailing = Resources.Load<AudioClip> ("Audio/TravesiasActivity/shipsSailing");
		shipRepair = Resources.Load<AudioClip> ("Audio/TravesiasActivity/repairBoat");
		krakenAppears = Resources.Load<AudioClip> ("Audio/TravesiasActivity/krakenAppears");
		krakenDead = Resources.Load<AudioClip> ("Audio/TravesiasActivity/cannonBallKraken");
		snakeAppears = Resources.Load<AudioClip> ("Audio/TravesiasActivity/waterSnake");
		snakeDead = Resources.Load<AudioClip> ("Audio/TravesiasActivity/cannonBallSnake");
		eatProvisions = Resources.Load<AudioClip> ("Audio/TravesiasActivity/provisionsMunch");
		cannonSplash = Resources.Load<AudioClip> ("Audio/TravesiasActivity/CannonBallSplash");
		cannonHitsShip = Resources.Load<AudioClip> ("Audio/TravesiasActivity/cannonHitsShip");

		ResetTiles();
		ClocksActive(false);
		Begin();
	}

	public void Begin(){
		ShowExplanation();
	}

	void EnableActions(bool enabled) {
		actions.ForEach(a => a.enabled = enabled);
	}

	void SetSend(int col, int row) {
		if(col == -1 && row == -1) board.text = "No hay envíos disponibles.";
		else {
			board.text = "Enviar provisiones al puerto ";
			//FOR TESTING PURPOSES
			//board.text = "Enviar provisiones al puerto " + model.GetColumnString(col) + model.GetRowString(row) + ".";
			int shieldIndex = GetPortIndex (row, col) - START_PORT_SPRITES;
			instructionsImage.gameObject.SetActive (true);
			instructionsImage.sprite = shieldSprites[shieldIndex];
		}
	}

	void HideInstructions(){
		board.text = "";
		instructionsImage.gameObject.SetActive (false);
	}

	int GetPortIndex (int row, int col)
	{
		int slot = model.GetSlot (row, col);
		for (int i = START_PORT_SPRITES; i < tileSprites.Length; i++) {
			if (tiles [slot].sprite == tileSprites [i])
				return i;
		}
		return 0;
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
			if (action != TravesiaAction.SEND)
				SetTile (model.GetEvent (row, col), action);
			else
				setSailRow = row;
		} else {
			model.Wrong();
			SoundController.GetController ().PlayClip (cannonSplash);
		}

		//update each row with a two second delay.
		List<List<TravesiaEvent>> rows = model.GetRows();
		for(int i = 0; i < rows.Count; i++) {
			if (!model.IsRowEmpty (i)) {
				yield return new WaitForSeconds (1f);

				List<TravesiaEvent> rowEvents = rows [i];

				model.AdvanceRow (rowEvents);
				model.CleanRow (i);

				TravesiaEvent ship = model.GetAndRemoveDoneShipsFromRow (i);
				if (ship != null)
					DoSomethingWithDoneShip (ship);

				ResetRow (i);
				SetRow (model.GetRows () [i]);
			} 
		}

		TravesiaEvent newRandomEvent = model.CheckForRandomEvents();

		if(newRandomEvent != null){
			yield return new WaitForSeconds(1f);
			SetTile(newRandomEvent,TravesiaAction.SAIL);
		}

		if(action == TravesiaAction.SEND) model.NewSend();

		Next();
	}

	void SetRow(List<TravesiaEvent> rowEvents) {
		foreach(TravesiaEvent rowEvent in rowEvents) {

			SetTile(rowEvent,TravesiaAction.SAIL,true);
		}
	}

	AudioClip GetSoundClip (TravesiaAction action)
	{
		switch (action) {
		case TravesiaAction.PROVISION:
			return eatProvisions;
		case TravesiaAction.REPAIR:
			return shipRepair;
		default:
			return shipSailing;
		}
	}

	void SetTile(TravesiaEvent rowEvent,TravesiaAction action,bool soundSilenced = false) {
		int slot = model.GetSlot(rowEvent.row, rowEvent.col);

		AudioClip currentClip;

		switch(rowEvent.GetState()) {
		case TravesiaEventState.SHIP:
			 currentClip = (rowEvent.row == setSailRow) ? shipSails : GetSoundClip (action);
			SoundController.GetController ().PlayClip (currentClip);
			tiles[slot].sprite = ShipSprite(rowEvent);
			clocks[slot].gameObject.SetActive(true);
			clocks[slot].GetComponentInChildren<Text>().text = rowEvent.GetProvisions().ToString();
			break;
		case TravesiaEventState.WRECKED_SHIP:
			if (action == TravesiaAction.PROVISION) {
				SoundController.GetController ().PlayClip (eatProvisions);
			} else {
				if(!soundSilenced)SoundController.GetController ().PlayClip (shipBreaks);
			}
			tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + 2+(rowEvent.isGoingLeft ? 1 : 0)];
			clocks[slot].gameObject.SetActive(true);
			clocks[slot].GetComponentInChildren<Text>().text = rowEvent.GetProvisions().ToString();
			break;
		case TravesiaEventState.SUNK_SHIP:
			SoundController.GetController ().PlayClip (cannonHitsShip);
			tiles[slot].sprite = tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + 4];
			break;
		case TravesiaEventState.MONSTER:
			int monsterIndex = rowEvent.GetObjectNumber () * DIFFERENT_MONSTER_SPRITES;
			if(!soundSilenced) SoundController.GetController ().PlayClip ((monsterIndex==0)? krakenAppears : snakeAppears);
			tiles[slot].sprite = tileSprites[START_MONSTER_SPRITES + monsterIndex];
			break;
		case TravesiaEventState.DEAD_MONSTER:
			int deadMonsterIndex = rowEvent.GetObjectNumber () * DIFFERENT_MONSTER_SPRITES;
			SoundController.GetController ().PlayClip ((deadMonsterIndex==0)? krakenDead : snakeDead);
			tiles [slot].sprite = tileSprites [START_MONSTER_SPRITES + deadMonsterIndex + 1];

			break;
		}
	}

	Sprite ShipSprite(TravesiaEvent rowEvent) {
		return tileSprites[START_SHIP_SPRITES + (rowEvent.GetObjectNumber() * DIFFERENT_SHIP_SPRITES) + (rowEvent.isGoingLeft ? 1 : 0)];
	}

	void DoSomethingWithDoneShip(TravesiaEvent doneShip) {
		if(doneShip.isShipCorrect){
			Debug.Log("Correct ship arrived.");
			SoundController.GetController ().PlayClip (shipArrives);
			correctShips++;
			shipCounter.text = correctShips.ToString ();
			model.Correct();
		} else {
			Debug.Log("Wrong ship arrived.");
			SoundController.GetController ().PlayFailureSound ();
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
//		SoundController.GetController ().PlayDropSound ();
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

	public GameObject logPanel, slotsPanel;

	public void OpenLog(){
		logPanel.SetActive(true);

		PopulateLog();
	}

	void PopulateLog() {
		List<TravesiaEvent> doneShips = model.GetDoneEvents();
		List<TravesiaEvent> onGoingShips = model.GetOnGoingShipEvents();

		CleanLog();

		foreach(TravesiaEvent ship in onGoingShips) {
			AddLog(ship, true);
		}

		foreach(TravesiaEvent ship in doneShips) {
			AddLog(ship, false);
		}
	}

	void CleanLog() {
		foreach (Transform child in slotsPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	void AddLog(TravesiaEvent ship, bool onGoing) {
		GameObject logEvent = Instantiate(ViewController.GetController().LoadPrefab("Games/TravesiasActivity/logSlot"));


		ViewController.GetController().FitObjectTo(logEvent, slotsPanel);

		//sprite de escudo
		int shieldIndex =  GetPortIndex (ship.row, ship.isGoingLeft ? 0 : 7) - START_PORT_SPRITES;
		logEvent.transform.GetChild(1).GetComponent<Image>().sprite = shieldSprites[shieldIndex];

		//sprite de bien, mal o en camino
		if (onGoing) {
			logEvent.transform.GetChild(2).GetComponent<Image>().sprite = logSprites [ship.GetObjectNumber()+3];
		} else {
			logEvent.transform.GetChild (2).GetComponent<Image> ().sprite = (ship.isShipCorrect ) ? logSprites [1] : logSprites [2];
		}
	}
	public void CloseLog(){
		logPanel.SetActive(false);
	}

	public void ClearSlot(Button button){
		SoundController.GetController ().PlayClickSound ();
		button.GetComponentInChildren<Text> ().text="";
	}
}