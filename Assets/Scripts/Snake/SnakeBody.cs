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
	public Chain chain;
	// private List<Transform> cells = new List<Transform>();

// UNITY METHODS ///////////////////////////////////////////////////////////////

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
		chain.AddLast( new ChainNode( 0, 0.5f, this.gameObject, 1 ) );
	}
	
	// Update is called once per frame
	void Update ()
	{
		// // Debug
		skeleton.Draw();
		chain.DrawChain( chain.head );
	}

	void OnGUI()
	{
		GUI.Label( new Rect( 10, 10, Screen.width, Screen.height ), chain.ToString() );
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //


// UPDATE BODY ///////////////////////////////////////////////////////////////

	public float zero = 0;

	public void MoveChain( ChainNode currentNode, float moveBy )
	{
		// Update current nodes value
		currentNode.value += moveBy * currentNode.bondStrength;
		// Push zero
		zero = Mathf.Max( zero, currentNode.value );
		Debug.DrawLine( Vector3.forward * 1f + Vector3.right * zero, Vector3.forward * -1f + Vector3.right * zero, Color.green );

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


	public void Grow( int num = 1 )
	{
		for( int i = 0; i < num; i++ )
		{
			// Instantiate cell
			Transform cell = Instantiate( cellPrefab, Vector3.zero, Quaternion.identity) as Transform;

			// Add chain node for current cell
			chain.AddLast( new ChainNode( chain.tail.value, snakeController.buffer, cell.gameObject, snakeController.bondStrength ) );

			// Put cell on the skeleton
			PutOnSkeleton( cell.transform, -chain.tail.value + zero );
		}
	}
//////////////////////////////////////////////////////////// EO UPDATE BODY //

	public void Shrink()
	{
		DestroyCell( chain.tail.previous );
	}

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
}
