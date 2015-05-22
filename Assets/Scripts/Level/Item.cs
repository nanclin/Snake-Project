using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// public GameObject particles;
	[HideInInspector]
	public Spawner spawner;
	public SpawnManager spawnManager;
	public int nutritionValue = 1;


	void OnTriggerEnter( Collider other )
	{
		switch( other.tag )
		{
			case "Player":
				// if( spawner )
				// 	spawner.DestroyItem( this );
				// else
				// 	Destroy( gameObject );
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
}
