using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

// // SINGLETON ///////////////////////////////////////////////////////////////
// 	private static GameManager _instance;

// 	public static GameManager Instance{
// 		get
// 		{
// 			if( Instance == null )
// 			{
// 				_instance = Object.FindObjectOfType( typeof( GameManager ) ) as GameManager;

// 				if (_instance == null)
// 				{
// 					GameObject go = new GameObject( "Game Manager" );
// 					DontDestroyOnLoad( go );
// 					_instance = go.AddComponent<GameManager>();
// 				}
// 			}
// 			return _instance;
// 		}
// 	}
// //////////////////////////////////////////////////////////// EO SINGLETON //

	// Global
	public static int ROUND_DECIMALS = 5;
	public static int SCORE;
	public static int STARS;
	public static float TIME;
	public static float START_TIME;

	// Static
	public static bool FSM_DEBUG = false;

	// Debug
	public Transform cameraHolder;

	// System
	public Transform snakeSpawnPoint;
	public GameObject exit;
	public enum GameState { Idle, NewGame, Run, GameOver, OpenExit, LevelFinished }
	private GameState _state;
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
		currentState = GameState.NewGame;
		currentState = GameState.Run;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		ExecuteState();
	}

	void OnGUI ()
	{
		if( currentState == GameState.NewGame ){
			GUI.Box( new Rect( 0, 0, sw, sh ), "Snake Project\nMay 2015\nNejc Anclin" );
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "Begin Game" ) ){
				currentState = GameState.Run;
			}
		}
		else if( currentState == GameState.GameOver )
		{	
			GUI.Box( new Rect( 0, 0, sw, sh ), "\n\n\n\nGame Over" );
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "New Game " ) ){
				currentState = GameState.NewGame;
			}
		}

		if( currentState == GameState.Run ||
			currentState == GameState.GameOver ){
			GUI.Box( new Rect( sw/2 - 100/2 - 60, 10, 100, 25 ), "Score: " + SCORE );
			GUI.Box( new Rect( sw/2 - 100/2 + 60, 10, 100, 25 ), "Stars: " + STARS + " / 3" );

			int m = (int)( TIME / 60f );
			int s = (int)( TIME % 60f );
			GUI.Box( new Rect( sw/2 - 100/2 + 180, 10, 100, 25 ), "Time: " + m + ":" + s );
		}

		if( currentState == GameState.LevelFinished ){
			GUI.Box( new Rect( 0, 0, sw, sh ), "\n\n\n\nCONGRATULATIONS!\n You finished this level!\nYour score is\n" + SCORE );

			int m = (int)( TIME / 60f );
			int s = (int)( TIME % 60f );
			GUI.Box( new Rect( sw/2 - 100/2 + 180, 10, 100, 25 ), "Time: " + m + ":" + s );
			
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "New Game " ) ){
				currentState = GameState.NewGame;
			}
		}
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// FSM MACHINE METHODS ////////////////////////////////////////////////////////

	// Set currentState to transition to new state
	private GameState currentState
	{
		get { return _state; }
		set {
			// GameState should not transition to itself
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

	private void EnterState( GameState state )
	{
		// print("EnterState: " + state);
		switch( state )
		{
			case GameState.NewGame:
				NewGameEnterState();
				break;
			case GameState.Run:
				RunEnterState();
				break;
			case GameState.GameOver:
				GameOverEnterState();
				break;
			case GameState.LevelFinished:
				LevelFinishedEnterState();
				break;
			case GameState.OpenExit:
				OpenExitEnterState();
				break;
		}
	}

	private void ExecuteState()
	{
		switch( currentState )
		{
			case GameState.NewGame:
				NewGameState();
				break;
			case GameState.Run:
				RunState();
				break;
			case GameState.GameOver:
				GameOverState();
				break;
			case GameState.LevelFinished:
				LevelFinishedState();
				break;
			case GameState.OpenExit:
				OpenExitState();
				break;
		}
	}

	private void ExitState( GameState state )
	{
		switch( state )
		{
			case GameState.NewGame:
				NewGameExitState();
				break;
			case GameState.Run:
				RunExitState();
				break;
			case GameState.GameOver:
				GameOverExitState();
				break;
			case GameState.LevelFinished:
				LevelFinishedExitState();
				break;
			case GameState.OpenExit:
				OpenExitExitState();
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
		GameManager.TIME = 0;
		GameManager.START_TIME = Time.time;
		
		// Reset initializers
		Initializer.ResetAll();
		
		// Reset spawners
		// Spawner.ResetSpawners();
		

		// Reset all snakes states
		foreach( SnakeController snakeController in SnakeController.POOL ){
			snakeController.SpawnSnake();
			snakeController.currentState = SnakeState.Idle;
		}

		// Reset camera
		// cameraHolder.transform.position = snakeSpawnPoint.position;

		// Reset exit
		if( exit )
			exit.SetActive( false );

		// Automatically start running game
		// currentState = GameState.Run;
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

		// Change all snakes states to move
		foreach( SnakeController snakeController in SnakeController.POOL ){
			snakeController.currentState = SnakeState.Move;
		}
	}

	private void RunState()
	{
		DebugExecute( "Run" );

		TIME = (int)( Time.time - START_TIME );
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
		currentState = GameState.GameOver;
	}
// EO GAME OVER STATE //

// OPEN EXIT STATE //
	private float openTime;
	private void OpenExitEnterState()
	{
		DebugEnter( "OpenExit" );

		openTime = Time.time;

		// Set snake still
		snake.currentState = SnakeState.Idle;

		// Enable exit
		exit.SetActive( true );
	}

	private void OpenExitState()
	{
		DebugExecute( "OpenExit" );


		float timeElapsed = Time.time - openTime;
		if( timeElapsed >= 5 )
			currentState = GameState.Run;
		else if( timeElapsed >= 4 )
			cameraHolder.GetComponent<FollowTarget>().target = snake.transform;
		else if( timeElapsed >= 1 )
			cameraHolder.GetComponent<FollowTarget>().target = exit.transform;

	}

	private void OpenExitExitState()
	{
		DebugExit( "OpenExit" );

		snake.currentState = SnakeState.Move;
	}
	public void OpenExit()
	{
		// print("EXIT IS NOW OPEN!");
		currentState = GameState.OpenExit;
	}
// EO OPEN EXIT STATE //

// LEVEL FINISHED STATE //
	private void LevelFinishedEnterState()
	{
		DebugEnter( "LevelFinished" );

		snake.currentState = SnakeState.Idle;
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
		currentState = GameState.LevelFinished;
	}
// EO LEVEL FINISHED STATE //

//////////////////////////////////////////////////////// EO STATE METHODS //
}
