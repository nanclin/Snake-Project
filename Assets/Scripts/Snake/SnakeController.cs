using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SnakeBody))]
public class SnakeController : Initializer
{
	[System.Serializable]
	public class Settings
	{
		public Color color = Color.green;
		public bool randomSpawnPosition = false;
		[Range( 0, 1 )] public float buffer = 0.55f;
		[Range( 0, 1 )] public float bondStrength = 1f;
		[Range( 0, 10 )] public int lengthOnBorn = 3;
		[Range( 0, 5 )] public float growDelay = 0.5f;
	}

	[System.Serializable]
	public class Handling
	{
		public ControlMode controlMode = SnakeController.ControlMode.Debug;
		[Range( 0, 1f )] public float maxSpeed = 0.08f;
		[Range( 0, 0.01f )] public float acceleration = 0.01f;
		[Range( 0, 90 )] public float rotationSpeed = 3;
		[Range( 0, 10 )] public float inputSensitivity = 5f;
		[Range( 0, 5 )] public float boostDuration = 1;
	}


	// Static
	public static List<SnakeController> POOL = new List<SnakeController>();
	private static bool FSM_DEBUG = false;

	// Enums
	public enum ControlMode{ PC, Android, AI, Debug }
	public enum SteerMode{ None, Wander, Mouse, Item, Checkpoint, Pathfinding }

	// Settings
	// Handling
	public Settings settings = new Settings();
	public Handling handling = new Handling();
	public SnakeAI snakeAI = new SnakeAI();
	
	// Components
	[HideInInspector] public SnakeBody body;

	// System
	[HideInInspector] public Transform respawnPoint;
	private SnakeState _state;

	override public void Init()
	{

	}

// UNITY METHODS ///////////////////////////////////////////////////////////////

	void OnEnable()
	{
		POOL.Add( this );
	}
	void OnDisable()
	{
		POOL.Remove( this );
	}

	// Use this for initialization
	void Awake ()
	{
		// Get body reference
		body = GetComponent<SnakeBody>();

		snakeAI.path = new NavMeshPath();
	}

	void Start()
	{
		// Set respawn point
		respawnPoint = new GameObject("Spawn Point").transform;
		respawnPoint.position = transform.position;
		respawnPoint.rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// boostInput = Input.GetAxisRaw("Vertical");
		// boostInput = Input.GetAxis("Vertical");

		if( currentState != SnakeState.Die )
		switch( handling.controlMode )
		{
			case ControlMode.Debug:   DebugInput();   break;
			case ControlMode.PC:      PCInput();      break;
			case ControlMode.AI:      AIInput();      break;
			case ControlMode.Android: AndroidInput(); break;
		}

		// Debug keybidings
		if( Input.GetKeyDown("enter") )
			body.Grow(1);

		if( Input.GetKeyDown("backspace") )
			currentState = SnakeState.Shrink;

		if( Input.GetKeyDown("5") )
			body.Grow(5);

		if( Input.GetKeyDown("r") )
			currentState = SnakeState.OnRail;

		if( Input.GetKeyDown("k") )
			currentState = SnakeState.Die;

		if( Input.GetKeyDown("i") )
			handling.controlMode = handling.controlMode == ControlMode.PC ? ControlMode.AI : ControlMode.PC;


		// FadeObjectInFrontOfCamera();
	}

	void FixedUpdate ()
	{
		ExecuteState();
	}


