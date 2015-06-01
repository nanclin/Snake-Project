using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SnakeController))]
public class SnakeBody : MonoBehaviour
{
	// Components
	public SnakeBodyCell cellPrefab;

	// System
	private SnakeSkeleton _skeleton;
	private Chain _chain;
	private SnakeBodyCell _head;
	private SnakeBodyCell _tail;
	private SnakeController snakeController;
	private List<SnakeBodyCell> cellList = new List<SnakeBodyCell>();

// UNITY METHODS ///////////////////////////////////////////////////////////////

	// Use this for initialization
	void Awake () {

		// Get snake controller reference
		snakeController = GetComponent<SnakeController>();

		// Get snake head body cell reference
		_head = snakeController.GetComponent<SnakeBodyCell>();
		cellList.Add( _head );
	}

	// Update is called once per frame
	void Update ()
	{
		// // Debug
		// skeleton.Draw();
		// chain.DrawChain( chain.head );
	}

	void OnGUI()
	{
		// GUI.Label( new Rect( 10, 10, Screen.width, Screen.height ), chain.ToString() );
		
		string debugString = "";
		foreach( SnakeBodyCell cell in cellList ){
			debugString += cell.isTail + " \n";
		}
		// GUI.Label( new Rect( 10, 10, Screen.width, Screen.height ), debugString );
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

	public void Init()
	{
		// Remove old cells
		while( chain != null && chain.head.next != null )
			DestroyCell( chain.head.next );

		// Initialize skeleton and chain
		_skeleton = new SnakeSkeleton();
		_chain = new Chain();

		growQueue = 0;

		zero = 0;

		// Reposition
		transform.position = snakeController.respawnPoint.position;
		transform.rotation = snakeController.respawnPoint.rotation;

		// Create initial skeleton
		skeleton.AppendJoint( transform.position + transform.forward * 0 );
		skeleton.AppendJoint( transform.position + transform.forward * -10 );

		// Put head to the chain
		chain.AddLast( new ChainNode( 0, snakeController.settings.buffer, this.gameObject, 1 ) );

		// Set Color of head
		head.GetComponent<Renderer>().material.color = snakeController.settings.color;

		// GROW INITIAL SNAKE CELLS ///////////////////////////////////////////////////////////////
		for( int i = 0; i < snakeController.settings.lengthOnBorn; i++ )
		{
			SnakeBodyCell cell = InstantiateCell();

			// Add chain node for current cell
			ChainNode node = new ChainNode( chain.tail.value - snakeController.settings.buffer * 2, snakeController.settings.buffer, cell.gameObject, snakeController.settings.bondStrength );
			chain.AddLast( node );
			cell.node = node;

			// Put cell on the skeleton
			PutOnSkeleton( cell.transform, -(chain.tail.value - snakeController.settings.buffer * 2) - 1f );
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
			
			if( gap > 0 )
				MoveChain( currentNode.next, gap );
			else
				MoveChain( currentNode.next, 0 );
		}

		// Put prefabs on skeleton
		if( currentNode != chain.head ){
			PutOnSkeleton( currentNode.prefab.transform, zero - currentNode.value );
		}
	}

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

	// Change color of the snake
	public void SetColor( Color color )
	{
		// Set color of body cells
		foreach( SnakeBodyCell cell in cellList )
			cell.SetColor( color );
	}

	public void DestroyCell( ChainNode node )
	{
		// Remove from list of all body cells
		cellList.Remove( node.prefab.GetComponent<SnakeBodyCell>() );

		chain.RemoveNode( node );

		// if( node.prefab.gameObject.GetComponent<SnakeBodyCell>().isTail )
			// tail.gameObject.GetComponent<Renderer>().material.color = snakeController.settings.color;

		// SnakeBodyCell cellScript = node.prefab.gameObject.GetComponent<SnakeBodyCell>();
		SnakeBodyCell cellScriptPrevious = node.previous.GetCellScript();
		// print( "cellScript: " + cellScript );
		// print( "node.previous: " + node.previous );
		// print( "cellScriptPrevious: " + cellScriptPrevious );

		// // if( cellScriptPrevious != null && cellScript.isTail != null && cellScriptPrevious.isTail != null ){
		// if( cellScriptPrevious != null ){
		// 	cellScriptPrevious.isTail = true;
		// 	cellScriptPrevious.gameObject.GetComponent<Renderer>().material.color = Color.blue;
		// }
		// tail = cellScriptPrevious;

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

	private SnakeBodyCell InstantiateCell()
	{
		// Instantiate cell
		SnakeBodyCell cell = (Instantiate( cellPrefab.transform, Vector3.zero, Quaternion.identity ) as Transform).GetComponent<SnakeBodyCell>();

		// // Set color to new cell
		cell.SetColor( snakeController.settings.color );
		cell.body = this;

		// // Set tail value
		// cell.isTail = true;
		// if( tail != null )
		// 	tail.GetComponent<SnakeBodyCell>().isTail = false;
		// tail = cell;

		// // Keep list of all body cells
		cellList.Add( cell );

		return cell;
	}

	private void TryGrowing()
	{
		if( growQueue > 0 && Time.time >= growTime )
		{
			// INSTANTIATE SNAKE CELL ///////////////////////////////////////////////////////////////
			// Instantiate cell
			// Transform cell = Instantiate( cellPrefab.transform, Vector3.zero, Quaternion.identity) as Transform;
			SnakeBodyCell cell = (Instantiate( cellPrefab.transform, Vector3.zero, Quaternion.identity ) as Transform).GetComponent<SnakeBodyCell>();

			// Keep list of all body cells
			cellList.Add( cell );

			// Set color to new cell
			cell.SetColor( snakeController.settings.color );
			cell.body = this;

			// // Set tail value
			// cell.isTail = true;
			// cell.gameObject.GetComponent<Renderer>().material.color = Color.blue;
			// if( tail != null ){
			// 	tail.GetComponent<SnakeBodyCell>().isTail = false;
			// 	tail.gameObject.GetComponent<Renderer>().material.color = snakeController.settings.color;
			// }
			// tail = cell;

			// Add chain node for current cell
			chain.AddLast( new ChainNode( chain.tail.value, snakeController.settings.buffer, cell.gameObject, snakeController.settings.bondStrength ) );

			// Put cell on the skeleton
			PutOnSkeleton( cell.transform, -chain.tail.value + zero );
			//////////////////////////////////////////////////////////// EO INSTANTIATE SNAKE CELL //

			growQueue--;

			// Reset grow delay timer (wait before growing next cell)
			growTime = Time.time + growDelay;
		}
	}

	public void Grow( int num = 1 )
	{
		growQueue += num;

		// Wait before start growing
		// growTime = Time.time + growDelay;
	}
//////////////////////////////////////////////////////////// EO METHODS //

// GETTERS/SETTERS ///////////////////////////////////////////////////////////////

	public SnakeSkeleton skeleton {
		get{ return _skeleton; }
	}

	public Chain chain {
		get{ return _chain; }
	}

	public SnakeBodyCell head {
		get{ return _head; }
	}

	public SnakeBodyCell tail {
		get{ return _tail; }
	}
//////////////////////////////////////////////////////////// EO GETTERS/SETTERS //
}
