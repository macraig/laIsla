using System;
using Assets.Scripts.Metrics;

public class TravesiaTile {
	private int timer;
	private TravesiaState state;
	private int timeToAppear, timeToMole;

	public TravesiaTile(){
		state = TravesiaState.FREE;
		timer = 0;
		timeToAppear = 0;
		timeToMole = 0;
	}

	public void DecreaseTimer(){
		if(timer != 0) timer--;
		if(timeToAppear != 0) timeToAppear--;
		if(timeToMole != 0) timeToMole--;
	}

	public bool HasToAppear() {
		return state == TravesiaState.TO_APPEAR_VEGGIE && timeToAppear == 0;
	}

	public void AppearVeggie() {
		if(state == TravesiaState.TO_APPEAR_VEGGIE) {
			state = TravesiaState.VEGGIE;
			timeToMole = TravesiaActivityModel.VEGETABLE_TO_MOLE;
		}
	}

	public void AppearInitVeggie() {
		state = TravesiaState.VEGGIE;
		timeToMole = TravesiaActivityModel.VEGETABLE_TO_MOLE;
	}

	public bool IsCorrect() {
		if(state == TravesiaState.MOLE){
			state = TravesiaState.SMACKED_MOLE;
			return true;
		}
		return false;
	}

	public bool TimeDoneAndNotSmacked() {
		return state == TravesiaState.MOLE && timer == 0;
	}

	public void DissapearMole() {
		if(state == TravesiaState.MOLE) state = TravesiaState.FREE;
	}

	public bool MoleHasToAppear() {
		return state == TravesiaState.VEGGIE && timeToMole == 0;
	}

	public void AppearMole() {
		if(state == TravesiaState.VEGGIE) {
			state = TravesiaState.MOLE;
			timer = TravesiaActivityModel.MOLE_TIME;
		}
	}

	public void SetTimeToAppear(int timeToAppear){
		this.timeToAppear = timeToAppear;

		if(timeToAppear != 0) state = TravesiaState.TO_APPEAR_VEGGIE;
	}

	public TravesiaState GetState() {
		return state;
	}

	public int GetTimer(){
		return timer;
	}

	public void SetTimer(int timer){
		this.timer = timer;
	}

	public void SetState(TravesiaState state){
		this.state = state;
	}
}