using UnityEngine;
using System.Collections;

public class State : Initializer {

	public bool state = false;

	// Event Handler
	public delegate void OnStateTrueEvent();
	public delegate void OnStateFalseEvent();
	public event OnStateTrueEvent OnStateTrue;
	public event OnStateFalseEvent OnStateFalse;

	void Awake()
	{
		Init();
	}

	override public void Init()
	{
		StateTrue();
	}


	public void StateTrue()
	{
		state = true;
		OnStateTrue();

	}
	public void StateFalse()
	{
		state = false;
		OnStateFalse();
	}
}