	void OnTriggerEnter( Collider other )
	{
		switch( other.tag )
		{
			case "Food":
				for( int i = 0; i < other.GetComponent<Item>().nutritionValue; i++ ){
					body.Grow();
					GameManager.SCORE++;
				}
				// settings.color = RandomColor.GetRandomColor();
				// body.SetColor( settings.color );
				break;

			case "Coin":
				GameManager.SCORE += 10;
				break;

			case "Star":
				GameManager.SCORE += 100;
				GameManager.STARS++;
				if( GameManager.STARS == 3 )
					GameManager.INSTANCE.currentState = GameManager.GameState.OpenExit;
				break;

			case "Snake Cell":
			case "Snake":
				if( currentState != SnakeState.Shrink && currentState != SnakeState.OnRail )
				{
					SnakeBodyCell otherCell = other.GetComponent<SnakeBodyCell>();

					bool otherIsSelfNeck = false;
					try{ otherIsSelfNeck = (GetComponent<SnakeBodyCell>() == otherCell.previous); }
					catch( System.ArgumentException e ){ print(e); }

					// Ignore collisions for neck cell
					if( !otherIsSelfNeck )
					{
						// Eat any kind of tail - enemy tail or self tail
						if( otherCell.isTail )
						{	
							// Both cells are one size long
							if( otherCell.isHead && body.size == 1 ){
								currentState = SnakeState.Shrink;
							}
							else{
								// Destroy hit tail
								otherCell.body.DestroyCell( otherCell );

								body.Grow();
							}
						}
						else if( otherCell.isHead )
						{
							currentState = SnakeState.Shrink;
						}
						else
						{
							currentState = SnakeState.Shrink;
						}
					}
				}
				break;

			case "Ground":
			case "Wall":
					currentState = SnakeState.Shrink;
				break;

			case "Hole":
				if( currentState != SnakeState.OnRail )
				{
					hole = other.GetComponent<Hole>();
					hole.RotatePeriscope( this.transform );
					spline = other.transform.Find("Periscope Spline").GetComponent<BezierSpline>();

					currentState = SnakeState.OnRail;
				}
				break;

			case "Button":
				break;

			default:
				// print("SnakeHeadCollider hit something not handeld by code! " + other);
				break;
		}
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //


// FSM MACHINE METHODS ////////////////////////////////////////////////////////

	// Set currentState to transition to new state
	public SnakeState currentState
	{
		get { return _state; }
		set {
			// SnakeState should not transition to itself
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

	private void EnterState( SnakeState state )
	{
		// print("EnterState: " + state);
		switch( state )
		{
			case SnakeState.Idle:
				IdleEnterState();
				break;
			case SnakeState.Move:
				MoveEnterState();
				break;
			case SnakeState.OnRail:
				OnRailEnterState();
				break;
			case SnakeState.Shrink:
				ShrinkEnterState();
				break;
			case SnakeState.Die:
				DieEnterState();
				break;
		}
	}

	private void ExecuteState()
	{
		switch( currentState )
		{
			case SnakeState.Idle:
				IdleState();
				break;
			case SnakeState.Move:
				MoveState();
				break;
			case SnakeState.OnRail:
				OnRailState();
				break;
			case SnakeState.Shrink:
				ShrinkState();
				break;
			case SnakeState.Die:
				DieState();
				break;
		}
	}

	private void ExitState( SnakeState state )
	{
		switch( state )
		{
			case SnakeState.Idle:
				IdleExitState();
				break;
			case SnakeState.Move:
				MoveExitState();
				break;
			case SnakeState.OnRail:
				OnRailExitState();
				break;
			case SnakeState.Shrink:
				ShrinkExitState();
				break;
			case SnakeState.Die:
				DieExitState();
				break;
		}
	}

	private void DebugEnter  ( string state ){ if( FSM_DEBUG ) print( "SNAKE: \t-->( \t" + state + "\t )"         	); }
	private void DebugExecute( string state ){ if( FSM_DEBUG ) print( "SNAKE: \t   ( \t" + state + "\t )"	); }
	private void DebugExit   ( string state ){ if( FSM_DEBUG ) print( "SNAKE: \t   ( \t" + state + "\t )-->"     	); }
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

		// if( Time.time > 0 )
		// 	currentState = SnakeState.Move;
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
		if( handling.controlMode == ControlMode.Android )
			AndroidInputGUI();

		if( GUI.Button( new Rect(200,0,100,20), "SHRINK" ) )
			currentState = SnakeState.Shrink;
	}

	private float speed = 0;
	private void MoveState()
	{
		DebugExecute( "Move" );

		// BOOST ///////////////////////////////////////////////////////////////
		
		boostOn = ( boostTime > Time.time );

		// Fade boost value
		int boostDir = boostOn ? 1 : 0;
		smoothBoost = Mathf.Lerp( smoothBoost, boostDir, Time.deltaTime * 5 );
		boostInput = smoothBoost;

		// Set boost
		float boost = boostInput * 0.1f;
		float boostRotation = Mathf.Max( 0, boostInput * 1 );
		//////////////////////////////////////////////////////////// EO BOOST //


		// TRANSLATE SNAKE ///////////////////////////////////////////////////////////////
		
		// Calculate position
		float positionChange = speed;
		transform.Translate( Vector3.forward * positionChange );

		// Calculate speed
		speed += handling.acceleration;
		speed = Mathf.Min( speed, handling.maxSpeed + boost );
		speed = Mathf.Max( speed, 0 );

		// Store current positon point
		body.skeleton.PrependJoint( transform.position );
		//////////////////////////////////////////////////////////// EO TRANSLATE SNAKE //

		// Rotate and translate snakes head
		transform.Rotate( Vector3.up * rotationInput * ( handling.rotationSpeed + boostRotation ) );

		// Update body cells
		body.UpdateBody( positionChange );
	}

	private void MoveExitState()
	{
		DebugExit( "Move" );
	}
// EO MOVE STATE //


	[HideInInspector] public BezierSpline spline;
	// private float startTime;
	// private float duration = 5;

	private float traversed;

// ON RAIL STATE //
	private void OnRailEnterState()
	{
		DebugEnter( "OnRail" );

		// startTime = Time.time;
		traversed = 0;

		// // Make ground see trough
		// GameObject ground = GameObject.Find("Ground");
		// if( ground != null ){
		// 	Color color = ground.GetComponent<Renderer>().material.color;
		// 	ground.GetComponent<Renderer>().material.color = new Color( color.r, color.g, color.b, 0.8f );
		// }
	}

	[HideInInspector] public Hole hole;

	private void OnRailState()
	{
		DebugExecute( "OnRail" );

		// Translate snake

		// Slow down boot towards the end of rail
		float boost = 0.3f;
		float slowDownStart = 5;
		float slowDownEnd = 2;
		float distanceToEnd = Mathf.Round( spline.length - traversed );

		if( distanceToEnd < slowDownStart - slowDownEnd && spline.outgoingSpline == null ){
			float zeroToOne =  1 - ( distanceToEnd - slowDownEnd) / slowDownStart;
			boost = Mathf.Lerp( boost, 0, zeroToOne );
		}

		// Calculate speed
		speed += handling.acceleration;
		speed = Mathf.Min( speed, handling.maxSpeed + boost );
		speed = Mathf.Max( speed, 0 );

		traversed += speed;

		// Transition to the next, outgoing spline, if any
		if( traversed > spline.length && spline.outgoingSpline != null )
		{
			traversed = traversed - spline.length;
			spline = spline.outgoingSpline;
			// traversed = spline.length;
			// currentState = SnakeState.Move;
		}

		TransformData data = spline.GetPointBaked( traversed );
		transform.position = data.position;
		transform.rotation = data.rotation;


		// Store current positon point
		body.skeleton.PrependJoint( transform.position );

		//
		body.UpdateBody( speed );


		// When snake comes to an end of the rail
		if( traversed >= spline.length )
		{
			if( hole.type == HoleType.Hole )
				currentState = SnakeState.Move;
			else if( hole.type == HoleType.Exit ){
				currentState = SnakeState.Idle;
				GameManager.INSTANCE.currentState = GameManager.GameState.LevelFinished;
			}
		}
	}

	private void OnRailExitState()
	{
		DebugExit( "OnRail" );

		// // Make ground opaque again
		// GameObject ground = GameObject.Find("Ground");
		// if( ground != null ){
		// 	Color color = ground.GetComponent<Renderer>().material.color;
		// 	ground.GetComponent<Renderer>().material.color = new Color( color.r, color.g, color.b, 1 );
		// }
	}
// EO ON RAIL STATE //

// SHRINK STATE //


	private int shrinkNumberOfCells = 3;

	private SnakeBodyCell targetCell;

	private bool dying;

	private void ShrinkEnterState()
	{
		DebugEnter( "Shrink" );

		// Reset speed
		speed = 0;

		// Set reference to cell to which to srhink
		targetCell = body.cellList[ Mathf.Min( body.cellList.Count - 1, shrinkNumberOfCells ) ];

		// If target node is the last node of the snake,
		// this is last, dying shrink
		dying = ( shrinkNumberOfCells >= body.cellList.Count );
	}

	private void ShrinkState()
	{
		DebugExecute( "Shrink" );

		// Move head along skeleton (from head to tail)
		float disToTarget = Mathf.Abs( targetCell.relPos - body.head.relPos );
		body.UpdateBodyShrink( -( disToTarget / 5f ) );

		// Destroy cells along skeleton
		if( body.head.next != null && body.head.distanceToNext < 0.1f )
			body.DestroyCell( body.head.next );

		// If arrived at target position,
		if( disToTarget < 0.1f )
		{
			// Trim skeleton
			body.skeleton.ShaveStart( body.correction + targetCell.relPos );

			// Reset correction to heads position
			body.correction = -body.head.relPos;

			// Switch state
			currentState = dying ? SnakeState.Die : SnakeState.Move;
		}
	}

	private void ShrinkExitState()
	{
		DebugExit( "Shrink" );
	}
// EO SHRINK STATE //

// DIE STATE //
	private void DieEnterState()
	{
		DebugEnter( "Die" );
	}

	private void DieState()
	{
		DebugExecute( "Die" );

		// // Switch state
		// currentState = SnakeState.Move;

		// If players' snake dies, it's game over
		if( name == "Player Snake" ){
			GameManager.INSTANCE.currentState = GameManager.GameState.GameOver;
			currentState = SnakeState.Idle;
		}
		
		// If any other snake dies, remove it from game
		else{
			Destroy( gameObject );
		}
	}

	private void DieExitState()
	{
		DebugExit( "Die" );

		// // Respawn snake
		// body.SpawnSnake( body.spawnPoint );
		// SpawnSnake(  );
	}
// EO DIE STATE //

//////////////////////////////////////////////////////// EO STATE METHODS //

// OTHER METHODS ///////////////////////////////////////////////////////////////

	public void SpawnSnake()
	{
		// Randomly position AI snakes
		if( settings.randomSpawnPosition ){
			respawnPoint.position = new Vector3( Random.Range( -50, 50 ), 0, Random.Range( -50, 50 ) );
			respawnPoint.Rotate( Vector3.up * Random.value * 360f );
		}
		
		body.Init();
	}
//////////////////////////////////////////////////////////// EO OTHER METHODS //

// GETTERS/SETTERS ///////////////////////////////////////////////////////////////

	[SerializeField]
	public int Size
	{
		get{
			return body.size;
		}
	}
//////////////////////////////////////////////////////////// EO GETTERS/SETTERS //

// INPUTS ////////////////////////////////////////////////////////

	private float rotationInput = 0;
	private float boostInput = 0;


	private float smoothInputHorizontal;

	private float SmoothInputHorizontal( float input )
	{
		// Smooth input
		smoothInputHorizontal = Mathf.Lerp( smoothInputHorizontal, input, Time.deltaTime * handling.inputSensitivity );
		return smoothInputHorizontal;
	}


	private void DebugInput ()
	{
		speed = 1f;

		// rotationInput = 0;
		// boostInput = 0;
		// speed = handling.maxSpeed = 0.1f;
	}


	private int currentID = 0;
	private Vector3[] corners;
	private Vector3 currentTarget;

	private void GeneratePath( Vector3 targetLocation, bool random = false )
	{
		if( random )
			targetLocation = new Vector3( Random.Range( -50, 50 ), 0, Random.Range( -50, 50 ) );

		NavMesh.CalculatePath( transform.position, targetLocation, NavMesh.AllAreas, snakeAI.path );

		currentID = 0;
		corners = snakeAI.path.corners;
	}

	private void AIInput ()
	{
		boostInput = 0;

		if( Input.GetMouseButtonDown(0) )
		{
			// Get mouse position in world
			Vector3 mouseTarget = Input.mousePosition;
			mouseTarget = new Vector3( mouseTarget.x, mouseTarget.y, Camera.main.gameObject.transform.position.y );
			mouseTarget = Camera.main.ScreenToWorldPoint( mouseTarget );

			GeneratePath( mouseTarget );
		}

		if( snakeAI.path != null && snakeAI.path.corners.Length > 0 )
		{
			Vector3 prevCorner = snakeAI.path.corners[0];
			for( int i = 1; i < snakeAI.path.corners.Length; i++ )
			{
				Vector3	corner = snakeAI.path.corners[i];
				Debug.DrawLine( prevCorner, corner );
				prevCorner = corner;
			}
		}
		// Draw angle accuracy lines
		Debug.DrawRay( transform.position, Quaternion.Euler(0, snakeAI.directionAccuracy, 0) * transform.forward * 100, Color.blue * 0.2f );
		Debug.DrawRay( transform.position, Quaternion.Euler(0, -snakeAI.directionAccuracy, 0) * transform.forward * 100, Color.blue * 0.2f );

		float inputAI = 0;

	// AVOID WITH RAYS VISION ///////////////////////////////////////////////////////////////
		
		float angleDyanmic = snakeAI.visionAngle * ( ( Mathf.Lerp( 1, Mathf.Abs(smoothInputHorizontal), snakeAI.fanEffect ) ) );

		float wallAvoidanceInput = 0;
		bool breakLoop = false;
		float partAngle = (angleDyanmic*2 / (snakeAI.numOfRays*2) );
		for( int i = 0; i < snakeAI.numOfRays * 2 + 1; i++ )
		{
			Vector3 vector = Quaternion.AngleAxis( angleDyanmic - partAngle * i, Vector3.up ) * transform.forward;
			Ray ray = new Ray( transform.position, vector );

			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, snakeAI.visionDistance, snakeAI.layerMask ) )
			{
				if( i < snakeAI.numOfRays ){
					if( !breakLoop ) wallAvoidanceInput = -1;
				}
				else{
					if( !breakLoop ) wallAvoidanceInput = 1;
				}
				Debug.DrawRay( ray.origin, ray.direction * snakeAI.visionDistance, Color.red );

				breakLoop = true;
			}
			else{
				if( !breakLoop ) wallAvoidanceInput = 0;
				Debug.DrawRay( ray.origin, ray.direction * snakeAI.visionDistance, Color.red/3 );
			}
		}
	//////////////////////////////////////////////////////////// EO AVOID WITH RAYS VISION //

	// STEER ///////////////////////////////////////////////////////////////
	
		// followTargetInput = snakeAI.SteerToTarget( transform, target, directionAccuracy );

		switch( snakeAI.steerMode )
		{
			case SteerMode.Mouse:
				// Get mouse position in world
				Vector3 mouseTarget = Input.mousePosition;
				mouseTarget = new Vector3( mouseTarget.x, mouseTarget.y, Camera.main.gameObject.transform.position.y );
				mouseTarget = Camera.main.ScreenToWorldPoint( mouseTarget );

				inputAI = snakeAI.SteerToTarget( transform, mouseTarget );
				break;

			case SteerMode.Item:
				inputAI = snakeAI.SteerToTarget( transform, Item.GetClosest( transform.position ) );
				break;

			case SteerMode.Checkpoint:
				Vector3 currentTarget = snakeAI.checkpoints[ currentID ].position;
				float distance = (currentTarget - transform.position).magnitude;
				if( distance < 3 ){
					currentID = (currentID + 1) % snakeAI.checkpoints.Count;
				}

				inputAI = snakeAI.SteerToTarget( transform, currentTarget );
				break;

			case SteerMode.Pathfinding:
				if( corners == null )
					GeneratePath( Vector3.zero, true );

				Vector3 currentCorner = corners[ currentID ];
				distance = (currentCorner - transform.position).magnitude;
				if( distance < 1 ){
					if( currentID >= corners.Length - 1 )
					{
						GeneratePath( Vector3.zero, true );
					}
					else{
						// currentID = (currentID + 1) % corners.Length;
						currentID = currentID + 1;
					}
				}

				inputAI = snakeAI.SteerToTarget( transform, currentCorner );
				break;

			case SteerMode.Wander:
				inputAI = snakeAI.Wander( transform );
				// inputAI = Input.GetAxis("Horizontal");
				break;	
		}

		// rotationInput = Input.GetAxis("Horizontal");
		if( Mathf.Abs( wallAvoidanceInput ) > 0 )
			inputAI = wallAvoidanceInput;
		// rotationInput = SmoothInputHorizontal( followCheckpointInput );
		// rotationInput = SmoothInputHorizontal( snakeAI.SteerToTarget( transform, Item.GetClosest( transform.position ) ) );

	//////////////////////////////////////////////////////////// EO STEER //

		rotationInput = SmoothInputHorizontal( inputAI );
	}

