using UnityEngine;

/**
 * A Beziér curve is defined by a sequence of points.
 * It starts at the first point and ends at the last point,
 * but does not need to go through the intermediate points.
 * Instead, those points pull the curve away from being a straight line.
 * Source: http://catlikecoding.com/unity/tutorials/curves-and-splines/
 */

public class BezierCurve : MonoBehaviour
{
	public Vector3[] points;

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
	}

	public Vector3 GetPoint( float t ) {
		return transform.TransformPoint( Bezier.GetPoint( points[0], points[1], points[2], points[3], t ) );
	}
}

// Static Bezier class ///////////////////////////////////////////////////////////////
public static class Bezier {

	public static Vector3 GetPoint( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t )
	{
		// Cubic bezier curve equation
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
			3f * oneMinusT * oneMinusT * t * p1 +
			3f * oneMinusT * t * t * p2 +
			t * t * t * p3;
	}
}
//////////////////////////////////////////////////////////// EO Static Bezier class //