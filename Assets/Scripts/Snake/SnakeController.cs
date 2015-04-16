using UnityEngine;
using System.Collections;

public class SnakeController : MonoBehaviour {

	// Static
	private static bool FSM_DEBUG = false;

	// Handling
	private float maxSpeed = 2f;
	private float acceleration = 2f;
	private float rotationSpeed = 40;
	// [Range(0.0f, 2.0f)]
	[SerializeField]
	public float buffer = 0.5f;
	// [Range(0.0f, 1.0f)]
	[SerializeField]
	public float bondStrength = 1f;

	// Components

	// System
	public enum State{
		Idle,
		Move,
		Shrink,
		ToHole,
		Die,
		Dead
	}
	private State _state;

	private SnakeBody body;
	private float dt;
	private float speed = 0;
	private Vector3 direction = Vector3.forward;
	private Vector3 currentPosition = Vector3.zero;


	// Use this for initialization
	void Start ()
	{
		body = GetComponent<SnakeBody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		ExecuteState();

		if( Input.GetKeyDown("space") )
			body.Grow();
	}


// FSM MACHINE METHODS ////////////////////////////////////////////////////////

	// Set currentState to transition to new state
	public State currentState
	{
		get { return _state; }
		set {
			// If current state is set,
			// run exit state code
			if( _state != null)
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
		}
	}

	private void ExecuteState()
	{
		switch( currentState )
		{
			case State.Move:
				MoveState();
				break;
			case State.Idle:
				IdleState();
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
		}
	}
// EO FSM MACHINE METHODS ////////////////////////////////////////////////////////


// STATE METHODS ////////////////////////////////////////////////////////

// IDLE STATE //
	private void IdleEnterState()
	{
		if( FSM_DEBUG ) print("FSM -> IdleEnterState");
	}

	private void IdleState()
	{
		if( FSM_DEBUG ) print("FSM -> Idle");
		if( Time.frameCount > 30 )
			currentState = State.Move;
	}

	private void IdleExitState()
	{
		if( FSM_DEBUG ) print("FSM -> IdleExitState");
	}
// EO IDLE STATE //


// MOVE STATE //
	private void MoveEnterState()
	{
		if( FSM_DEBUG ) print("FSM -> MoveEnterState");
	}

	private void MoveState()
	{
		if( FSM_DEBUG ) print("FSM -> Move");

		// dt = Time.deltaTime;
		dt = 0.025f;
		// dt = 1f;

		float turbo = Input.GetAxis("Vertical") * 5;
		float turboRotation = Mathf.Max( 0, Input.GetAxis("Vertical") * 80 );

		// Translate snake
		float positionChange = speed * dt;
		transform.Translate( Vector3.forward * positionChange );
		speed += acceleration;
		speed = Mathf.Min( speed, maxSpeed + turbo );
		speed = Mathf.Max( speed, 0 );

		// Store current positon point
		body.skeleton.PrependJoint( transform.position );

		// // Rotate and translate snakes head
		transform.Rotate( Vector3.up * Input.GetAxis("Horizontal") * ( rotationSpeed + turboRotation ) * dt );


		//
		body.UpdateBody( positionChange );
	}

	private void MoveExitState()
	{
		if( FSM_DEBUG ) print("FSM -> MoveExitState");
	}
// EO MOVE STATE //
}