	private int leftInput = 0;
	private int rightInput = 0;

	private void PCInput ()
	{
		// // Get input by mouse clicking left or right side of the screen
		// int leftInput = Input.GetMouseButton(0) && Input.mousePosition.x <= Screen.width / 2 ? -1 : 0;
		// int rightInput = Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 2 ? 1 : 0;

		// Apply input values
		// rotationInput = SmoothInputHorizontal( leftInput + rightInput );	        // Mouse click control
		rotationInput = SmoothInputHorizontal( Input.GetAxisRaw( "Horizontal" ) );	// Smooth custom
		// rotationInput = Input.GetAxis("Horizontal");		                        // Unity smooth

		// Turn boost on
		if( Input.GetKeyDown("space") )
			TrunBoostOn();
	}

	private void TrunBoostOn()
	{
		if( !boostOn )
			boostTime = Time.time + handling.boostDuration;
	}

	private bool boostOn = true;
	private float boostTime = 0;
	private float smoothBoost = 0;

	private void AndroidInput ()
	{
		rotationInput = SmoothInputHorizontal( leftInput + rightInput + Input.GetAxisRaw("Horizontal") );
		// rotationInput = SmoothInputHorizontal( Input.GetAxisRaw("Horizontal") );
	}
	private void AndroidInputGUI ()
	{
		// Draw buttons
		float size = 100;
		float padding = 30;

		// Get inputs
		leftInput = GUI.RepeatButton( new Rect( padding, Screen.height - size - padding, size, size), "<" ) ? -1 : 0;
		rightInput = GUI.RepeatButton( new Rect( Screen.width - size - padding, Screen.height - size - padding, size, size), ">" ) ? 1 : 0;


		// Turn boost on
		if( GUI.Button( new Rect( Screen.width/2 - size*2/2, Screen.height - size - padding, size*2, size), "BOOST" ) )
			TrunBoostOn();


		// // Grow
		// if( GUI.Button( new Rect( Screen.width/2 - size*2/2 - size - padding, Screen.height - size - padding, size*2, size), "GROW" ) ){
		// 	body.Grow();
		// }
	}
//////////////////////////////////////////////////////// EO INPUTS //

// RAYCASTING ///////////////////////////////////////////////////////////////

