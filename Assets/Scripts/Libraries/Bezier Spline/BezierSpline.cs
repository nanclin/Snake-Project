using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * A Beziér curve is defined by a sequence of points.
 * It starts at the first point and ends at the last point,
 * but does not need to go through the intermediate points.
 * Instead, those points pull the curve away from being a straight line.
 * Source: http://catlikecoding.com/unity/tutorials/curves-and-splines/
 */

[System.Serializable]
public class BezierSpline : MonoBehaviour
{
	[SerializeField] private Vector3[] points;
	[SerializeField] private BezierPointMode[] modes;
	// public BezierSpline incomingSpline;
	[SerializeField] public BezierSpline outgoingSpline;

// UNITY METHODS ///////////////////////////////////////////////////////////////

	// Special Unity method, which is called by the editor
	// when the component is CREATED or RESET.
	// Initializes BezierCurve with three points
	public void Reset()
	{
		points = new Vector3[] {
			new Vector3( 1f, 0f, 0f ),
			new Vector3( 2f, 0f, 0f ),
			new Vector3( 3f, 0f, 0f ),
			new Vector3( 4f, 0f, 0f )
		};

		// Set default values of new points
		modes = new BezierPointMode[] {
			BezierPointMode.Aligned,
			BezierPointMode.Aligned
		};
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// ADD/REMOVE CURVE ///////////////////////////////////////////////////////////////
	
	public void AddCurve ()
	{
		// First, select last point on spline
		// and use it as first point for new curve
		Vector3 point = points[ points.Length - 1 ];

		// Resize array
		Array.Resize( ref points, points.Length + 3 );

		// Store three new points
		point.x += 1f;
		points[ points.Length - 3 ] = point;
		point.x += 1f;
		points[ points.Length - 2 ] = point;
		point.x += 1f;
		points[ points.Length - 1 ] = point;

		// Resize length of anchor modes
		Array.Resize( ref modes, modes.Length + 1 );
		modes[ modes.Length - 1 ] = modes[ modes.Length - 2 ];

		// Align new point with the previous
		EnforceMode( points.Length - 4 );
		BakeSpline();
	}

	public void RemoveCurve()
	{
		// Resize length of anchor modes
		Array.Resize( ref points, points.Length - 3 );
		Array.Resize( ref modes, modes.Length - 1 );
	}
//////////////////////////////////////////////////////////// EO ADD/REMOVE CURVE //

// GET POINT ///////////////////////////////////////////////////////////////

	public Vector3 GetPoint( float t )
	{
		int i;
		if( t >= 1f ) {
			t = 1f;
			i = points.Length - 4;
		}
		else{
			// Clamps value between 0 and 1 and returns value
			t = Mathf.Clamp01( t ) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint( Bezier.GetPoint( points[ i ], points[ i + 1 ], points[ i + 2 ], points[ i + 3 ], t ) );
	}

	// For drawing debug lines
	public Vector3 zD, aD, bD, cD;

	public TransformData GetPointBaked( float t, bool interpolateRotation = true, bool debug = false )
	{
		float traversed = 0;

		TransformData data = new TransformData();

		if( t > length){
			data.position = GetPoint( 1f );
			// Convert from this.transforms local space to world space
			Vector3 p0 = transform.TransformPoint( points[ PointCount-1 ] );
			Vector3 p1 = transform.TransformPoint( points[ PointCount-2 ] );
			data.rotation = Quaternion.LookRotation( p0 - p1 );
		}
		else if( t < 0 ){
			data.position = GetPoint( 0f );
			// Convert from this.transforms local space to world space
			Vector3 p0 = transform.TransformPoint( points[0] );
			Vector3 p1 = transform.TransformPoint( points[1] );
			data.rotation = Quaternion.LookRotation( p1 - p0 );
		}
		else{
			for( int i = 0; i < bakedSpline.Count - 1; i++ )
			{
				// Get current bone
				Vector3 a = bakedSpline[ i ];
				Vector3 b = bakedSpline[ i + 1 ];
				if(debug) aD = a;
				if(debug) bD = b;
				Vector3 bone = b - a;

				traversed += bone.magnitude;

				if( traversed >= t )
				{
					// MOVE ///////////////////////////////////////////////////////////////

					// Move to the point on the middle of the bone
					float remaining = bone.magnitude - (traversed - t);
					data.position = a + bone.normalized * remaining;


					// ROTATE ///////////////////////////////////////////////////////////////

					// How far along the bone t has come (from 0 to 1 format)
					float zeroToOne = remaining / bone.magnitude;

					// Points before and after points a and b
					Vector3 z = bakedSpline[ Mathf.Max( i - 1, 0 ) ];
					Vector3 c = bakedSpline[ Mathf.Min( i + 2, bakedSpline.Count-1 ) ];
					if(debug) zD = z;
					if(debug) cD = c;

					// Rotations on a and b peaks
					Quaternion r1 = Quaternion.LookRotation( b-z );
					Quaternion r2 = Quaternion.LookRotation( c-a );

					// For end points get rotation from anchor points
					if( i-1 < 0 ){
						Vector3 p0 = transform.TransformPoint( points[0] );
						Vector3 p1 = transform.TransformPoint( points[1] );
						r1 = Quaternion.LookRotation( p1 - p0 );
					}
					if( i+2 > bakedSpline.Count-1 ){
						Vector3 p0 = transform.TransformPoint( points[ points.Length-2 ] );
						Vector3 p1 = transform.TransformPoint( points[ points.Length-1 ] );
						r2 = Quaternion.LookRotation( p1 - p0 );
					}
					// Lerp rotation from previous to next rotations,
					// or snap it to the baked line
					data.rotation = interpolateRotation ? Quaternion.Lerp( r1, r2, zeroToOne ) : Quaternion.LookRotation( bone );

					break;
				}
		 	}
		}
	 	return data;
	}

	// private int chainCurveCount;
	// private Vector3[] pointsChain;

	// private Vector3[] CombineSplines()
	// {
	// 	pointsChain = points;
	// 	chainCurveCount = CurveCount;
	// 	BezierSpline currentSpline = outgoingSpline;

	// 	int safe = 100;
	// 	while( currentSpline != null && safe-- > 0 )
	// 	{
	// 		print( "currentSpline: " + currentSpline );
	// 		// int oldLength = pointsChain.Length;
	// 		Array.Resize( ref pointsChain, pointsChain.Length + currentSpline.points.Length );
	// 		currentSpline.points.CopyTo( pointsChain, currentSpline.points.Length );

	// 		chainCurveCount += currentSpline.CurveCount;
	// 		currentSpline = currentSpline.outgoingSpline;
	// 	}

	// 	return pointsChain;
	// }

	public List<Vector3> bakedSpline;
	public float length;
	public int bakedSteps;

	public void BakeSpline( int bakedSteps = 10 )
	{
		bakedSpline = new List<Vector3>();

		length = 0;
		// bakedSteps--;
		for( int i = 0; i <= bakedSteps; i++ )
		{
			float t = (float)i / (float)bakedSteps;
			bakedSpline.Add( GetPoint( t ) );

			if( i > 0)
				length += ( bakedSpline[ i - 1 ] - bakedSpline[ i ] ).magnitude;
		}
	}
//////////////////////////////////////////////////////////// EO GET POINT //

	private void EnforceMode( int index )
	{
		int modeIndex = (index + 1) / 3;

		BezierPointMode mode = modes[ modeIndex ];

		// Skip enforcing for FREE points and START/END points
		if( mode == BezierPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1 )
			return;

		// Get indexes
		int pointIndex = modeIndex * 3;
		int holdingIndex;
		int enforcedIndex;

		if( index <= pointIndex ){
			holdingIndex = pointIndex - 1;
			enforcedIndex = pointIndex + 1;
		}
		else {
			holdingIndex = pointIndex + 1;
			enforcedIndex = pointIndex - 1;
		}


		Vector3 point = points[ pointIndex ];
		Vector3 enforcedTangent = point - points[ holdingIndex ];
		// Keep length of enforced anchor
		// in case of Aligned mode
		if( mode == BezierPointMode.Aligned )
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance( point, points[ enforcedIndex ] );
		points[ enforcedIndex ] = point + enforcedTangent;
	}

// GETTERS/SETTER ///////////////////////////////////////////////////////////////

	// Returns number of curves used in spline
	public int CurveCount
	{
		get{
			return (points.Length - 1) / 3;
		}
	}

	// Returns number of points/anchors of current spline
	public int PointCount
	{
		get{
			return points.Length;
		}
	}

	// CONTROL POINTS SET/GET ///////////////////////////////////////////////////////////////

	public Vector3 GetPoint( int index )
	{
		return points[ index ];
	}

	public void SetPoint( int index, Vector3 point )
	{
		// In case of main point is moving
		if( index % 3 == 0 )
		{
			// Move neighbour anchor points along with point
			Vector3 delta = point - points[ index ];
			if( index > 0 )
				points[ index - 1 ] += delta;
			if( index + 1 < points.Length )
				points[ index + 1 ] += delta;
		}

		// Asign new position and enforce mode
		points[ index ] = point;
		EnforceMode( index );
		BakeSpline( bakedSteps );
	}
	//////////////////////////////////////////////////////////// EO CONTROL POINTS SET/GET //


	// POINT MODE SET/GET ///////////////////////////////////////////////////////////////
	public BezierPointMode GetPointMode( int index )
	{
		return modes[ (index + 1) / 3 ];
	}

	public void SetPointMode( int index, BezierPointMode mode )
	{
		modes[ (index + 1) / 3 ] = mode;
		EnforceMode( index );
		BakeSpline( bakedSteps );
	}
	//////////////////////////////////////////////////////////// EO POINT MODE SET/GET //
//////////////////////////////////////////////////////////// EO GETTERS/SETTER //
}

// TransformData ///////////////////////////////////////////////////////////////

public struct TransformData {

	public Vector3 position;
	public Quaternion rotation;

	override public string ToString ()
	{
		string s = "";
		
		s += " position:\t"         + position        + "\n";
		s += " rotation:\t"         + rotation        + "\n";
		
		return s;
	}
}
//////////////////////////////////////////////////////////// EO TransformData //