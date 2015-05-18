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
			Vector3 targetVector = playerPos;
			float dis = (playerPos - attractor.transform.position).magnitude;
			float influence = 0;

			// Calculate influence value from distance of target from center of attractor
			
			// 1) Out of range
			if( dis > attractor.outerRange ){
				influence = 0;
			}
			// 2) Outer ring area
			else if( dis < attractor.outerRange && dis > attractor.innerRange ){
				influence = 1 - (dis - attractor.innerRange) / (attractor.outerRange - attractor.innerRange);
				// Get point on line between target and attractor (with influence)
				targetVector = Vector3.Lerp( playerPos, attractor.transform.position, influence );
			}
			// 3) Inner area
			else if( dis < attractor.innerRange ){
				influence = 1;
				targetVector = attractor.transform.position;
			}
			
			// Apply influence
			if( influence > 0 )
			{
				// Draw debug gizmos
				MyDraw.DrawSquare( targetVector, 0.5f, Color.white/2 );
				Debug.DrawLine( playerPos, attractor.transform.position, Color.white/2 );

				// Keep camera halfway between target and player (with influence)
				targetVector = Vector3.Lerp( targetVector, playerPos, 1-influence );

				// Add vector to sum of attractor vectors
				// and count how many have been added (for averaging purposes)
				sumVector += targetVector;
				count++;
			}
		}
		// Return average vector of all attractors
		return sumVector / count;
	}

// DEBUG ///////////////////////////////////////////////////////////////
	void OnDrawGizmos()
	{
		// Gizmos.DrawWireSphere( transform.position, innerRange );
		// Gizmos.DrawWireSphere( transform.position, outerRange );
		MyDraw.DrawCircle( transform.position, innerRange, Color.white );
		MyDraw.DrawCircle( transform.position, outerRange, Color.white );
	}
//////////////////////////////////////////////////////////// EO DEBUG //
}
