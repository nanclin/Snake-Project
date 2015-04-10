using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

	// Global
	public static int ROUND_DECIMALS = 5;
	

	public Transform prefab;
	private SnakeSkeleton skeleton;

	// Use this for initialization
	void Start ()
	{
		skeleton = new SnakeSkeleton();

	}
	
	private float a = 0;
	public float speed = 1;

	// Update is called once per frame
	void Update ()
	{
		
		// Move point
		a += speed * Time.deltaTime;
		// a += 0.1f;

		// Handle positions off of skeleton
		int n = 1;
		while( a > skeleton.length ){
			a -= skeleton.length;
			print( "jump " + n++ );
		}
		while( a < 0 ){
			a += skeleton.length;
			print( "jump " + n++ );
		}

		// Get and apply data
		SkeletonPointData data = skeleton.GetPointOnSkeleton( a );
		// print( data );
		prefab.position = data.point;
		prefab.rotation = data.angle;



		skeleton.Draw();
		
	}
}
