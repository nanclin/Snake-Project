using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SnakeSkeleton {

	// private static bool DEBUG = false;

	private float _length = -1;
	public List<Vector3> joints = new List<Vector3>();


	// CONSTRUCTOR ////////////////////////////////////////////////////////

	// Use this for initialization
	public SnakeSkeleton ()
	{

		// PrependJoint( new Vector3(0.0f, 0.0f, 0.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 10f) );


		/* skeleton point demo */

		// Append/Prepend test
		// AppendJoint( new Vector3(0.0f, 0.0f, 0.0f) );
		// AppendJoint( new Vector3(0.0f, 0.0f, 1.0f) );
		// AppendJoint( new Vector3(1.0f, 0.0f, 2.0f) );
		// AppendJoint( new Vector3(1.0f, 0.0f, 3.0f) );
		// PrependJoint( new Vector3(2.0f, 0.0f, 1.0f) );
		// AppendJoint( new Vector3(2.0f, 0.0f, 1.0f) );

		// Simple straight line
		// PrependJoint( new Vector3(0.0f, 0.0f, 0.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 1.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 2.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 3.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 4.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 5.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 6.0f) );

		// Bend line, angle test
		// PrependJoint( new Vector3(0.0f, 0.0f, 0.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 1.0f) );
		// PrependJoint( new Vector3(0.0f, 0.1f, 2.0f) );
		// PrependJoint( new Vector3(0.0f, 0.3f, 3.0f) );
		// PrependJoint( new Vector3(0.0f, 0.9f, 4.0f) );
		// PrependJoint( new Vector3(1.0f, 2.0f, 5.001f) );

		// Skew lines, angle test
		// PrependJoint( new Vector3(0.0f, 0.0f, 0.0f) );
		// PrependJoint( new Vector3(-1.0f, 0.0f, 0.0f) );
		// PrependJoint( new Vector3(-2.0f, 0.0f, 1.0f) );
		// PrependJoint( new Vector3(-1.0f, 0.0f, 2.0f) );
		// PrependJoint( new Vector3(0.0f, 0.0f, 2.0f) );
		// joints.RemoveRange(2,-1);

		// Spiral
		// for( float i=0; i<=360; i+=(360/10) ) {
		// 	PrependJoint( new Vector3( Mathf.Sin(i/180*Mathf.PI), (i/360)*5.0f, Mathf.Cos(i/180*Mathf.PI) ) );
		// }


		// Loop
		// for( float i=0; Math.Round(i,4)<=1f; i+=0.1f ) {
		// 	PrependJoint( new Vector3( 0, 0, i ) );
		// }
	}


	// EO CONSTRUCTOR ////////////////////////////////////////////////////////



	// JOINTS LIST MANIPULATION ////////////////////////////////////////////////////////


	// Add joint to the top of the skeleton, closer to the head
	public void PrependJoint ( Vector3 point )
	{
		// Insert new joint to the beginning of list
		joints.Insert( 0, point );

		// If joint(s) already existed before
		if( joints.Count > 1 ){

			// Take currently added point,
			// figure out how much _length it added,
			float dis = Vector3.Distance( joints[0], joints[1] );

			// add that value to skeleton _length
			_length += dis;
			// _length = (float) Math.Round( _length, GameManager.ROUND_DECIMALS );	// round length to 4 decimal precision
		}

		// If first joint, set length to 0 instead of -1
		else {
			_length = 0;
		}
	}

	// Add joint to the bottom of the skeleton, closer to the tail
	public void AppendJoint ( Vector3 point )
	{
		// Insert new joint to the beginning of list
		joints.Add(point);

		// If joint(s) already existed before
		if(joints.Count > 1){
			// Take currently added point,
			// figure out how much _length it added,
			float dis = Vector3.Distance( joints[ joints.Count-1 ], joints[ joints.Count-2 ] );

			// add that value to skeleton _length
			_length += dis;
			// _length = (float)Math.Round(_length, GameManager.ROUND_DECIMALS);	// round length to 4 decimal precision
		}

		// If first joint, set length to 0 instead of -1
		else {
			_length = 0;
		}
	}



	// EO JOINTS LIST MANIPULATION ////////////////////////////////////////////////////////



	// GET DATA ALONG SKELETON ////////////////////////////////////////////////////////


	// EO GET DATA ALONG SKELETON ////////////////////////////////////////////////////////



	// DEBUG METHODS ////////////////////////////////////////////////////////

	// Draw joints and bones of skeleton data for debugging pursposes
	public void Draw( bool drawBones=true, bool drawJoints=true)
	{
		if( joints.Count > 0 ) {
			// Draw first, head point
			Vector3 a;
			Vector3 b;

			// Debug.Log( joints[0] );

			if( drawJoints ) {
				// MyDraw.DrawLineThroughPoint( joints[ joints.Count-1 ], Color.green );	// draw last joint
				MyDraw.DrawLineThroughPoint( joints[ 0 ], Color.green );					// draw first joint
			}

			for( int i = 0; i < joints.Count-1; i++ ) {

				// Get current and next joint
				a = joints[ i ];
				b = joints[ i+1 ];

				// Draw bones
				if( drawBones ) {
					Debug.DrawLine( a, b, Color.red );
				}

				// Draw joints
				if( drawJoints ) {
					MyDraw.DrawLineThroughPoint( b, Color.blue );
				}
			}
		}


		// // Draw unit markers
		// float alreadyMarkedDistance = 0;
		// while( alreadyMarkedDistance < length ) {
		// 	SkeletonPointData data = GetPointOnSkeleton( alreadyMarkedDistance );
		// 	MyDraw.DrawLine( data.point, Color.blue, r/3) ;
		// 	alreadyMarkedDistance += 1;
		// }
	}


	// Returns data about skeleton in readeable form
	override public string ToString ()
	{
		string str = "\n--- Skeleton ---\n";

		str += "Length:\t"+_length+"\n";

		// str += "First:\t"+joints.First()+"\n";
		// str += "Last:\t"+joints.Last()+"\n";

		// str += "Joints: \n";

		// // Loop through joints FIRST 5
		// for( int i = 0; i < Mathf.Clamp( 5, 0, joints.Count ); i++ ) {
		// 	Vector3 point = joints[i];
		// 	str += "   "+i+": ("+point.x+", "+point.y+", "+point.z+")\n";
		// }
		// if(joints.Count > 5) {
		// 	str += "   ...\n";
		// 	// Loop through joints LAST 5
		// 	int num = Mathf.Clamp( joints.Count-1-5, 5, joints.Count );
		// 	for( int i = num; i < joints.Count; i++ ) {
		// 		Vector3 point = joints[i];
		// 		str += "   "+i+": ("+point.x+", "+point.y+", "+point.z+")\n";
		// 	}
		// }

		return str;
	}

	// EO DEBUG METHODS ////////////////////////////////////////////////////////



	// BASIC SETTERS, GETTTERS ////////////////////////////////////////////////////////

	[SerializeField]
	public Vector3 firstJoint {
		get {
			if( joints.Count > 0 )
				return joints.First();
			else
	        	throw new System.Exception("Skeleton has no joints. Can't return \"first joint\"!");
		}
	}

	[SerializeField]
	public Vector3 lastJoint {
		get {
			if( joints.Count > 0 )
				return joints.Last();
			else
	        	throw new System.Exception("Skeleton has no joints. Can't return \"length joint\"!");
		}
	}


	[SerializeField]
	public float length {
		get {
			if( joints.Count > 0 )
				return _length;
			else
	        	throw new System.Exception("Skeleton has no bone. Can't return \"length\"!");
		}
	}

	// EO BASIC SETTERS, GETTTERS ////////////////////////////////////////////////////////

}