	// [HideInInspector]
	// public GameObject source;
	// public LayerMask layerMask;

	// private GameObject lastOther;


	// private void FadeObjectInFrontOfCamera()
	// {
		
	// 	// Vector3 a = transform.position;
	// 	Vector3 a = source.transform.position;
	// 	Vector3 b = transform.position - source.transform.position;

	// 	RaycastHit hit;
	// 	Ray ray = new Ray( a, b );

	// 	bool isHit = Physics.Raycast( ray, out hit, 100, layerMask );
	// 	print( "isHit: " + isHit );

	// 	if ( isHit ){
 //            // print("There is something in front of the object! " + hit.collider);
 //            // Debug.Break();

 //            lastOther = hit.collider.gameObject;

 //            Color color = lastOther.GetComponent<Renderer>().material.color;
 //            lastOther.GetComponent<Renderer>().material.color = new Color( color.r, color.g, color.b, 0.1f );
	// 	}
	// 	else{
	// 		if( lastOther != null ){
	//             Color color = lastOther.GetComponent<Renderer>().material.color;
	//             lastOther.GetComponent<Renderer>().material.color = new Color( color.r, color.g, color.b, 1 );
	//         }
	// 	}

 //        Debug.DrawRay( a, b * 100 );
	// }
//////////////////////////////////////////////////////////// EO RAYCASTING //

// SNAKE AI ///////////////////////////////////////////////////////////////
	[System.Serializable]
	public class SnakeAI
	{
		// Steering AI
		public SteerMode steerMode = SteerMode.Checkpoint;
		[Range( 0, 90 )] public float directionAccuracy = 10;
		
