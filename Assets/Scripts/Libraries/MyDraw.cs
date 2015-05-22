using UnityEngine;
using System.Collections;

public class MyDraw
{

	// private static float[] angles = new float[] { -2f, 2f };
	private static float[] angles = new float[] { 0f };
	private static int STEP = 0;


	public static void DrawLine( Vector3 position, Color color, float radius = 0.05f, float duration = 0 )
	{
		float angle = ( 90 + angles[ STEP % angles.Length ] ) / 180 * Mathf.PI;

		// Make every other line little longer
		// (for debbguing purposes in case joints land very close together)
		float alt = ( (int)( STEP % 2 ) * radius * 0.3f );

		float x1 = Mathf.Sin( angle ) *  ( radius + alt );
		float y1 = Mathf.Cos( angle ) *  ( radius + alt );
		float x2 = Mathf.Sin( angle ) * - ( radius + alt );
		float y2 = Mathf.Cos( angle ) * - ( radius + alt );

		Debug.DrawLine (new Vector3(position.x + x1, position.y, position.z + y1), new Vector3(position.x + x2, position.y, position.z + y2), color , duration);

		STEP++;
	}

	public static void DrawLineThroughPoint( Vector3 point, Vector3 direction, Color color, float length = 0.2f )
	{
		Vector3 a = point - direction * length / 2;
		Vector3 b = point + direction * length / 2;
		Debug.DrawLine ( a, b, color);
	}

	public static void DrawSquare(Vector3 center, float radius, Color color )
	{
		Vector3 prevPoint = Quaternion.Euler(0, 45, 0) * Vector3.forward;
		prevPoint = prevPoint.normalized * radius;
		prevPoint = prevPoint * Mathf.Sqrt( 2 );

		for( int i = 0; i <= 360; i += 360/4 )
		{
			Vector3 currentPoint = Quaternion.Euler(0, i + 45, 0) * Vector3.forward;
			currentPoint = currentPoint.normalized * radius;
			currentPoint = currentPoint * Mathf.Sqrt( 2 );
			
			Debug.DrawLine( center + prevPoint, center + currentPoint, color );

			prevPoint = currentPoint;
		}
	}

	public static void DrawCircle(Vector3 center, float radius, Color color )
	{
		Vector3 prevPoint = Quaternion.Euler(0, 45, 0) * Vector3.forward;
		prevPoint = prevPoint.normalized * radius;

		for( int i = 0; i <= 360; i += 360/36 )
		{
			Vector3 currentPoint = Quaternion.Euler(0, i + 45, 0) * Vector3.forward;
			currentPoint = currentPoint.normalized * radius;
			
			Debug.DrawLine( center + prevPoint, center + currentPoint, color );

			prevPoint = currentPoint;
		}
	}

	public static void DrawCross( Vector3 center, float radius, Color color )
	{
		DrawLineThroughPoint( center, Vector3.forward, color, radius );
		DrawLineThroughPoint( center, Vector3.right, color, radius );
	}


	public static void DrawVector( Vector3 initPosition, Vector3 vector, Vector3 up, Color color, bool normalized = false, bool rightHand = true )
	{
		if( normalized )
			vector = vector.normalized;

		Debug.DrawLine( initPosition, initPosition + vector, color );

		if( rightHand )
			Debug.DrawLine( initPosition, initPosition + -Vector3.Cross( vector, up ), color );
	}
}
