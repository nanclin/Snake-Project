using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Door : Initializer {

	public List<State> states;

	private int numOfStatesTrue;
	private bool opened;

	void Awake()
	{
		foreach( State state in states )
		{
			state.OnStateTrue += OnStateTrue;
			state.OnStateFalse += OnStateFalse;
		}
	}

	override public void Init()
	{
		print("Door Initialized");
		states = new List<State>();
		numOfStatesTrue = 0;
		opened = false;
	}

	private void OnStateTrue()
	{
		numOfStatesTrue++;
		if( !opened && numOfStatesTrue == states.Count ){
			print( "door is open" );
			opened = true;
			transform.position = transform.position + Vector3.up * 5;
		}
	}

	private void OnStateFalse()
	{
		// numOfStatesTrue--;
		// if( opened && numOfStatesTrue < states.Count ){
		// 	print( "door is closed" );
		// 	opened = false;
		// 	transform.position = transform.position - Vector3.up * 5;
		// }
	}
}
