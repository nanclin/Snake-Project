using UnityEngine;
using UnityEditor;

/**
 * Source: http://catlikecoding.com/unity/tutorials/curves-and-splines/
 */

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
	private BezierSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private Tool LastTool = Tool.None;

	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	private bool ShowPositionHandles = false;
	private bool ShowBaked = true;
	private bool ShowOutgoingSpline = true;
	private bool interpolateRotation = true;

// UNITY METHODS ///////////////////////////////////////////////////////////////


	// Executes when mouse moves on editor window
	private void OnSceneGUI()
	{
		// Hide transform tool when control point is selected
		if( selectedIndex > -1 ){
			LastTool = Tools.current;
			Tools.current = Tool.None;
		}
		else{
			Tools.current = LastTool;
		}

		// LOOP ALL OUTGOING SPLINES ///////////////////////////////////////////////////////////////
		
		spline = target as BezierSpline;
		int depth = 1;

		while( spline != null && depth < 100 )
		{
			// Take targets transform into account
			handleTransform = spline.transform;
			// Rotate handles relative to world or transform
			handleRotation = ( Tools.pivotRotation == PivotRotation.Local ) ? handleTransform.rotation : Quaternion.identity;

			// Fade outgoing splines
			float colorModified = Mathf.Max( 1f / depth, 0.5f );

			// SHOW POINTS/ANCHORS AND SPLINE ///////////////////////////////////////////////////////////////
			Vector3 p0 = ShowPoint(0);
			for( int i = 1; i < spline.PointCount; i += 3 )
			{
				Vector3 p1 = ShowPoint( i );
				Vector3 p2 = ShowPoint( i + 1 );
				Vector3 p3 = ShowPoint( i + 2 );
				
				// Draw handle lines
				Handles.color = Color.white * 0.8f * colorModified;
				Handles.DrawLine( p0, p1 );
				Handles.DrawLine( p2, p3 );
				
				// Draw curve
				Handles.DrawBezier( p0, p3, p1, p2, Color.white * colorModified, null, 2f );
				p0 = p3;
			}
			//////////////////////////////////////////////////////////// EO SHOW POINT //


			// SHOW BAKED SPLINE ///////////////////////////////////////////////////////////////
			if( ShowBaked )
			{
				spline.BakeSpline( spline.bakedSteps );

				Handles.color = Color.green * colorModified;

				for ( int i = 0; i < spline.bakedSpline.Count-1; i++ )
				{
					Vector3 a = spline.bakedSpline[ i ];
					Vector3 b = spline.bakedSpline[ i+1 ];
					Handles.DrawLine( a, b );
					Handles.DotCap( 0, a, Quaternion.identity, 0.02f );
				}
			}
			//////////////////////////////////////////////////////////// EO SHOW BAKED SPLINE //


			// Prevent looping to itself
			if( spline.outgoingSpline == (BezierSpline)target )
				break;

			if( !ShowOutgoingSpline )
				break;
				
			// Set next spline
			spline = spline.outgoingSpline;
			depth++;
		}
		//////////////////////////////////////////////////////////// EO LOOP ALL OUTGOING SPLINES //

		// SHOW CURRENT BAKED SPLINE ///////////////////////////////////////////////////////////////

		spline = target as BezierSpline;

		if( ShowBaked )
		{
		// 	// Show smooth angle transition debug
		// 	if( interpolateRotation )
		// 	{
		// 		Handles.color = Color.red * 0.7f;
		// 		Handles.DrawLine( spline.zD, spline.bD );
		// 		Handles.DrawLine( spline.aD, spline.cD );
		// 	}

			// // Show baked spline walker
			// Handles.color = Color.green;
			// TransformData data = spline.GetPointBaked( t * spline.length, interpolateRotation, true );
			// Handles.CubeCap( 0, data.position, data.rotation, 0.5f );
		}
		//////////////////////////////////////////////////////////// EO SHOW CURRENT BAKED SPLINE //

		// // Move tester point along spline
		// Handles.color = Color.white;
		// Handles.CubeCap( 0, spline.GetPoint( t ), Quaternion.identity, 0.5f );
	}

	private float t = 0;

	public override void OnInspectorGUI ()
	{
		// DrawDefaultInspector();

		spline = target as BezierSpline;

		// ADD/REMOVE POINTS SECTION ///////////////////////////////////////////////////////////////

		// Call method when button is pressed
		// and implement undo functionality
		if( GUILayout.Button("Add Curve") )
		{
			Undo.RecordObject( spline, "Add Curve" );
			spline.AddCurve();
			EditorUtility.SetDirty( spline );
		}
		if( GUILayout.Button("Remove Curve") )
		{
			Undo.RecordObject( spline, "Remove Curve" );
			spline.RemoveCurve();
			EditorUtility.SetDirty( spline );
		}


		// OPTIONS SECTION SECTION ///////////////////////////////////////////////////////////////

		// Incoming / outgoing splines
		// spline.incomingSpline = EditorGUILayout.ObjectField( "Incoming Spline", spline.incomingSpline, typeof(BezierSpline), true ) as BezierSpline;
		spline.outgoingSpline = EditorGUILayout.ObjectField( "Outgoing Spline", spline.outgoingSpline, typeof(BezierSpline), true ) as BezierSpline;
		ShowOutgoingSpline = EditorGUILayout.Toggle( "Show Outgoing Splines", ShowOutgoingSpline );

		// Check or uncheck visibility of position handles
		ShowPositionHandles = EditorGUILayout.Toggle( "Show Position Handles", ShowPositionHandles );
		SceneView.RepaintAll();

		// T slider
		t = EditorGUILayout.Slider( "T", t, -1, 2 );


		// BAKE SECTION ///////////////////////////////////////////////////////////////

		GUISpace();
		EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Baked spline");
			ShowBaked = EditorGUILayout.Toggle( ShowBaked );
		EditorGUILayout.EndHorizontal();

		// interpolateRotation = EditorGUILayout.Toggle( "Interpolate Rotation", interpolateRotation );


		EditorGUI.BeginChangeCheck();
		// Set number of baked points
		spline.bakedSteps = EditorGUILayout.IntSlider( "Number of Points", spline.bakedSteps, 1, 100 );

		if( EditorGUI.EndChangeCheck() ){
			spline.BakeSpline( spline.bakedSteps );
		}

		
		if( GUILayout.Button("Bake Spline") )
			spline.BakeSpline( spline.bakedSteps );
	


		// SELECTED POINT SECTION ///////////////////////////////////////////////////////////////

		// Draw info of selected point
		if( selectedIndex >= 0 && selectedIndex < spline.PointCount )
			DrawSelectedPointInspector();


		EditorUtility.SetDirty( target );
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// OTHER METHODS ///////////////////////////////////////////////////////////////
	
	private const float handleSizePoint = 0.05f;
	private const float handleSizeAnchor = 0.025f;
	private const float pickSize = 0.1f;
	
	private int selectedIndex = -1;

	// Shows and handles point handle
	private Vector3 ShowPoint( int index )
	{
		// Get point from curve at given index, and convert it to transforms space
		Vector3 point = handleTransform.TransformPoint( spline.GetPoint( index ) );

		// Show position handles
		if( selectedIndex == index || ShowPositionHandles )
		{
			// Assing handle position back to point on curve
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle( point, handleRotation );
			if( EditorGUI.EndChangeCheck() )
			{
				// Before making change, record state
				// and set state to changed/dirty
				Undo.RecordObject( spline, "Move Point" );
				EditorUtility.SetDirty( spline );
				// Transform from world space back to lines' local space
				spline.SetPoint( index, handleTransform.InverseTransformPoint( point ) );
			}
		}

		// Show custom handles
		else
		{
			// Handle settings
			Handles.color = modeColors[ (int)spline.GetPointMode(index) ];
			float size = HandleUtility.GetHandleSize(point);
			float handleSize = ( index%3==0 ) ? handleSizePoint : handleSizeAnchor;

			// Select handle
			if( Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap ) ){
				selectedIndex = index;
				Repaint();
			}

		}
		return point;
	}

	private void DrawSelectedPointInspector()
	{	
		GUISpace();

		GUILayout.Label("Selected Point");

		GUILayout.BeginHorizontal();
			// Select next/previous point/anchor
			if( GUILayout.Button("Previous") && selectedIndex - 1 >= 0 )
				selectedIndex--;
			if( GUILayout.Button("Next")     && selectedIndex + 1 < spline.PointCount )
				selectedIndex++;

			// Deselect points
			if( GUILayout.Button("Deselect") ){
				selectedIndex = -1;
				return;
			}
		GUILayout.EndHorizontal();

		// Position field
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field( "Position", spline.GetPoint( selectedIndex ) );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RecordObject( spline, "Move Point" );
			EditorUtility.SetDirty( spline );
			spline.SetPoint( selectedIndex, point );
		}

		// Control mode dropdown
		EditorGUI.BeginChangeCheck();
		BezierPointMode mode = (BezierPointMode)EditorGUILayout.EnumPopup( "Mode", spline.GetPointMode( selectedIndex ) );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RecordObject( spline, "Change Point Mode" );
			spline.SetPointMode( selectedIndex, mode );
			EditorUtility.SetDirty( spline );
		}
	}

	private void GUISpace( float space = 5 )
	{
		GUILayout.Space( space );
		GUILayout.Box( "", GUILayout.ExpandWidth(true), GUILayout.Height(2) );
		GUILayout.Space( space );
	}
//////////////////////////////////////////////////////////// EO OTHER METHODS //
}