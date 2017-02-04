using System;
using Assets.Scripts.Metrics;

public class PlagaTile {
	private int timer;
	private PlagaState state;
	private int timeToAppear, timeToMole;

	public PlagaTile(){
		state = PlagaState.FREE;
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
		return state == PlagaState.TO_APPEAR_VEGGIE && timeToAppear == 0;
	}

	public void AppearVeggie() {
		if(state == PlagaState.TO_APPEAR_VEGGIE) {
			state = PlagaState.VEGGIE;
			timeToMole = PlagasActivityModel.VEGETABLE_TO_MOLE;
		}
	}

	public void AppearInitVeggie() {
		state = PlagaState.VEGGIE;
		timeToMole = PlagasActivityModel.VEGETABLE_TO_MOLE;
	}

	public bool IsCorrect() {
		if(state == PlagaState.MOLE){
			state = PlagaState.SMACKED_MOLE;
			return true;
		}
		return false;
	}

	public bool TimeDoneAndNotSmacked() {
		return state == PlagaState.MOLE && timer == 0;
	}

	public void DissapearMole() {
		if(state == PlagaState.MOLE) state = PlagaState.FREE;
	}

	public bool MoleHasToAppear() {
		return state == PlagaState.VEGGIE && timeToMole == 0;
	}

	public void AppearMole() {
		if(state == PlagaState.VEGGIE) {
			state = PlagaState.MOLE;
			timer = PlagasActivityModel.MOLE_TIME;
		}
	}

	public void SetTimeToAppear(int timeToAppear){
		this.timeToAppear = timeToAppear;

		if(timeToAppear != 0) state = PlagaState.TO_APPEAR_VEGGIE;
	}

	public PlagaState GetState() {
		return state;
	}

	public int GetTimer(){
		return timer;
	}

	public void SetTimer(int timer){
		this.timer = timer;
	}

	public void SetState(PlagaState state){
		this.state = state;
	}
}