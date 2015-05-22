using UnityEngine;
using System.Collections;

public class CameraSystem : Initializer
{
	public static bool DEBUG = false;

	public float smooth = 1.5f;
	public float lookForwardDis = 5f;
	public float boundaryPower = 15f;
	[Range(0,1)] public float paddingHorizontal;
	[Range(0,1)] public float paddingVertical;
	// public float innerRing = 4;
	// public float outerRing = 10;
	// public float liftStep = 0.5f;
	// public float maxLift = 3;
	// [Range(0,1)]
	// public float forwardVsBody = 0.5f;

	private Transform player;
	private SnakeController snake;
	private Vector3 relCameraPos;

	override public void Init()
	{
		transform.position = transform.position + relCameraPos;
	}

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag( "Player" ).transform;
		snake = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<SnakeController>();
		relCameraPos = transform.position - player.position;
	}

	private Vector3 snakeBodyAttractor;
	private Vector3 forwardAttractor;
	private Vector3 finalPosition;
	
	private Transform cube;
	private Vector3 screenPos;
	private Vector3 worldPos;


	void FixedUpdate ()
	{

		// // Resize outer ring with camera dolly out
		// Camera camera = GetComponent<Camera>();
		// worldPos = camera.ScreenToWorldPoint( new Vector3( Screen.width/2, Screen.height, transform.position.y ) );
		// cube.transform.position = worldPos;
		// float relativeToPlayer = worldPos.z - transform.position.z;
		// // print( "worldPos: " + worldPos );
		// // print( "relativeToPlayer: " + relativeToPlayer );
		// // outerRing = Mathf.Max( 13, Mathf.Abs( relativeToPlayer ) );
		// outerRing = Mathf.Abs( relativeToPlayer ) * 1f;
		// innerRing = outerRing * 0f;


		// // Lift camera vector
		// Vector3 lift = Vector3.up * Mathf.Min( maxLift - relCameraPos.y, Mathf.Max(0, snake.Size - 6) * liftStep );

		// // Move camera in front of where snake is going
		// forwardAttractor = player.position + player.forward * forward;

		// // Calculate vector for average position of snake cells
		// Vector3 sumVector = Vector3.zero;
		// int count = 0;
		// Chain chain = snake.body.chain;
		// ChainNode node = chain.head;
		// while( node != null )
		// {
		// 	sumVector += node.prefab.transform.position;
		// 	count++;
		// 	node = node.next;
		// }
		// snakeBodyAttractor = sumVector / count;

		// // Influence...
		// float disFromCameraCenter = (transform.position - player.position - relCameraPos - lift).magnitude;
		// float influence = ( disFromCameraCenter - innerRing ) / (outerRing - innerRing);
		// influence = Mathf.Max( influence, 0 );
		// // print( "disFromCameraCenter: " + disFromCameraCenter );
		// // print( "influence: " + influence );

		// // Calculate and apply final camera position
		// // finalPosition = Vector3.Lerp( forwardAttractor, snakeBodyAttractor, 0.5f - influence/2 ) + relCameraPos + lift;
		// finalPosition = Vector3.Lerp( forwardAttractor, snakeBodyAttractor, forwardVsBody ) + relCameraPos + lift;
		// transform.position = Vector3.Lerp( transform.position, finalPosition, smooth * Time.deltaTime );
	
		//////////////////////////////////////////////////////////// EO  //

		// Get camera
		Camera camera = GetComponent<Camera>();

		// Get points
	
		// Vertical distance of camera origin to snake		
		float h = camera.transform.position.y - snake.transform.position.y;

		// Get corner points of screen in world coordinates
		Vector3 TR = camera.ScreenToWorldPoint( new Vector3(
			Screen.width - Screen.width * paddingHorizontal / 2,
			Screen.height - Screen.height * paddingVertical / 2,
			h ) );
		Vector3 TL = camera.ScreenToWorldPoint( new Vector3(
			Screen.width * paddingHorizontal / 2,
			Screen.height - Screen.height * paddingVertical / 2,
			h ) );
		Vector3 BR = camera.ScreenToWorldPoint( new Vector3(
			Screen.width - Screen.width * paddingHorizontal / 2,
			Screen.height * paddingVertical / 2,
			h ) );
		Vector3 BL = camera.ScreenToWorldPoint( new Vector3(
			Screen.width * paddingHorizontal / 2,
			Screen.height * paddingVertical / 2,
			h ) );

		// Draw boundary rectangle
		if( DEBUG ) Debug.DrawLine( TR, TL );
		if( DEBUG ) Debug.DrawLine( TR, BR );
		if( DEBUG ) Debug.DrawLine( TL, BL );
		if( DEBUG ) Debug.DrawLine( BR, BL );

		// Get distances over the boundary
		float disTop = ( snake.transform.position - TR ).z;
		float disBottom = ( BR - snake.transform.position ).z;
		float disLeft = ( TL - snake.transform.position ).x;
		float disRight = ( snake.transform.position - TR ).x;

		finalPosition =
			  Vector3.forward * Mathf.Max( 0, disTop )    * boundaryPower
			- Vector3.forward * Mathf.Max( 0, disBottom ) * boundaryPower
			- Vector3.right   * Mathf.Max( 0, disLeft )   * boundaryPower
			+ Vector3.right   * Mathf.Max( 0, disRight )  * boundaryPower
			;
		// transform.position = Vector3.Lerp( transform.position, finalPosition, smooth * 2 * Time.deltaTime );



		forwardAttractor = player.position + player.forward * lookForwardDis;
		Vector3 attractors = CameraAttractor.GetAttractorsVector( player.position, forwardAttractor );
		finalPosition += attractors + relCameraPos;

		// Apply final position
		transform.position = Vector3.Lerp( transform.position, finalPosition, smooth * Time.deltaTime );
	}

	void OnDrawGizmos()
	{
		if( player && DEBUG ){

			// Final position
			MyDraw.DrawCircle( finalPosition - Vector3.up * finalPosition.y, 2, Color.yellow );

			// Draw camera Position
			MyDraw.DrawCross( transform.position - Vector3.up * transform.position.y, 1, Color.yellow );

			// Forward attractor
			MyDraw.DrawSquare( forwardAttractor, 0.5f, Color.green );
			Debug.DrawLine( player.position, forwardAttractor, Color.green );
			
			// // Snake body attractor
			// Gizmos.color = Color.red;
			// Gizmos.DrawWireCube( snakeBodyAttractor, Vector3.one );
			

			// // Influence range
			// Gizmos.DrawWireSphere( transform.position - Vector3.up * transform.position.y, outerRing );
			// Gizmos.DrawWireSphere( transform.position - Vector3.up * transform.position.y, innerRing );


			// Gizmos.color = Color.red;
			// Gizmos.DrawWireSphere( worldPos, 1f );
		}
	}
}
