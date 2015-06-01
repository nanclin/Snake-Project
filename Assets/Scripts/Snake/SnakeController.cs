using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SnakeBody))]
public class SnakeController : Initializer {

	public static List<SnakeController> POOL = new List<SnakeController>();

	// Static
	// private static bool FSM_DEBUG = GameManager.FSM_DEBUG;
	private static bool FSM_DEBUG = false;

	// Handling
	private float maxSpeed = 0.08f;
	public Color color = Color.green;
	// private float maxSpeed = 0.04f;
	private float acceleration = 0.01f;
	private float rotationSpeed = 3;
	
	public enum SteerMode{ Debug, PC, Android, AI }
	public SteerMode steerMode = SteerMode.Debug;
	public float buffer;
	public float bondStrength;
	public LayerMask layerMask;

	// Components
	[HideInInspector] public SnakeBody body;
	// public GameManager gameManager;

	// System
	[HideInInspector] public Transform respawnPoint;
	private SnakeState _state;
	private float speed = 0;

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

		path = new NavMeshPath();

		// Set respawn point
		respawnPoint = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		boostInput = Input.GetAxis("Vertical");

		if( currentState != SnakeState.Die )
		switch( steerMode )
		{
			case SteerMode.Debug:   DebugInput();   break;
			case SteerMode.PC:      PCInput();      break;
			case SteerMode.AI:      AIInput();      break;
			case SteerMode.Android: AndroidInput(); break;
		}

		// Debug keybidings
		if( Input.GetKeyDown("space") )
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
			steerMode = steerMode == SteerMode.PC ? SteerMode.AI : SteerMode.PC;


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
			case "Ground":
			case "Wall":
			case "Player":
				bool otherIsNeck = other.gameObject == body.chain.head.next.prefab;
				bool otherIsTail = other.gameObject == body.chain.tail.prefab;
				if( currentState != SnakeState.Shrink && currentState != SnakeState.OnRail )
				{
					if( otherIsTail && body.chain.Count > 2 ){
						body.DestroyCell( body.chain.tail );
						body.Grow();
					}
					else if( !otherIsNeck ){
						currentState = SnakeState.Shrink;
					}
				}
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
		if( steerMode == SteerMode.Android )
			AndroidInputGUI();
	}

	private void MoveState()
	{
		DebugExecute( "Move" );

		// Set boost
		float boost = boostInput * 0.1f;
		float boostRotation = Mathf.Max( 0, boostInput * 1 );

		// TRANSLATE SNAKE ///////////////////////////////////////////////////////////////
		// Calculate position
		float positionChange = speed;
		transform.Translate( Vector3.forward * positionChange );
		// Calculate speed
		speed += acceleration;
		speed = Mathf.Min( speed, maxSpeed + boost );
		speed = Mathf.Max( speed, 0 );
		// Store current positon point
		body.skeleton.PrependJoint( transform.position );
		//////////////////////////////////////////////////////////// EO TRANSLATE SNAKE //

		// // Rotate and translate snakes head
		transform.Rotate( Vector3.up * rotationInput * ( rotationSpeed + boostRotation ) );

		//
		body.UpdateBody( positionChange );
	}

	private void MoveExitState()
	{
		DebugExit( "Move" );
	}
