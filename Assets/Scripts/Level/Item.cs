using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour {

	public static List<Item> instances = new List<Item>();

	// public GameObject particles;
	[HideInInspector]
	public Spawner spawner;
	public SpawnManager spawnManager;
	public int nutritionValue = 1;

//////////////////////////////////////////////////////////// EO MANAGE LIST OF INSTANCES //
	void Awake()
	{
		instances.Add( this );
	}
	void OnDestroy()
	{
		instances.Remove( this );
	}
// MANAGE LIST OF INSTANCES ///////////////////////////////////////////////////////////////

// METHODS ///////////////////////////////////////////////////////////////
// 
	public static Vector3 GetClosest( Vector3 position )
	{
		// Get closest item
		Vector3 closestLocation = Vector3.zero;
		float closestDistance = Mathf.Infinity;

		foreach( Item item in instances )
		{
			Vector3 location = item.transform.position;

			float distance = (location - position).magnitude;
			if( distance < closestDistance )
			{
				closestDistance = distance;
				closestLocation = location;
			}

			// Debug.DrawLine( location, position, Color.white * 0.6f );
		}
		return closestLocation;
	}
//////////////////////////////////////////////////////////// EO METHODS //

// COLLISION ///////////////////////////////////////////////////////////////
	void OnTriggerEnter( Collider other )
	{
		switch( other.tag )
		{
			case "Player":
				if( spawner )
					spawner.DestroyItem( this );
				else
					Destroy( gameObject );

				if( spawnManager )
					spawnManager.DestroyItem( this );
				break;
			default:
				// print("Item was hit by something not handeld by code!");
				break;
		}
	}

	void OnCollisionEnter( Collision col )
	{
		if( col.gameObject.tag == "Ground" ){
			transform.GetComponent<Collider>().isTrigger = true; 
			transform.GetComponent<Rigidbody>().isKinematic = true; 
		}
	}
//////////////////////////////////////////////////////////// EO COLLISION //
}
