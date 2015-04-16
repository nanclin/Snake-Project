using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Chain {

	// Handling

	// System
	private int _count = 0;
	public ChainNode head;
	public ChainNode tail;


// CONSTRUCTOR ////////////////////////////////////////////////////////

	public Chain()
	{

	}
//////////////////////////////////////////////////////// EO CONSTRUCTOR //


// NODES MANIPULATION ////////////////////////////////////////////////////////

	public void AddLast( ChainNode newNode )
	{
		if( head == null ) {
			head = newNode;
		}

		if( tail != null ) {
			tail.next = newNode;
		}
		tail = newNode;

		_count++;

		// Reposition newly added ChainNode
		if( tail.previous != null )
			MoveChain( tail.previous, 0 );
	}
//////////////////////////////////////////////////////// EO NODES MANIPULATION //





	public void MoveChain( ChainNode currentNode, float moveBy )
	{
		currentNode.value += moveBy * currentNode.bondStrength;

		// Continoue with the next ChainNode
		if( currentNode.next != null )
		{
			float gap = currentNode.Gap;

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
	}




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

	public ChainNode( float value, float buffer, float bondStrength = 1 )
	{
		this.value = value;
		this.buffer = buffer;
		this.bondStrength = bondStrength;
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
}