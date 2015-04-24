﻿using UnityEngine;
using System.Collections;

public class SnakeController : MonoBehaviour {

	// Static
	private static bool FSM_DEBUG = false;

	// Handling
	private float maxSpeed = 0.08f;
	private float acceleration = 0.01f;
	private float rotationSpeed = 2;
	public enum ControlType{ Debug, PC, Android }
	public ControlType controlType = ControlType.Debug;
	public float buffer = 0.5f;
	public float bondStrength = 0.2f;

	// Components
	private SnakeBody body;

	// System
	public enum State { Idle, Move, Shrink, ToHole, Die, Dead }
	private State _state;
	private float speed = 0;


// BUILTIN METHODS ///////////////////////////////////////////////////////////////

	// Use this for initialization
	void Start ()
	{
		body = GetComponent<SnakeBody>();

		// Grow cells
		for( int i = 0; i < 2; i++ )
		{	
			body.Grow();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch( controlType )
		{
			case ControlType.Debug: DebugInput(); break;
			case ControlType.PC: PCInput(); break;
			// case ControlType.Android: AndroidInput(); break;
		}
	}

	void FixedUpdate ()
	{
		ExecuteState();
	}


	void OnTriggerEnter( Collider other )
	{
		switch( other.tag )
		{
			case "Item":
				for( int i = 0; i < other.GetComponent<Item>().nutritionValue; i++ )
					body.Grow();
				break;
			default:
				print("SnakeHeadCollider hit something not handeld by code!");
				break;
		}
	}
//////////////////////////////////////////////////////////// EO BUILTIN METHODS //


// FSM MACHINE METHODS ////////////////////////////////////////////////////////

	// Set currentState to transition to new state
	private State currentState
	{
		get { return _state; }
		set {
			// If current state is set,
			// run exit state code
			if( _state != null )
				ExitState( _state );

			// Set new current state value
			_state = value;

			// Run enter state code
			EnterState( _state );
		}
	}

	private void EnterState( State state )
	{
		// print("EnterState: " + state);
		switch( state )
		{
			case State.Idle:
				IdleEnterState();
				break;
			case State.Move:
				MoveEnterState();
				break;
			case State.Shrink:
				ShrinkEnterState();
				break;
		}
	}

	private void ExecuteState()
	{
		switch( currentState )
		{
			case State.Idle:
				IdleState();
				break;
			case State.Move:
				MoveState();
				break;
			case State.Shrink:
				ShrinkState();
				break;
		}
	}

	private void ExitState( State state )
	{
		switch( state )
		{
			case State.Idle:
				IdleExitState();
				break;
			case State.Move:
				MoveExitState();
				break;
			case State.Shrink:
				ShrinkExitState();
				break;
		}
	}

	private void DebugEnter  ( string state ){ if( FSM_DEBUG ) print( "\t-->( \t" + state + "\t )"         	); }
	private void DebugExecute( string state ){ if( FSM_DEBUG ) print( "\t   ( \t" + state + "\t )"	); }
	private void DebugExit   ( string state ){ if( FSM_DEBUG ) print( "\t   ( \t" + state + "\t )-->"     	); }
//////////////////////////////////////////////////////// EO FSM MACHINE METHODS //


// STATE METHODS ////////////////////////////////////////////////////////

// IDLE STATE //
	private void IdleEnterState()
	{
		DebugEnter( "Idle" );
	}

	private void IdleState()
	{
		DebugExecute( "Idle" );

		if( Time.time > 0 )
			currentState = State.Move;
	}

	private void IdleExitState()
	{
		DebugExit( "Idle" );
	}
// EO IDLE STATE //


// MOVE STATE //
	private void MoveEnterState()
	{
		DebugEnter( "Move" );
	}

	void OnGUI ()
	{
		if( controlType == ControlType.Android )
			AndroidInput();
	}

	private void MoveState()
	{
		DebugExecute( "Move" );

		// Set boost
		float boost = boostInput * 0.1f;
		float boostRotation = Mathf.Max( 0, boostInput * 1 );

		// Translate snake

		// Calculate position
		float positionChange = speed;
		transform.Translate( Vector3.forward * positionChange );
		// Calculate speed
		speed += acceleration;
		speed = Mathf.Min( speed, maxSpeed + boost );
		speed = Mathf.Max( speed, 0 );
		// Store current positon point
		body.skeleton.PrependJoint( transform.position );

		// // Rotate and translate snakes head
		transform.Rotate( Vector3.up * rotationInput * ( rotationSpeed + boostRotation ) );

		//
		body.UpdateBody( positionChange );
	}

	private void MoveExitState()
	{
		DebugExit( "Move" );
		// if( FSM_DEBUG ) print("FSM -> MoveExitState");
	}
// EO MOVE STATE //

// SHRINK STATE //


	private float shrinkNumberOfCells = 2;
	private float shrinkPosition;

	private ChainNode head;
	private ChainNode targetNode;

	private void ShrinkEnterState()
	{
		DebugEnter( "Shrink" );



		shrinkPosition = 0;


		head = body.chain.head;
		targetNode = head.next.next;


		// string trace = "";
		// ChainNode node = body.chain.tail;
		// while( node != null ){


		// 	node.value = ( node.value - head.value );
		// 	trace += "node: " + node.value + "\n";

		// 	node = node.previous;
		// }
		// // print( trace );



	}

	private void ShrinkState()
	{
		DebugExecute( "Shrink" );


		// Move head along skeleton - from head to tail
		float dis = Mathf.Abs( targetNode.value - head.value );
		body.MoveChain( head, -dis/10 );
		body.PutOnSkeleton( transform, body.zero - head.value );
		// body.PutOnSkeleton( transform, -head.value );

		// Destroy cells on path
		if( head.Distance < 0.1f ){
			body.DestroyCell( head.next );
		}

		// If arrived at target position,
		// trim skeleton and reset zero value to heads value
		if( dis < 0.1f )
		{
			print("end of shrinking");
			body.skeleton.ShaveStart( body.zero - head.value );
			body.zero = head.value;

			// Switch state
			currentState = State.Move;
		}
	}

	private void ShrinkExitState()
	{
		DebugExit( "Shrink" );
	}
// EO SHRINK STATE //

//////////////////////////////////////////////////////// EO STATE METHODS //

// INPUTS ////////////////////////////////////////////////////////

	private float rotationInput = 0;
	private float boostInput = 0;

	private bool shrinkFlag;

	private void DebugInput ()
	{
		speed = 0.5f;

		rotationInput = 0;
		boostInput = 0;

		// if( Time.frameCount == 7 )
		// 	currentState = State.Shrink;

		if( Input.GetKeyDown("space") )
			body.Grow();

		if( Input.GetKeyDown("backspace") )
			currentState = State.Shrink;

		// if( transform.position.x >= 5 && !shrinkFlag ){
		// 	currentState = State.Shrink;
		// 	shrinkFlag = true;
		// }
	}

	private void PCInput ()
	{
		rotationInput = Input.GetAxis("Horizontal");
		boostInput = Input.GetAxis("Vertical");


		if( Input.GetKeyDown("space") )
			body.Grow();

		if( Input.GetKeyDown("backspace") )
			currentState = State.Shrink;

		if( Input.GetKeyDown("5") )
			body.Grow(5);
	}

	private float boostTime = 0;

	private void AndroidInput ()
	{
		// Draw buttons
		float size = 150;
		float padding = 30;
		bool left = GUI.RepeatButton( new Rect( padding, Screen.height - size - padding, size, size), "<" );
		bool right = GUI.RepeatButton( new Rect( Screen.width - size - padding, Screen.height - size - padding, size, size), ">" );

		if( left ){
			rotationInput = -1;
		}
		else if ( right ) {
			rotationInput = 1;
		}
		else {
			rotationInput = 0;
		}

		if( GUI.Button( new Rect( Screen.width/2 - size*2/2 + size + padding, Screen.height - size - padding, size*2, size), "BOOST" ) ){
			// Add x seconds to boostTime
			boostTime = Mathf.Max( Time.time, boostTime ) + 2;
		}
		boostInput = ( boostTime > Time.time ) ? 1 : 0;

		if( GUI.Button( new Rect( Screen.width/2 - size*2/2 - size - padding, Screen.height - size - padding, size*2, size), "GROW" ) ){
			body.Grow();
		}
	}
//////////////////////////////////////////////////////// EO INPUTS //
}
