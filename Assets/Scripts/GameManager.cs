using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	// Global
	public static int ROUND_DECIMALS = 5;
	
	// Components
	public SnakeController snake;
	public Transform cellPrefab;
	private SnakeSkeleton skeleton;
	private Chain chain;

	// Debug
	public Transform marker;

	// Use this for initialization
	void Start ()
	{
		// Initialize skeleton and chain
		skeleton = snake.skeleton;
		chain = GetComponent<Chain>();

		// Create initial skeleton
		// skeleton.AppendJoint( transform.up * 0 );
		// skeleton.AppendJoint( transform.up * -5 );

		// Grow cells
		for( int i = 0; i < 10; i++ )
		{
			// Add chain node for current cell
			chain.addLast( new ChainNode( i, chain.buffer, chain.bondStrength ) );
			
			// Instantiate cell and put it on the skeleton
			Transform cell = Instantiate( cellPrefab, Vector3.zero, Quaternion.identity) as Transform;
			SkeletonPointData point = snake.skeleton.GetPointOnSkeleton( chain.tail.value );
			cell.position = point.position;
			cell.rotation = point.rotation;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		skeleton.Draw();
		// print( skeleton.ToString() );	
	}
}
