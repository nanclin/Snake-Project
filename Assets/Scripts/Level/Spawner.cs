using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	// Static
	public static List<Spawner> spawnerList = new List<Spawner>();

	// System
	private List<Item> itemList = new List<Item>();
	private float lastSpawnTime = 0;

	// Handling
	public float spawnDelay;
	public int maxAlive = 100;
	public int stock = 0;
	private int takenFromStock = 0;
	public bool spawnAtStart;
	public bool area;
	[Range(1,100)] public float range;
	public Item itemPrefab;

// UNITY METHODS ///////////////////////////////////////////////////////////////

	void OnEnable()
	{
		spawnerList.Add( this );
	}

	void OnDisable()
	{
		spawnerList.Remove( this );
	}

	// Use this for initialization
	void Start ()
	{
		Init();
	}
	
	// Update is called once per frame
	void Update ()
	{
		while(
			// spawnAtStart ||
			( Time.time - lastSpawnTime >= spawnDelay ) &&
			( itemList.Count < maxAlive || maxAlive == 0 ) &&
			( takenFromStock < stock || stock == 0 )
		){
			Spawn();
		}

		if( Input.GetKeyDown("p") )
		{
			Init();
		}
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //


	public void Spawn()
	{
		// Keep track of stock
		takenFromStock++;

		// Reset timer
		lastSpawnTime = Time.time;

		// Calculate position
		Vector3 spawnPosition;
		float distance = area ? Random.value * range : 0;
		spawnPosition = transform.position + Quaternion.Euler(0, Random.value * 360, 0) * Vector3.forward * distance;

		// Spawn prefab
		Item instance = Instantiate( itemPrefab, spawnPosition, Quaternion.identity ) as Item;
		instance.spawner = this;
		// Add instance to the list (for later cleanup)
		itemList.Add( instance );
	}

	// Remove all instances created by this Spawner
	public void Cleanup()
	{
		// Destroy every item
		foreach( Item item in itemList )
			DestroyItem( item );

		itemList = new List<Item>();
	}

	// Set to initial state
	public void Init()
	{
		// Cleanup old items
		Cleanup();

		// Reset stock
		takenFromStock = 0;

		// Reset timer
		if( spawnAtStart )
			lastSpawnTime = Time.time - spawnDelay;
		else
			lastSpawnTime = Time.time;
	}

	public void DestroyItem( Item item )
	{
		if( itemList.Count == maxAlive )
			lastSpawnTime = Time.time;

		itemList.Remove( item );
		Destroy( item.gameObject );
	}

	// Draw spawner area range
	void OnDrawGizmos()
	{
		if( area ){
	        // Gizmos.color = Color.red;
	        Gizmos.color = new Color( 1, 0, 0, 0f );
	        Gizmos.DrawSphere( transform.position, range );
	        Gizmos.color = Color.red;
	        Gizmos.DrawWireSphere( transform.position, range );
		}
		else{
	        // Gizmos.color = Color.red;
	        Gizmos.color = new Color( 1, 0, 0, 0f );
	        Gizmos.DrawCube( transform.position, Vector3.one );
	        Gizmos.color = Color.red;
	        Gizmos.DrawWireCube( transform.position, Vector3.one );
		}
    }
}
