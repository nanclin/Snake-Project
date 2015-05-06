using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	// Global
	public static int ROUND_DECIMALS = 5;
	public static int SCORE;
	public static int STARS;

	// Static
	public static bool FSM_DEBUG = false;

	// Debug
	public Transform marker;

	// System
	public enum State { Idle, NewGame, Run, GameOver, LevelFinished }
	private State _state;
	private float sw = Screen.width;
	private float sh = Screen.height;
	private float bw = 150;
	private float bh = 100;

	// Components
	public SnakeController snake;

// UNITY METHODS ///////////////////////////////////////////////////////////////

	// Use this for initialization
	void Start ()
	{
		currentState = State.NewGame;
		// currentState = State.Run;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		ExecuteState();
	}

	void OnGUI ()
	{
		if( currentState == State.NewGame ){
			GUI.Box( new Rect( 0, 0, sw, sh ), "Snake Project\nMay 2015\nNejc Anclin" );
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "Begin Game" ) ){
				currentState = State.Run;
			}
		}
		else if( currentState == State.GameOver )
		{	
			GUI.Box( new Rect( 0, 0, sw, sh ), "\n\n\n\nGame Over" );
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "New Game " ) ){
				currentState = State.NewGame;
				snake.RespawnSnake();
				snake.currentState = SnakeState.Idle;
			}
		}

		if( currentState == State.Run ||
			currentState == State.GameOver ){
			GUI.Box( new Rect( sw/2 - 100/2 - 60, 10, 100, 25 ), "Score: " + SCORE );
			GUI.Box( new Rect( sw/2 - 100/2 + 60, 10, 100, 25 ), "Stars: " + STARS + " / 3" );
		}

		if( currentState == State.LevelFinished ){
			GUI.Box( new Rect( 0, 0, sw, sh ), "\n\n\n\nCONGRATULATIONS!\n Tou finished this level!\nYour score is\n" + SCORE );
			snake.currentState = SnakeState.Idle;
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "New Game " ) ){
				snake.RespawnSnake();
				currentState = State.NewGame;
			}
		}
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //
	

// FSM MACHINE METHODS ////////////////////////////////////////////////////////

	// Set currentState to transition to new state
	private State currentState
	{
		get { return _state; }
		set {
			// State should not transition to itself
			if( _state != value )
			{
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
	}

	private void EnterState( State state )
	{
		// print("EnterState: " + state);
		switch( state )
		{
			case State.NewGame:
				NewGameEnterState();
				break;
			case State.Run:
				RunEnterState();
				break;
			case State.GameOver:
				GameOverEnterState();
				break;
			case State.LevelFinished:
				LevelFinishedEnterState();
				break;
		}
	}

	private void ExecuteState()
	{
		switch( currentState )
		{
			case State.NewGame:
				NewGameState();
				break;
			case State.Run:
				RunState();
				break;
			case State.GameOver:
				GameOverState();
				break;
			case State.LevelFinished:
				LevelFinishedState();
				break;
		}
	}

	private void ExitState( State state )
	{
		switch( state )
		{
			case State.NewGame:
				NewGameExitState();
				break;
			case State.Run:
				RunExitState();
				break;
			case State.GameOver:
				GameOverExitState();
				break;
			case State.LevelFinished:
				LevelFinishedExitState();
				break;
		}
	}

	private void DebugEnter  ( string state ){ if( FSM_DEBUG ) print( "GAME: \t-->( \t" + state + "\t )"         	); }
	private void DebugExecute( string state ){ if( FSM_DEBUG ) print( "GAME: \t   ( \t" + state + "\t )"	); }
	private void DebugExit   ( string state ){ if( FSM_DEBUG ) print( "GAME: \t   ( \t" + state + "\t )-->"     	); }
//////////////////////////////////////////////////////// EO FSM MACHINE METHODS //


// STATE METHODS ////////////////////////////////////////////////////////

// NEW GAME STATE //
	private void NewGameEnterState()
	{
		DebugEnter( "NewGame" );

		// Reset score
		GameManager.SCORE = 0;
		GameManager.STARS = 0;
		
		// Reset spawners
		Spawner.ResetSpawners();
	}

	private void NewGameState()
	{
		DebugExecute( "NewGame" );
	}

	private void NewGameExitState()
	{
		DebugExit( "NewGame" );
	}
// EO NEW GAME STATE //


// RUN GAME STATE //
	private void RunEnterState()
	{
		DebugEnter( "Run" );

		snake.currentState = SnakeState.Move;
	}

	private void RunState()
	{
		DebugExecute( "Run" );
	}

	private void RunExitState()
	{
		DebugExit( "Run" );
	}
// EO RUN GAME STATE //


// GAME OVER STATE //
	private void GameOverEnterState()
	{
		DebugEnter( "GameOver" );
	}

	private void GameOverState()
	{
		DebugExecute( "GameOver" );
	}

	private void GameOverExitState()
	{
		DebugExit( "GameOver" );
	}
	public void GameOver()
	{
		currentState = State.GameOver;
	}
// EO GAME OVER STATE //

// LEVEL FINISHED STATE //
	private void LevelFinishedEnterState()
	{
		DebugEnter( "LevelFinished" );
	}

	private void LevelFinishedState()
	{
		DebugExecute( "LevelFinished" );

	}

	private void LevelFinishedExitState()
	{
		DebugExit( "LevelFinished" );
	}
	public void LevelFinished()
	{
		currentState = State.LevelFinished;
	}
// EO LEVEL FINISHED STATE //

//////////////////////////////////////////////////////// EO STATE METHODS //

// OTHER METHODS ///////////////////////////////////////////////////////////////

	public void OpenExit()
	{
		print("EXIT IS NOW OPEN!");
	}
//////////////////////////////////////////////////////////// EO OTHER METHODS //

}
