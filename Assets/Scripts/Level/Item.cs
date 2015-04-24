using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public GameObject particles;
	[HideInInspector]
	public Spawner spawner;
	public int nutritionValue = 1;


	void OnTriggerEnter( Collider other )
	{
		switch( other.tag )
		{
			case "Player":
				Instantiate( particles, transform.position, Quaternion.identity );

				if( spawner != null)
					spawner.numOfInstances--;
					
				Destroy( gameObject );
				break;
			default:
				print("Item was hit by something not handeld by code!");
				break;
		}
	}
}
