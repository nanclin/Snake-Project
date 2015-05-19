using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Attrators should not overlap
 */

public class CameraAttractor : MonoBehaviour {

	public static List<CameraAttractor> attractors = new List<CameraAttractor>();

	public float innerRange = 5;
	public float outerRange = 10;

// ADD/REMOVE ATTRATORS FROM LIST ///////////////////////////////////////////////////////////////

	void OnEnable()
	{
		attractors.Add( this );
	}

	void OnDisable()
	{
		attractors.Remove( this );
	}
//////////////////////////////////////////////////////////// EO ADD/REMOVE ATTRATORS FROM LIST //

	public static Vector3 GetAttractorsVector( Vector3 playerPos, Vector3 forwardPos )
	{
		Vector3 sumVector = Vector3.zero;
		int count = 0;

		// Loop trough all attractors
		foreach( CameraAttractor attractor in attractors )
		{
			Color color = Color.white * 2/3;

			Vector3 targetVector = playerPos;
			Vector3 targetHalf = playerPos;
			Vector3 forwardHalf = playerPos;
			
			float dis = (playerPos - attractor.transform.position).magnitude;
			float influence = 0;

			// Calculate influence value from distance of target from center of attractor
			
			// 1) Out of range
			if( dis > attractor.outerRange ){
				influence = 0;
			}
			else{

				// 2) Outer ring area
				if( dis < attractor.outerRange && dis > attractor.innerRange ){
					influence = 1 - (dis - attractor.innerRange) / (attractor.outerRange - attractor.innerRange);
				}
				// 3) Inner area
				else if( dis < attractor.innerRange ){
					influence = 1;
					targetVector = attractor.transform.position;
				}

				// Half point between player and its forward vector
				forwardHalf = Vector3.Lerp( playerPos, forwardPos, 1-influence );
				if( CameraSystem.DEBUG ) MyDraw.DrawCircle( forwardHalf, 0.5f/2, Color.green );

				// Get point on line between target and attractor (with influence)
				targetVector = Vector3.Lerp( forwardHalf, attractor.transform.position, influence );
				if( CameraSystem.DEBUG ) MyDraw.DrawSquare( targetVector, 0.5f, color );
				
				// Half point between player and target vector
				targetHalf = Vector3.Lerp( forwardHalf, targetVector, influence );
				if( CameraSystem.DEBUG ) MyDraw.DrawCircle( targetHalf, 0.5f/2, Color.white * 2/3 );


				// DRAW DEBUG
				if( CameraSystem.DEBUG ) Debug.DrawLine( forwardHalf, attractor.transform.position, color );


				// Add vector to sum of attractor vectors
				// and count how many have been added (for averaging purposes)
				sumVector += targetHalf;
				count++;
			}
		}
		// Return average vector of all attractors
		if( count == 0 )
			return forwardPos;
		return sumVector / count;
	}

// DEBUG ///////////////////////////////////////////////////////////////
	void OnDrawGizmos()
	{
		if( CameraSystem.DEBUG )
		{
			MyDraw.DrawCircle( transform.position, innerRange, Color.white );
			MyDraw.DrawCircle( transform.position, outerRange, Color.white );
		}
	}
//////////////////////////////////////////////////////////// EO DEBUG //
}
