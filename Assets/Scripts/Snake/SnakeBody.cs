using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour {

	// Debug
	public Transform marker;

	// Components
	private Transform head;
	public Transform cellPrefab;

	// System
	private SnakeController snakeController;
	public SnakeSkeleton skeleton = new SnakeSkeleton();
	private Chain chain;
	private List<Transform> cells = new List<Transform>();

	// Use this for initialization
	void Start () {
	
		// Initialize skeleton and chain
		snakeController = GetComponent<SnakeController>();
		chain = new Chain();

		// Create initial skeleton
		skeleton.AppendJoint( transform.forward * 0 );
		skeleton.AppendJoint( transform.forward * -10 );

		// Put head to the chain
		// cells.Add( transform );
		chain.AddLast( new ChainNode( 0, snakeController.buffer, 1 ) );

		// Grow cells
		for( int i = 0; i < 0; i++ )
		{	
			Grow();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		SkeletonPointData data = skeleton.GetPointOnSkeleton( 25 * Time.deltaTime );
		marker.position =  Vector3.forward * 0 + data.position;
		// print( Math.Round( Time.deltaTime * 25, 3 ) );
	}

	public void UpdateBody( float moveBy )
	{

		// Grow parts in realtime
		if( Time.frameCount > 30 && Time.frameCount%30 == 0 && chain.Count < 5 )
			Grow();

		// Update chain data
		chain.MoveChain( chain.head, moveBy );


		string trace = "";

		int i = 0;
		ChainNode currentNode = chain.head.next;

		while( currentNode != null && i < 100)
		{
			// Value of current ChainNode, recalculated for skeleton
			float currentValue = -currentNode.value + chain.head.value;
			trace += "[" + i + "]\t";
			trace += currentNode.value + "\t -> \t" + currentValue + "\n";

			// Get and reposition currentCell
			Transform currentCell = cells[i];
			SkeletonPointData point = skeleton.GetPointOnSkeleton( currentValue);
			currentCell.position = point.position;
			currentCell.rotation = point.rotation;

			// trace += currentNode.value + " : " + chain.head.value + "\n";

			// Move to next node
			currentNode = currentNode.next;
			i++;
		}
		// print( trace );

		skeleton.Draw();
		chain.DrawChain( chain.head );
	}


	public void Grow()
	{
		// Debug.Break();

		// Instantiate cell
		Transform cell = Instantiate( cellPrefab, Vector3.zero, Quaternion.identity) as Transform;
		cells.Add( cell );

		// Add chain node for current cell
		chain.AddLast( new ChainNode( chain.tail.value, snakeController.buffer, snakeController.bondStrength ) );

		// // Put cell on the skeleton
		SkeletonPointData point = skeleton.GetPointOnSkeleton( -chain.tail.value + chain.head.value );
		cell.position = point.position;
		cell.rotation = point.rotation;
	}
}
