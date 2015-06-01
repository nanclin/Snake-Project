using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Chain {

	// Handling

	// System
	public ChainNode first;
	public ChainNode last;
	private int _count = 0;

	// private LinkedList<ChainNode> = new LinkedList<ChainNode>();


// CONSTRUCTOR ////////////////////////////////////////////////////////

	// public Chain( SnakeBody body = null )
	// {
	// 	this.body = body;
	// }
//////////////////////////////////////////////////////// EO CONSTRUCTOR //


// NODES MANIPULATION ////////////////////////////////////////////////////////



	public void AddLast( ChainNode newNode )
	{
		if( first == null )
			first = newNode;
		else
			newNode.previous = last;

		if( last != null ) {
			last.next = newNode;
		}
		last = newNode;


		// // Reposition newly added ChainNode
		// if( last.previous != null )
		// 	MoveChain( last.previous, 0 );
		
		if( _count > 0 ){
			newNode.prefab.GetComponent<SnakeBodyCell>().OnCellDestroy += RemoveNode;
		}


		_count++;
	}
	public void RemoveNode( ChainNode node )
	{
		// Remove view
		node.prefab.GetComponent<SnakeBodyCell>().OnCellDestroy -= RemoveNode;
		GameObject.Destroy( node.prefab );

		if( node != first && node != last ){
			node.previous.next = node.next;
			node.next.previous = node.previous;
		}
		if( node == last ){
			if( node.previous != null)
				last = node.previous;
			node.previous.next = null;
		}
		if( node == first ){
			first = node.next;
			node.next.previous = null;
		}

		_count--;
	}
//////////////////////////////////////////////////////// EO NODES MANIPULATION //





	// public void MoveChain( ChainNode currentNode, float moveBy )
	// {
	// 	currentNode.value += moveBy * currentNode.bondStrength;

	// 	// Continoue with the next ChainNode
	// 	if( currentNode.next != null )
	// 	{

	// 		float gap = currentNode.Gap;
	// 		float dis = currentNode.Distance;

	// 		if( gap > 0 )
	// 			MoveChain( currentNode.next, gap );
	// 		else
	// 			MoveChain( currentNode.next, 0 );

	// 		// string trace = "";
	// 		// trace += "CURRENT NODE:\n";
	// 		// trace += "value: " + (float) Math.Round( currentNode.value, GameManager.ROUND_DECIMALS ) + "\n";
	// 		// trace += "\n NEXT NODE:\n";
	// 		// trace += "value: " + (float) Math.Round( currentNode.next.value, GameManager.ROUND_DECIMALS ) + "\n";
	// 		// trace += "\n RELATIONS:\n";
	// 		// trace += "dis: " + (float) Math.Round( dis, GameManager.ROUND_DECIMALS ) + "\n";
	// 		// trace += "gap: " + (float) Math.Round( gap, GameManager.ROUND_DECIMALS ) + "\n";
	// 		// Debug.Log( trace );
	// 	}
	// }




// BASIC SETTERS, GETTTERS ////////////////////////////////////////////////////////
	[SerializeField]
	public int Count {
		get {
			return _count;
		}
	}
//////////////////////////////////////////////////////// EO BASIC SETTERS, GETTTERS //


// DEBUG METHODS ////////////////////////////////////////////////////////

	public void DrawChain ( ChainNode currentNode, float colorModifier = 1 )
	{
		Debug.DrawLine(
			Vector3.right * currentNode.value + Vector3.forward * -0.1f,
			Vector3.right * currentNode.value + Vector3.forward * 0.1f, Color.red * colorModifier );
		MyDraw.DrawCircle(
			Vector3.right * currentNode.value,
			currentNode.buffer, Color.red * colorModifier );
		
		if( currentNode.next != null) {
			colorModifier = Mathf.Max( colorModifier - 0.2f, 0.2f );
			DrawChain( currentNode.next, colorModifier );
		}
	}

	override public string ToString()
	{
		string trace = "";

		trace += "count: " + _count + "\n";

		ChainNode node = first;
		int i = 0;
		while( node != null )
		{
			trace += "Node " + i + "\n";
			// trace += "  value: " + node.value + "\n";
			// trace += "  buffer: " + node.buffer + "\n";
			// trace += "  bondStrength: " + node.bondStrength + "\n";
			// trace += "  prev: " + node.previous + "\n";
			// trace += "  next: " + node.next + "\n";

			node = node.next;
		}
		return trace;
	}
//////////////////////////////////////////////////////// EO DEBUG METHODS //
	
}

public class ChainNode
{
	// Handling
	public float buffer;
	public float bondStrength;

	// System
	public float value;
	public ChainNode next;
	public ChainNode previous;
	public GameObject prefab;

	public ChainNode( float value, float buffer, GameObject prefab, float bondStrength = 1 )
	{
		this.value = value;
		this.buffer = buffer;
		this.prefab = prefab;
		this.bondStrength = bondStrength;
	}

	public void LinkToParentNode( ChainNode parent )
	{
		// Temporary store child
		ChainNode child = parent.next;

		// Set links to current node
		next = child;
		previous = parent;

		// Relink child and parent nodes with current node
		if( child != null )
		child.previous = this;
		parent.next = this;

	}

	// Return distance to the next ChainNode
	[SerializeField]
	public float Distance {
		get {
			if( next != null ) {
				return value - next.value;
			}
			else {
				throw new System.ArgumentException("Can't get distance to the next ChainNode, because it doesn't exist!");
			}
		}
	}

	// Return gap to the next ChainNode
	[SerializeField]
	public float Gap {
		get {
			if( next != null ) {
				return Distance - buffer - next.buffer;
			}
			else {
				throw new System.ArgumentException("Can't get gap to the next ChainNode, because it doesn't exist!");
			}
		}
	}
	// [SerializeField]
	// public ChainNode next {
	// 	get {

	// 	}
	// }
	
	
	[SerializeField]
	public SnakeBodyCell GetCellScript() {
		return prefab.gameObject.GetComponent<SnakeBodyCell>();
	}
}