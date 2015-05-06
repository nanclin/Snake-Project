using UnityEngine;
using UnityEditor;

/**
 * Source: http://catlikecoding.com/unity/tutorials/curves-and-splines/
 */

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
	private BezierCurve curve;
	private Transform handleTransform;
	private Quaternion handleRotation;

	// Executes when mouse moves on editor window
	private void OnSceneGUI()
	{
		curve = target as BezierCurve;

		// Take targets transform into account
		handleTransform = curve.transform;
		// Rotate handles relative to world or transform
		handleRotation = ( Tools.pivotRotation == PivotRotation.Local ) ? handleTransform.rotation : Quaternion.identity;

		// Show points
		Vector3 p0 = ShowPoint( 0 );
		Vector3 p1 = ShowPoint( 1 );
		Vector3 p2 = ShowPoint( 2 );
		Vector3 p3 = ShowPoint( 3 );

		// Draw handle lines
		Handles.color = Color.white * 0.8f;
		Handles.DrawLine( p0, p1 );
		Handles.DrawLine( p2, p3 );

		// Draw curve
		Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
	}

	// Shows and handles point handle
	private Vector3 ShowPoint( int index )
	{
		// Get point from curve at given index, and convert it to transforms space
		Vector3 point = handleTransform.TransformPoint( curve.points[ index ] );

		// Assing handle position back to point on curve
		EditorGUI.BeginChangeCheck();
		point = Handles.DoPositionHandle( point, handleRotation );
		if( EditorGUI.EndChangeCheck() )
		{
			// Before making change, record state
			// and set state to changed/dirty
			Undo.RecordObject( curve, "Move Point" );
			EditorUtility.SetDirty( curve );
			// Transform from world space back to lines' local space
			curve.points[ index ] = handleTransform.InverseTransformPoint( point );
		}
		return point;
	}
}