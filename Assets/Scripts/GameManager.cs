using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	// Global
	public static int ROUND_DECIMALS = 5;
	
	private Chain chain = new Chain();

	// Debug
	public Transform marker;

	// Use this for initialization
	void Start ()
	{
		// Head ChainNode
		chain.AddLast( new ChainNode( 0, 0.5f ) );

		// Initial ChainNodes
		for( int i = 1; i < 3; i++ ) {
			chain.AddLast( new ChainNode( -2f * i, 0.5f, 0.2f ) );
		}
	}

	// Update is called once per frame
	void Update ()
	{
		// Grow new ChainNodes
		if( Time.frameCount%50 == 0 && chain.Count < 7 )
			chain.AddLast( new ChainNode( chain.tail.value, UnityEngine.Random.Range( 0.3f, 0.7f ), 0.2f ) );


		// Move chain
		// if( Time.frameCount > 1 )
			// chain.MoveChain( chain.head, 0.1f );
			chain.MoveChain( chain.head, 0.1f * Input.GetAxis("Horizontal") );

		// Draw Chain
		chain.DrawChain( chain.head );
	}

}
