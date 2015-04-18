using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

	// Global
	public static int ROUND_DECIMALS = 5;

	// Debug
	public Transform marker;



	private SnakeSkeleton skeleton = new SnakeSkeleton();

	// Use this for initialization
	void Start ()
	{
		for( int i = 0; i <= 5; i++ ){
			skeleton.AppendJoint( new Vector3( i, 0, 0 ) );
		}
		print( skeleton.ToString() );
	}

	// Update is called once per frame
	void Update ()
	{
		if( Time.frameCount > 1 ){
			// skeleton.ShaveStart( 1f );
			// skeleton.ShaveEnd( 5f );
			// skeleton.TrimStart( 3f );
			// skeleton.TrimEnd( 3f );
			print( skeleton.ToString() );
		}

		skeleton.Draw();
	}

}
