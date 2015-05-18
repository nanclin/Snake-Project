using UnityEngine;
using System.Collections;

public class SnakeController : MonoBehaviour {

	// Static
	private static bool FSM_DEBUG = GameManager.FSM_DEBUG;

	// Handling
	private float maxSpeed = 0.08f;
	// private float maxSpeed = 0.04f;
	private float acceleration = 0.01f;
	private float rotationSpeed = 3;
	public enum ControlType{ Debug, PC, Android }
	public ControlType controlType = ControlType.Debug;
	public float buffer;
	public float bondStrength;

	// Components
	public SnakeBody body;
	public GameManager gameManager;

	// System
	// public static enum SnakeState { Idle, Move, Shrink, OnRail, Die, Dead }
	private SnakeState _state;
	private float speed = 0;


// UNITY METHODS ///////////////////////////////////////////////////////////////

	// Use this for initialization
	void Awake ()
	{
		body = GetComponent<SnakeBody>();


		// marker2.position = spline.GetPointBaked( 13.5f );

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
		// FadeObjectInFrontOfCamera();
	}

	void FixedUpdate ()
	{
		ExecuteState();
	}


	void OnTriggerEnter( Collider other )
	{
		// print( "SnakeController - OnTriggerEnter" );

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
					gameManager.OpenExit();
				break;
			case "Wall":
			case "Snake Cell":
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
				// print("SnakeController");
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
				print("SnakeHeadCollider hit something not handeld by code! " + other);
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
				if( _state != null )
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
				gameManager.LevelFinished();
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


	private float shrinkNumberOfCells = 5;
	private float shrinkCurrentNumberOfCells;
	private float shrinkPosition;

	private ChainNode head;
	private ChainNode targetNode;

	private bool dying;

	private void ShrinkEnterState()
	{
		DebugEnter( "Shrink" );
		// Debug.Break();

		shrinkPosition = 0;
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
		body.MoveChain( head, -dis/10 );
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

		gameManager.GameOver();
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

	public void SpawnSnake( Transform spawnPoint )
	{
		// while( body.chain.head.next != null )
		// 	body.DestroyCell( body.chain.head.next );

		body.Init( spawnPoint );

		// Grow cells
		for( int i = 0; i < 0; i++ )
		{	
			body.Grow();
		}
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

	private bool shrinkFlag;

	private void DebugInput ()
	{
		speed = 1.5f;

		rotationInput = 0;
		boostInput = 0;

		// if( Time.frameCount == 7 )
		// 	currentState = SnakeState.Shrink;

		if( Input.GetKeyDown("space") )
			body.Grow();

		if( Input.GetKeyDown("backspace") )
			currentState = SnakeState.Shrink;

		// if( transform.position.x >= 5 && !shrinkFlag ){
		// 	currentState = SnakeState.Shrink;
		// 	shrinkFlag = true;
		// }
	}

	private void PCInput ()
	{
		rotationInput = Input.GetAxis("Horizontal");
		boostInput = Input.GetAxis("Vertical");


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

		// Boost
		// if( GUI.Button( new Rect( Screen.width/2 - size*2/2 + size + padding, Screen.height - size - padding, size*2, size), "BOOST" ) ){
		if( GUI.Button( new Rect( Screen.width/2 - size*2/2, Screen.height - size - padding, size*2, size), "BOOST" ) ){
			// Add x seconds to boostTime
			boostTime = Mathf.Max( Time.time, boostTime ) + 2;
		}
		boostInput = ( boostTime > Time.time ) ? 1 : 0;

		// // Grow
		// if( GUI.Button( new Rect( Screen.width/2 - size*2/2 - size - padding, Screen.height - size - padding, size*2, size), "GROW" ) ){
		// 	body.Grow();
		// }
	}
//////////////////////////////////////////////////////// EO INPUTS //

// RAYCASTING ///////////////////////////////////////////////////////////////

	[HideInInspector]
	public GameObject source;
	public LayerMask layerMask;

	private GameObject lastOther;


	private void FadeObjectInFrontOfCamera()
	{
		
		// Vector3 a = transform.position;
		Vector3 a = source.transform.position;
		Vector3 b = transform.position - source.transform.position;

		RaycastHit hit;
		Ray ray = new Ray( a, b );

		bool isHit = Physics.Raycast( ray, out hit, 100, layerMask );
		print( "isHit: " + isHit );

		if ( isHit ){
            // print("There is something in front of the object! " + hit.collider);
            // Debug.Break();

            lastOther = hit.collider.gameObject;

            Color color = lastOther.GetComponent<Renderer>().material.color;
            lastOther.GetComponent<Renderer>().material.color = new Color( color.r, color.g, color.b, 0.1f );
		}
		else{
			if( lastOther != null ){
	            Color color = lastOther.GetComponent<Renderer>().material.color;
	            lastOther.GetComponent<Renderer>().material.color = new Color( color.r, color.g, color.b, 1 );
	        }
		}

        Debug.DrawRay( a, b * 100 );
	}
//////////////////////////////////////////////////////////// EO RAYCASTING //
}
