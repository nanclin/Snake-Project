using UnityEngine;
using System.Collections;

public class CameraSystem : Initializer
{
	public float smooth = 1.5f;
	public float forward = 5f;
	public float innerRing = 4;
	public float outerRing = 10;
	public float liftStep = 0.5f;
	public float maxLift = 3;
	[Range(0,1)]
	public float forwardVsBody = 0.5f;

	private Transform player;
	private SnakeController snake;
	private Vector3 relCameraPos;

	override public void Init()
	{

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
	
	public Transform cube;
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

		forwardAttractor = player.position + player.forward * forward;
		Vector3 attractors = CameraAttractor.GetAttractorsVector( player.position, forwardAttractor );
		finalPosition = attractors + relCameraPos;

		// Apply final position
		transform.position = Vector3.Lerp( transform.position, finalPosition, smooth * Time.deltaTime );
	}

	void OnDrawGizmos()
	{
		if( player ){

			// Final position
			MyDraw.DrawCircle( finalPosition - Vector3.up * finalPosition.y, 2, Color.yellow );

			// Draw camera Position
			MyDraw.DrawCross( transform.position - Vector3.up * transform.position.y, 1, Color.yellow );

			// Forward attractor
			MyDraw.DrawSquare( forwardAttractor, 0.5f, Color.green );
			// Gizmos.color = Color.green;
			// Gizmos.DrawWireCube( forwardAttractor, Vector3.one );
			
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
