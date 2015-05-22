using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bush : SpawnManager
{
	private Vector3 initPos;

	private float shakeSpeed = 20f;
	private float shakeForce = 0.1f;
	// public Item itemPrefab;
	
	[System.Serializable]
	public class Timing {
		public float growDelayMin = 1;
		public float growDelayMax = 1;

		public float spawnDelayMin = 0.5f;
		public float spawnDelayMax = 0.5f;
	}

	private float currentGrowDelay;
	private float currentGrowTime;
	private float currentSpawnTime;
	private float currentSpawnDelay;
	
	public Timing timing;

	private Transform graphicsRef;
	private List<GameObject> itemsOnBush = new List<GameObject>();
	private List<GameObject> itemsOffBush = new List<GameObject>();

// UNITY METHODS ///////////////////////////////////////////////////////////////
	void Awake()
	{
		initPos = transform.position;
		graphicsRef = transform.Find( "Bush Graphics" ).transform;

		// Randomly rotate
		transform.Rotate( Vector3.up * Random.value * 360f );
	}

	void Update()
	{
		Shaking();
		Growing();
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// INIT ///////////////////////////////////////////////////////////////
	override public void Init()
	{
		base.Init();

		// Prepare items graphics
		foreach( Transform itemGraphics in transform.Find( "Bush Graphics" ) )
		{
			if( itemsOnBush.Count < currentStock )
				ShowGraphicOnBush( itemGraphics.gameObject );
			else
				HideGraphicOnBush( itemGraphics.gameObject );
		}

		// Reset grow timer
		currentGrowTime = 0;
		currentGrowDelay = Random.Range( timing.growDelayMin, timing.growDelayMax );

		// Reset spawn timer
		currentSpawnTime = 0;
		currentSpawnDelay = Random.Range( timing.spawnDelayMin, timing.spawnDelayMax );
	}
//////////////////////////////////////////////////////////// EO INIT //

// COLLISION ///////////////////////////////////////////////////////////////
	private int collisionCount = 0;
	private Vector3 collisionDirection;
	private float startTime;

	void OnTriggerEnter( Collider other )
	{
		if( other.tag == "Player" || other.tag == "Snake Cell" )
		{
			if( collisionCount == 0 ){
				startTime = Time.time;
				collisionDirection = other.transform.right;
			}

			collisionCount++;
		}
	}
	void OnTriggerExit( Collider other )
	{
		if( other.tag == "Player" || other.tag == "Snake Cell" )
		{
			collisionCount--;

			if( collisionCount == 0 ){
				graphicsRef.position = initPos;
				currentGrowTime = 0;
			}
		}
	}
//////////////////////////////////////////////////////////// EO COLLISION //

// BUSH FUNCTIONALITIES ///////////////////////////////////////////////////////////////
// 
	public float throwForce = 200;

	
	private void Shaking()
	{
		// print("TODO: Fix DIZZY EXPERIENCE caused by constant shaking of the bush.");
		
		if( collisionCount > 0 )
		{
			// Animate graphics shake
			float t = Time.time - startTime;
			Vector3 pos = initPos + Mathf.Sin( t * shakeSpeed ) * collisionDirection * shakeForce;
			graphicsRef.position = pos;

			// If theres are any berries on the bush to spawn them
			if( itemsOnBush.Count > 0 )
			{
				currentSpawnTime += Time.deltaTime;

				// Spawn condition
				if( currentSpawnTime > currentSpawnDelay )
				{
					// Reset spawn timer
					currentSpawnTime = 0;
					currentSpawnDelay = Random.Range( timing.spawnDelayMin, timing.spawnDelayMax );

					// Get random item from bush
					int id = Random.Range( 0, itemsOnBush.Count );
					GameObject item = itemsOnBush[ id ];
					HideGraphicOnBush( item );

					// Instantiate item
					Item currentItem = SpawnItem( item.transform.position, itemPrefab );

					// Position item and throw it
					Vector3 direction =
						item.transform.forward + Vector3.up * 1f
						+ item.transform.right * Random.value * 1f
						+ item.transform.up    * Random.value * 1f
						;
					currentItem.GetComponent<Rigidbody>().AddForce( direction * throwForce );
				}
			}			
		}
	}

	private void Growing()
	{
		// Grow criterias:
		// - no collision
		// - bush has place to grow
		// - items left in stock
		if( collisionCount == 0 && itemsOffBush.Count > 0 && currentStock - itemsOnBush.Count > 0 )
		{
			// Increase grow progress
			// and check if prgoressed enough to grow
			currentGrowTime += Time.deltaTime;

			if( currentGrowTime > currentGrowDelay )
			{
				// Reset grow timer
				currentGrowTime = 0;
				currentGrowDelay = Random.Range( timing.growDelayMin, timing.growDelayMax );

				// Get on of the off berries
				int id = Random.Range( 0, itemsOffBush.Count );
				GameObject item = itemsOffBush[ id ];

				ShowGraphicOnBush( item );
			}
		}
	}
//////////////////////////////////////////////////////////// EO BUSH FUNCTIONALITIES //

// BUSH GRAPHICS MANAGER ///////////////////////////////////////////////////////////////
	
	private void ShowGraphicOnBush( GameObject itemGraphics )
	{
		itemGraphics.SetActive( true );
		itemsOffBush.Remove( itemGraphics );
		itemsOnBush.Add( itemGraphics );
	}

	private void HideGraphicOnBush( GameObject itemGraphics )
	{
		itemGraphics.SetActive( false );
		itemsOnBush.Remove( itemGraphics );
		itemsOffBush.Add( itemGraphics );
	}
//////////////////////////////////////////////////////////// EO BUSH GRAPHICS MANAGER //

}
