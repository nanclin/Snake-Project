using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// public GameObject particles;
	[HideInInspector]
	public Spawner spawner;
	public int nutritionValue = 1;


	void OnTriggerEnter( Collider other )
	{
		switch( other.tag )
		{
			case "Player":
				spawner.DestroyItem( this );
				break;
			default:
				print("Item was hit by something not handeld by code!");
				break;
		}
	}
}
