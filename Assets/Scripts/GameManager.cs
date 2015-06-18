using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	// Global
	public static GameManager INSTANCE;
	public static int ROUND_DECIMALS = 5;
	public static int SCORE;
	public static int STARS;
	public static float TIME;
	public static float START_TIME;

	// Static
	public static bool FSM_DEBUG = true;
	
	// System
	public enum GameState { Idle, Loading, SplashScreen, MapScreen, NewGame, Run, GameOver, OpenExit, LevelFinished }
	private GameState _state;
	private float sw = Screen.width;
	private float sh = Screen.height;
	private float bw = 150;
	private float bh = 100;

	// Components
	// public Transform cameraHolder;
	[HideInInspector] public SnakeController snake;
	[HideInInspector] public GameObject exit;

// UNITY METHODS ///////////////////////////////////////////////////////////////

	void Awake()
	{
		DontDestroyOnLoad( transform.gameObject );

		if( INSTANCE == null ){
			INSTANCE = this;
		}
	}

	// Use this for initialization
	void Start ()
	{
		// LoadLevel( "Level 01" );

		currentState = GameState.SplashScreen;
		// currentState = GameState.NewGame;
		// currentState = GameState.Run;
	}

	void Update()
	{
		if( Input.GetKeyDown("o") )
			currentState = GameState.OpenExit;
	}

	void FixedUpdate ()
	{
		ExecuteState();
	}

	void OnGUI ()
	{
		GUI.Box( new Rect( 0, 0, 100, Screen.width/3 ), currentState.ToString() );

		// SPLASH SCREEN GUI ///////////////////////////////////////////////////////////////
		if( currentState == GameState.SplashScreen )
		{
			GUI.Box( new Rect( 0, 0, sw, sh ), "Snake Project\n2015\nNejc Anclin" );

			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "Start" ) ){
				LoadLevel( "Map Screen" );
			}
		}
		//////////////////////////////////////////////////////////// EO SPLASH SCREEN GUI //

		// MAP SCREEN GUI ///////////////////////////////////////////////////////////////
		if( currentState == GameState.MapScreen )
		{
			GUI.Box( new Rect( 0, 0, sw, sh ), "Map Screen" );

			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh / 2 ), "Level 01" ) ){
				LoadLevel( "Level 01" );
			}
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2 + 100, bw, bh / 2 ), "Level 02" ) ){
				LoadLevel( "Level 02" );
			}
		}
		//////////////////////////////////////////////////////////// EO MAP SCREEN GUI //

		// NEW GAME GUI ///////////////////////////////////////////////////////////////
		if( currentState == GameState.NewGame )
		{
			GUI.Box( new Rect( 0, 0, sw, sh ), "Snake Project\nMay 2015\nNejc Anclin" );

			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh ), "Begin Game" ) ){
				currentState = GameState.Run;
			}
		}
		//////////////////////////////////////////////////////////// EO NEW GAME GUI //
		
		// GAME OVER GUI ///////////////////////////////////////////////////////////////
		else if( currentState == GameState.GameOver )
		{	
			GUI.Box( new Rect( 0, 0, sw, sh ), "\n\n\n\nGame Over" );

			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh / 2 ), "Restart" ) ){
				currentState = GameState.NewGame;
			}
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2 + 100, bw, bh / 2 ), "Quit" ) ){
				LoadLevel( "Map Screen" );
			}
		}
		//////////////////////////////////////////////////////////// EO GAME OVER GUI //

		// GAME RUNNING GUI ///////////////////////////////////////////////////////////////
		if( currentState == GameState.Run ||
			currentState == GameState.GameOver ){
			GUI.Box( new Rect( sw/2 - 100/2 - 60, 10, 100, 25 ), "Score: " + SCORE );
			GUI.Box( new Rect( sw/2 - 100/2 + 60, 10, 100, 25 ), "Stars: " + STARS + " / 3" );

			int m = (int)( TIME / 60f );
			int s = (int)( TIME % 60f );
			GUI.Box( new Rect( sw/2 - 100/2 + 180, 10, 100, 25 ), "Time: " + m + ":" + s );
		}
		//////////////////////////////////////////////////////////// EO GAME RUNNING GUI //

		// LEVEL FINISHED GUI ///////////////////////////////////////////////////////////////
		if( currentState == GameState.LevelFinished ){
			GUI.Box( new Rect( 0, 0, sw, sh ), "\n\n\n\nCONGRATULATIONS!\n You finished this level!\nYour score is\n" + SCORE );

			int m = (int)( TIME / 60f );
			int s = (int)( TIME % 60f );
			GUI.Box( new Rect( sw/2 - 100/2 + 180, 10, 100, 25 ), "Time: " + m + ":" + s );
			
			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2, bw, bh / 2 ), "Continue" ) ){
				LoadLevel( "Map Screen" );
			}

			if( GUI.Button( new Rect( sw/2-bw/2, sh/2-bh/2 + 100, bw, bh / 2 ), "Restart" ) ){
				currentState = GameState.NewGame;
			}
		}
		//////////////////////////////////////////////////////////// EO LEVEL FINISHED GUI //
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// FSM MACHINE METHODS ////////////////////////////////////////////////////////

	// Set currentState to transition to new state
	public GameState currentState
	{
		get { return _state; }
		set {
			// GameState should not transition to itself
			if( _state != value )
			{
				// If current state is set,
				// run exit state code
				// if( _state != null )
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
			case GameState.Loading:
				LoadingEnterState();
				break;
			case GameState.MapScreen:
				MapScreenEnterState();
				break;
			case GameState.SplashScreen:
				SplashScreenEnterState();
				break;
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
			case GameState.Loading:
				LoadingState();
				break;
			case GameState.SplashScreen:
				SplashScreenState();
				break;
			case GameState.MapScreen:
				MapScreenState();
				break;
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
			case GameState.Loading:
				LoadingExitState();
				break;
			case GameState.SplashScreen:
				SplashScreenExitState();
				break;
			case GameState.MapScreen:
				MapScreenExitState();
				break;
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

// OTHER METHODS ///////////////////////////////////////////////////////////////

	private void ResetLevel()
	{
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
		if( exit != null )
			exit.SetActive( false );

		// Automatically start running game
		// currentState = GameState.Run;		
	}

	private void LoadLevel( string level )
	{
		currentLevel = level;
		currentState = GameState.Loading;
		// // Load scene
		// Application.LoadLevel( Application.loadedLevel + 1 );		
	}
//////////////////////////////////////////////////////////// EO OTHER METHODS //

// STATE METHODS ////////////////////////////////////////////////////////


// LOADING STATE //

	private AsyncOperation async;
	private string currentLevel;

	private void LoadingEnterState()
	{
		DebugEnter( "Loading" );

		async = Application.LoadLevelAsync( currentLevel );
	}

	private void LoadingState()
	{
		DebugExecute( "Loading" );

		if( async.isDone ){
			switch( currentLevel )
			{
				case "Splash Screen":
					break;
				case "Map Screen":
					currentState = GameState.MapScreen;
					break;
				default:

					// SET LEVEL REFERENCES ///////////////////////////////////////////////////////////////
					snake = GameObject.Find("Player Snake").GetComponent<SnakeController>();
					exit = GameObject.FindWithTag("Exit");
					//////////////////////////////////////////////////////////// EO SET LEVEL REFERENCES //

					currentState = GameState.NewGame;
					break;
			}
		}
	}

	private void LoadingExitState()
	{
		DebugExit( "Loading" );
	}
// EO LOADING STATE //

// SPLASH SCREEN STATE //
	private void SplashScreenEnterState()
	{
		DebugEnter( "SplashScreen" );
	}

	private void SplashScreenState()
	{
		DebugExecute( "SplashScreen" );
	}

	private void SplashScreenExitState()
	{
		DebugExit( "SplashScreen" );
	}
// EO SPLASH SCREEN STATE //

// MAP SCREEN STATE //
	private void MapScreenEnterState()
	{
		DebugEnter( "MapScreen" );
	}

	private void MapScreenState()
	{
		DebugExecute( "MapScreen" );
	}

	private void MapScreenExitState()
	{
		DebugExit( "MapScreen" );
	}
// EO MAP SCREEN STATE //

// NEW GAME STATE //
	private void NewGameEnterState()
	{
		DebugEnter( "NewGame" );
		ResetLevel();
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
		if( exit != null )
			exit.SetActive( true );
		else
			Debug.LogError("Exit missing!");
	}

	private void OpenExitState()
	{
		DebugExecute( "OpenExit" );


		float timeElapsed = Time.time - openTime;
		if( timeElapsed >= 0.3f )
			currentState = GameState.Run;
		// else if( timeElapsed >= 4 )
		// 	cameraHolder.GetComponent<FollowTarget>().target = snake.transform;
		// else if( timeElapsed >= 1 )
		// 	cameraHolder.GetComponent<FollowTarget>().target = exit.transform;

	}

	private void OpenExitExitState()
	{
		DebugExit( "OpenExit" );

		snake.currentState = SnakeState.Move;
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
// EO LEVEL FINISHED STATE //

//////////////////////////////////////////////////////// EO STATE METHODS //
}
