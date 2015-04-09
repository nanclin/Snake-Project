using UnityEngine;
using System.Collections;

public class MyDraw : MonoBehaviour {

	// private static float[] angles = new float[] { -2f, 2f };
	private static float[] angles = new float[] { 0f };
	private static int STEP = 0;


	public static void DrawLine(Vector3 position, Color color, float radius = 0.05f, float duration = 0) {

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

	public static void DrawLineThroughPoint( Vector3 point, Color color, float length = 0.01f )
	{
		Vector3 a = point - Vector3.right * length / 2;
		Vector3 b = point + Vector3.right * length / 2;
		Debug.DrawLine ( a, b, color);
	}


	public static void DrawCircle(Vector3 center, Color color, float radius = 1, float duration = 0)
	{
		float prevX = 0 + center.x;
		float prevY = radius + center.y;

		for(int i = 0; i <= 360; i += 360/36){
			float x = Mathf.Cos(Mathf.Deg2Rad * (i+90)) * radius;
			float y = Mathf.Sin(Mathf.Deg2Rad * (i+90)) * radius;
			Debug.DrawLine(new Vector2(prevX, prevY), new Vector2(x,y), color, duration);
			prevX = x + center.x;
			prevY = y + center.y;
		}
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
