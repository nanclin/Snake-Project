using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Chain : MonoBehaviour {

	public Transform prefab;

	[Range(0.0f, 1.0f)]
	public float bondStrength;
	[Range(0.0f, 2.0f)]
	public float buffer;

	public Node head;
	public Node tail;


	void Awake()
	{
		for( int i = 0; i < 5; i++ ) {
			addLast( new Node( -i, buffer, bondStrength, Instantiate( prefab, Vector3.zero, Quaternion.identity) as Transform ) );
		}
		printNodes( head );
	}

	void Update()
	{
		changeValue( head, 0.15f * Input.GetAxis("Vertical") );
	}

	public void addLast( Node newNode )
	{
		if( head == null ) {
			head = newNode;
		}

		if( tail != null ) {
			tail.next = newNode;
		}
		tail = newNode;
	}

	// Print all nodes, starting with currentNode
	// (example of recursion)
	public void printNodes( Node currentNode )
	{
		print( currentNode.value );
		
		// Continoue with the next Node
		if( currentNode.next != null ) {
			printNodes( currentNode.next );
		}
	}

	public void changeValue( Node currentNode, float valueChange )
	{
		currentNode.value += valueChange;
		currentNode.prefab.position = new Vector3( 0, 0, currentNode.value );

		// Continoue with the next Node
		if( currentNode.next != null ) {

			float dif = currentNode.value - currentNode.next.value - currentNode.buffer - currentNode.next.buffer;

			// Make nodes move only forward
			// if( dif > 0 )
				changeValue( currentNode.next, dif * currentNode.next.bondStrength );
		}

	}
}

public class Node
{
	// Handling
	public float buffer;
	public float bondStrength;

	// System
	public float value;
	public Node next;
	public Node previous;
	public Transform prefab;

	public Node( float value, float buffer, float bondStrength, Transform prefab )
	{
		this.value = value;
		this.prefab = prefab;
		this.buffer = buffer;
		this.bondStrength = bondStrength;
	}
}