// EO MOVE STATE //


	public BezierSpline spline;
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

	public Hole hole;

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
		speed += acceleration;
		speed = Mathf.Min( speed, maxSpeed + boost );
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


	private float shrinkNumberOfCells = 3;
	private float shrinkCurrentNumberOfCells;

	private ChainNode head;
	private ChainNode targetNode;

	private bool dying;

	private void ShrinkEnterState()
	{
		DebugEnter( "Shrink" );

		shrinkCurrentNumberOfCells = shrinkNumberOfCells;
		head = body.chain.head;

		// Get target node (node to which snake is going to be shrinked)
		targetNode = head;
		while( shrinkCurrentNumberOfCells-- > 0 && targetNode.next != null ){
			targetNode = targetNode.next;
		}

		// If target node is the last node of the snake,
		// this is last, dying shrink
		dying = ( targetNode == body.chain.tail );
	}

	private void ShrinkState()
	{
		DebugExecute( "Shrink" );


		// Move head along skeleton - from head to tail
		float dis = Mathf.Abs( targetNode.value - head.value );
		body.MoveChain( head, -dis/5 );
		// body.MoveChain( head, -Mathf.Max( 0.02f, dis/10 ) );
		body.PutOnSkeleton( transform, body.zero - head.value );

		// Destroy cells on path
		if( head.next != null && head.Distance < 0.1f ){
			body.DestroyCell( head.next );
		}

		// If arrived at target position,
		// trim skeleton and reset zero value to heads value
		if( dis < 0.1f )
		{
			body.skeleton.ShaveStart( body.zero - head.value );
			body.zero = head.value;

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

		if( steerMode != SteerMode.AI )
			// GameManager.INSTANCE.GameOver();
			GameManager.INSTANCE.currentState = GameManager.GameState.GameOver;
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
		if( steerMode == SteerMode.AI ){
			respawnPoint.position = new Vector3( Random.Range( -50, 50 ), 0, Random.Range( -50, 50 ) );
			respawnPoint.Rotate( Vector3.up * Random.value * 360f );
		}
		
		body.Init( respawnPoint );
	}
//////////////////////////////////////////////////////////// EO OTHER METHODS //

// GETTERS/SETTERS ///////////////////////////////////////////////////////////////

	[SerializeField]
	public int Size
	{
		get{
			return body.chain.Count;
		}
	}
//////////////////////////////////////////////////////////// EO GETTERS/SETTERS //

// INPUTS ////////////////////////////////////////////////////////

	private float rotationInput = 0;
	private float boostInput = 0;


	private float smoothInputHorizontal;
	public float inputSensitivity = 5f;

	private float SmoothInputHorizontal( float input )
	{
		// Smooth input
		smoothInputHorizontal = Mathf.Lerp( smoothInputHorizontal, input, Time.deltaTime * inputSensitivity );
		return smoothInputHorizontal;
	}


	private void DebugInput ()
	{
		// speed = 1.5f;

		// rotationInput = 0;
		boostInput = 0;
	}


	public float dis = 10;
	public float angle = 45;
	public int num = 1;

	[Range(0,90)]public float directionAccuracy;

	public List<Transform> points = new List<Transform>();
	public Vector3[] corners;
	private int currentID = 0;
	Vector3 currentTarget;

	public enum SteerType{ None, Wander, Mouse, Item, Checkpoint, Pathfinding }
	public SteerType steerType = SteerType.Checkpoint;

	[Range(0,90)] public float rate = 0.5f;
	[Range(0,5)] public float strength = 0.5f;
	[Range(0,1)] public float fanEffect = 0.5f;

	private SnakeAI snakeAI = new SnakeAI();
	private NavMeshPath path;

	private void GeneratePath( Vector3 targetLocation, bool random = false )
	{
		if( random )
			targetLocation = new Vector3( Random.Range( -50, 50 ), 0, Random.Range( -50, 50 ) );

		NavMesh.CalculatePath( transform.position, targetLocation, NavMesh.AllAreas, path );

		currentID = 0;
		corners = path.corners;
	}

	private void AIInput ()
	{
		if( Input.GetMouseButtonDown(0) )
		{
			// Get mouse position in world
			Vector3 mouseTarget = Input.mousePosition;
			mouseTarget = new Vector3( mouseTarget.x, mouseTarget.y, Camera.main.gameObject.transform.position.y );
			mouseTarget = Camera.main.ScreenToWorldPoint( mouseTarget );

			GeneratePath( mouseTarget );
		}

		if( path != null && path.corners.Length > 0 )
		{
			Vector3 prevCorner = path.corners[0];
			for( int i = 1; i < path.corners.Length; i++ )
			{
				Vector3	corner = path.corners[i];
				Debug.DrawLine( prevCorner, corner );
				prevCorner = corner;
			}
		}
		// // Draw angle accuracy lines
		// Debug.DrawRay( transform.position, Quaternion.Euler(0, directionAccuracy, 0) * transform.forward * 100, Color.blue * 0.2f );
		// Debug.DrawRay( transform.position, Quaternion.Euler(0, -directionAccuracy, 0) * transform.forward * 100, Color.blue * 0.2f );

		float inputAI = 0;

	// AVOID WITH RAYS VISION ///////////////////////////////////////////////////////////////
		
		float angleDyanmic = angle * ( ( Mathf.Lerp( 1, Mathf.Abs(smoothInputHorizontal), fanEffect ) ) );

		float wallAvoidanceInput = 0;
		bool breakLoop = false;
		float partAngle = (angleDyanmic*2 / (num*2) );
		for( int i = 0; i < num * 2 + 1; i++ )
		{
			Vector3 vector = Quaternion.AngleAxis( angleDyanmic - partAngle * i, Vector3.up ) * transform.forward;
			Ray ray = new Ray( transform.position, vector );

			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, dis, layerMask ) )
			{
				if( i < num ){
					if( !breakLoop ) wallAvoidanceInput = -1;
				}
				else{
					if( !breakLoop ) wallAvoidanceInput = 1;
				}
				Debug.DrawRay( ray.origin, ray.direction * dis, Color.red );

				breakLoop = true;
			}
			else{
				if( !breakLoop ) wallAvoidanceInput = 0;
				Debug.DrawRay( ray.origin, ray.direction * dis, Color.red/3 );
			}
		}

	// STEER ///////////////////////////////////////////////////////////////
	
		// followTargetInput = snakeAI.SteerToTarget( transform, target, directionAccuracy );

		switch( steerType )
		{
			case SteerType.Mouse:
				// Get mouse position in world
				Vector3 mouseTarget = Input.mousePosition;
				mouseTarget = new Vector3( mouseTarget.x, mouseTarget.y, Camera.main.gameObject.transform.position.y );
				mouseTarget = Camera.main.ScreenToWorldPoint( mouseTarget );

				inputAI = snakeAI.SteerToTarget( transform, mouseTarget, directionAccuracy );
				break;

			case SteerType.Item:
				inputAI = snakeAI.SteerToTarget( transform, Item.GetClosest( transform.position ), directionAccuracy );
				break;

			case SteerType.Checkpoint:
				Vector3 currentTarget = points[ currentID ].position;
				float distance = (currentTarget - transform.position).magnitude;
				if( distance < 3 ){
					currentID = (currentID + 1) % points.Count;
				}

				inputAI = snakeAI.SteerToTarget( transform, currentTarget, directionAccuracy );
				break;

			case SteerType.Pathfinding:
				if( corners.Length == 0 )
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

				inputAI = snakeAI.SteerToTarget( transform, currentCorner, directionAccuracy );
				break;

			case SteerType.Wander:
				inputAI = snakeAI.Wander( transform, rate, strength, directionAccuracy );
				// inputAI = Input.GetAxis("Horizontal");
				break;	
		}

		// rotationInput = Input.GetAxis("Horizontal");
		if( Mathf.Abs( wallAvoidanceInput ) > 0 )
			inputAI = wallAvoidanceInput;
		// rotationInput = SmoothInputHorizontal( followCheckpointInput );
		// rotationInput = SmoothInputHorizontal( snakeAI.SteerToTarget( transform, Item.GetClosest( transform.position ), directionAccuracy ) );

		rotationInput = SmoothInputHorizontal( inputAI );
	}


	private void PCInput ()
	{
		// // Get input by mouse clicking left or right side of the screen
		// int leftInput = Input.GetMouseButton(0) && Input.mousePosition.x <= Screen.width / 2 ? -1 : 0;
		// int rightInput = Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 2 ? 1 : 0;

		// Apply input values
		// rotationInput = SmoothInputHorizontal( leftInput + rightInput );	        // Mouse click control
		rotationInput = SmoothInputHorizontal( Input.GetAxisRaw( "Horizontal" ) );	// Smooth custom
		// rotationInput = Input.GetAxis("Horizontal");		                        // Unity smooth
		
		boostInput = Input.GetAxis("Vertical");
	}

	private bool boostEnabled = true;
	private float boostTime = 0;

	private int leftInput = 0;
	private int rightInput = 0;

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


		// Boost
		if( boostEnabled ){
			// if( GUI.Button( new Rect( Screen.width/2 - size*2/2 + size + padding, Screen.height - size - padding, size*2, size), "BOOST" ) ){
			if( GUI.Button( new Rect( Screen.width/2 - size*2/2, Screen.height - size - padding, size*2, size), "BOOST" ) ){
				// Add x seconds to boostTime
				boostTime = Mathf.Max( Time.time, boostTime ) + 1;
				boostEnabled = false;
			}
		}
		int boostDir = ( boostTime > Time.time ) ? 1 : 0;
		if( boostDir > 0 )
			boostInput = SmoothBoost( boostDir );
		else
			boostEnabled = true;

		// // Grow
		// if( GUI.Button( new Rect( Screen.width/2 - size*2/2 - size - padding, Screen.height - size - padding, size*2, size), "GROW" ) ){
		// 	body.Grow();
		// }
	}

	private float smoothBoost = 0;
	public float boostAcceleration = 2f;

	private float SmoothBoost( float boost )
	{
		smoothBoost = Mathf.Lerp( smoothBoost, boost, Time.deltaTime * boostAcceleration );
		return smoothBoost;
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
}

class SnakeAI
{
	public float SteerToTarget( Transform transform, Vector3 target, float directionAccuracy )
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

	private float directionAngle = 0;
	public float Wander( Transform transform, float rate, float strength, float directionAccuracy )
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
			return SteerToTarget( transform, circleCenter + directionVector, directionAccuracy );
		}
		else{
			return 0;
		}
	}
}