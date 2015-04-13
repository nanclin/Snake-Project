using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class Chain : MonoBehaviour {

	public Transform prefab;

	[Range(0.0f, 1.0f)]
	public float bondStrength = 0.5f;
	[Range(0.0f, 2.0f)]
	public float buffer = 0.3f;

	public ChainNode head;
	public ChainNode tail;


	void Start()
	{
		// printNodes( head );
	}

	void Update()
	{
		// moveChain( head, 0.15f * Input.GetAxis("Vertical") );
	}

	public void addLast( ChainNode newNode )
	{
		if( head == null ) {
			head = newNode;
		}

		if( tail != null ) {
			tail.next = newNode;
		}
		tail = newNode;
	}

	// // Print all nodes, starting with currentNode
	// // (example of recursion)
	// public void printNodes( ChainNode currentNode )
	// {
	// 	print( currentNode.value );
		
	// 	// Continoue with the next ChainNode
	// 	if( currentNode.next != null ) {
	// 		printNodes( currentNode.next );
	// 	}
	// }

	public void moveChain( ChainNode currentNode, float moveBy )
	{
		currentNode.value += moveBy;

		// // Debug purposes
		// currentNode.prefab.position = new Vector3( 0, 0, currentNode.value );

		// Continoue with the next ChainNode
		if( currentNode.next != null ) {
			float dif = currentNode.value - currentNode.next.value - currentNode.buffer - currentNode.next.buffer;

			// Make nodes move only forward
			if( dif > 0 )
				moveChain( currentNode.next, dif * currentNode.next.bondStrength );
		}

	}
}

public class ChainNode
{
	// Components
	public Transform prefab;

	// Handling
	public float buffer;
	public float bondStrength;

	// System
	public float value;
	public ChainNode next;
	public ChainNode previous;

	public ChainNode( float value, float buffer, float bondStrength )
	{
		this.value = value;
		this.buffer = buffer;
		this.bondStrength = bondStrength;

		// // Debug purposes
		// this.prefab.position = new Vector3( 0, 0, this.value );

		// Debug.Log("New node created. Initial value is " + this.value );
	}
}