		// Avoindance settings
		[Range( 0, 5 )] public int numOfRays = 1;
		[Range( 0, 100 )] public float visionDistance = 5;
		[Range( 0, 90 )] public float visionAngle = 20;
		[Range( 0, 1 )] public float fanEffect = 0.5f;
		
		// Wander mode settings
		[Range( 0, 90 )] public float rate = 0.5f;
		[Range( 0, 5 )] public float strength = 0.5f;
		
		// Chekpoints mode settings
		public List<Transform> checkpoints = new List<Transform>();

		// System
		[HideInInspector] public LayerMask layerMask = 256;
		public NavMeshPath path;

		/**
		 * SteerToTarget
		 */
		public float SteerToTarget( Transform transform, Vector3 target )
		{
			// Draw line to the selected target
			Debug.DrawLine( target, transform.position, Color.green );

			Vector3 vectorToTarget = target - transform.position;
			float directionOffset = Vector3.Angle( vectorToTarget, transform.forward );

			if( directionOffset > directionAccuracy )
			{
				Vector3 projection = Vector3.Project( vectorToTarget, transform.right );

				// Draw steer direction
				Debug.DrawRay( transform.position, projection.normalized, Color.blue );
				
				// Return steer direction
				return Vector3.Dot( transform.right, projection.normalized );
			}

			// Don't steer
			return 0;
		}

		/**
		 * Wander
		 */
		private float directionAngle = 0;
		public float Wander( Transform transform )
		{
			Vector3 circleCenter = transform.position + transform.forward * Mathf.Sqrt(2) * strength;

			// Change wander direction
			// if( Time.frameCount%3 == 0 )
				directionAngle += Random.Range( -rate, rate );

			// Find location to steer towards
			Vector3 directionVector = transform.forward * strength;
			directionVector = Quaternion.Euler(0, directionAngle, 0) * directionVector;

			// DEBUG
			// MyDraw.DrawCircle( circleCenter, 1, Color.black );
			MyDraw.DrawCircle( circleCenter, strength, Color.white * 0.6f );
			Debug.DrawRay( circleCenter, directionVector, Color.white * 0.6f );
			
			if( Time.frameCount%10 == 0 ){
				// Steer towards wander location
				return SteerToTarget( transform, circleCenter + directionVector );
			}
			else{
				return 0;
			}
		}
	}
//////////////////////////////////////////////////////////// EO SNAKE AI //
}