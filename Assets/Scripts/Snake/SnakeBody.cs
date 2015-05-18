using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour {

	// Debug
	public Transform marker;

	// Components
	public Transform head;
	public Transform cellPrefab;
	public Transform spawnPoint;

	// System
	private SnakeController snakeController;
	public SnakeSkeleton skeleton;
	public Chain chain;
	// private List<Transform> cells = new List<Transform>();

// UNITY METHODS ///////////////////////////////////////////////////////////////

	// Use this for initialization
	void Start () {

		snakeController = GetComponent<SnakeController>();

		// Init( spawnPoint.position );
	}
	
	// Update is called once per frame
	void Update ()
	{
		// // Debug
		skeleton.Draw();
		// chain.DrawChain( chain.head );
	}

	void OnGUI()
	{
		// GUI.Label( new Rect( 10, 10, Screen.width, Screen.height ), chain.ToString() );
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

	public void Init( Transform spawnPoint, int size = 2 )
	{
		// Remove old cells
		while( chain != null && chain.head.next != null )
			DestroyCell( chain.head.next );

		// Initialize skeleton and chain
		skeleton = new SnakeSkeleton();
		chain = new Chain();

		growQueue = 0;

		zero = 0;

		// Reposition
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;

		// Create initial skeleton
		skeleton.AppendJoint( transform.position + transform.forward * 0 );
		skeleton.AppendJoint( transform.position + transform.forward * -10 );

		// Put head to the chain
		chain.AddLast( new ChainNode( 0, 0.5f, this.gameObject, 1 ) );

		// GROW INITIAL SNAKE CELLS ///////////////////////////////////////////////////////////////
		// while( growQueue-- > 0)
		for( int i = 0; i < size; i++ )
		{
			// Instantiate cell
			Transform cell = Instantiate( cellPrefab, Vector3.zero, Quaternion.identity) as Transform;

			float value = chain.tail.value;
			// Add chain node for current cell
			chain.AddLast( new ChainNode( value - snakeController.buffer * 2, snakeController.buffer, cell.gameObject, snakeController.bondStrength ) );

			// Put cell on the skeleton
			PutOnSkeleton( cell.transform, -(value - snakeController.buffer * 2) );
		}
		//////////////////////////////////////////////////////////// EO GROW INITIAL SNAKE CELLS //
	}

// UPDATE BODY ///////////////////////////////////////////////////////////////

	[HideInInspector]
	public float zero;

	public void MoveChain( ChainNode currentNode, float moveBy )
	{
		// Update current nodes value
		currentNode.value += moveBy * currentNode.bondStrength;
		// Push zero
		zero = Mathf.Max( zero, currentNode.value );
		// Debug.DrawLine( Vector3.forward * 1f + Vector3.right * zero, Vector3.forward * -1f + Vector3.right * zero, Color.green );

		// Continoue with the next ChainNode
		if( currentNode.next != null )
		{

			float gap = currentNode.Gap;
			float dis = currentNode.Distance;
			
			if( gap > 0 )
				MoveChain( currentNode.next, gap );
			else
				MoveChain( currentNode.next, 0 );

			// string trace = "";
			// trace += "CURRENT NODE:\n";
			// trace += "value: " + (float) Math.Round( currentNode.value, GameManager.ROUND_DECIMALS ) + "\n";
			// trace += "\n NEXT NODE:\n";
			// trace += "value: " + (float) Math.Round( currentNode.next.value, GameManager.ROUND_DECIMALS ) + "\n";
			// trace += "\n RELATIONS:\n";
			// trace += "dis: " + (float) Math.Round( dis, GameManager.ROUND_DECIMALS ) + "\n";
			// trace += "gap: " + (float) Math.Round( gap, GameManager.ROUND_DECIMALS ) + "\n";
			// Debug.Log( trace );
		}

		// Put prefabs on skeleton
		if( currentNode != chain.head ){
			PutOnSkeleton( currentNode.prefab.transform, zero - currentNode.value );
		}
	}

	// public float temp = 0;

	public void UpdateBody( float moveBy )
	{
		TryGrowing();

		MoveChain( chain.head, moveBy );

		skeleton.TrimEnd( Mathf.Max( Mathf.Min( -(chain.tail.value - chain.head.value) + 1, skeleton.length ), 0 ) );


		// // Grow parts in realtime
		// if( Time.frameCount > 30 && Time.frameCount%30 == 0 && chain.Count < 5 )
		// 	Grow();


			// ChainNode node = chain.head;

			// node.value += moveBy;

			// while( node.next != null )
			// {
			// 	print( node.Gap );

			// 	if( node.Gap > 0 )
			// 		node.

			// 	node = node.next;
			// 	print( node );
			// }



		// string trace = "";

		// int i = 0;
		// ChainNode currentNode = chain.head.next;

		// while( currentNode != null && i < 100)
		// {
		// 	// Get and reposition currentCell
		// 	Transform currentCell = cells[i];
		// 	PutOnSkeleton( currentCell.gameObject, -currentNode.value + chain.head.value );

		// 		// trace += currentNode.value + " : " + chain.head.value + "\n";

		// 	// Move to next node
		// 	currentNode = currentNode.next;
		// 	i++;
		// }
		// 	// print( trace );

		// 	// skeleton.TrimEnd( Mathf.Max( Mathf.Min( -(chain.tail.value - chain.head.value) + 1, skeleton.length ), 0 ) );
		// // print( -(chain.tail.value - chain.head.value) );


	}
//////////////////////////////////////////////////////////// EO UPDATE BODY //

// METHODS ///////////////////////////////////////////////////////////////

	public void DestroyCell( ChainNode cell )
	{
		chain.RemoveNode( cell );
	}

	public void PutOnSkeleton( Transform prefab, float position )
	{
		SkeletonPointData point = skeleton.GetPointOnSkeleton( position );
		prefab.position = point.position;
		prefab.rotation = point.rotation;
	}

	private int growQueue = 0;
	private float growTime;
	private float growDelay = 0.5f;

	private void TryGrowing()
	{
		if( growQueue > 0 && Time.time >= growTime )
		{
			// INSTANTIATE SNAKE CELL ///////////////////////////////////////////////////////////////
			// Instantiate cell
			Transform cell = Instantiate( cellPrefab, Vector3.zero, Quaternion.identity) as Transform;

			// Add chain node for current cell
			chain.AddLast( new ChainNode( chain.tail.value, snakeController.buffer, cell.gameObject, snakeController.bondStrength ) );

			// Put cell on the skeleton
			PutOnSkeleton( cell.transform, -chain.tail.value + zero );
			//////////////////////////////////////////////////////////// EO INSTANTIATE SNAKE CELL //

			growQueue--;

			// Reset grow delay timer
			growTime = Time.time + growDelay;
		}
	}

	public void Grow( int num = 1 )
	{
		growQueue += num;

		// Wait before start growing
		growTime = Time.time + growDelay;

		// for( int i = 0; i < num; i++ )
		// {
		// 	// Instantiate cell
		// 	Transform cell = Instantiate( cellPrefab, Vector3.zero, Quaternion.identity) as Transform;

		// 	// Add chain node for current cell
		// 	chain.AddLast( new ChainNode( chain.tail.value, snakeController.buffer, cell.gameObject, snakeController.bondStrength ) );

		// 	// Put cell on the skeleton
		// 	PutOnSkeleton( cell.transform, -chain.tail.value + zero );
		// }
	}
//////////////////////////////////////////////////////////// EO METHODS //
}
