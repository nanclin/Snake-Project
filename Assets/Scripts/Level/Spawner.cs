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
		// Initialize at the begining
		Init();
	}
	
	// Update is called once per frame
	void Update ()
	{
		while(
			( Time.time - lastSpawnTime >= spawnDelay ) &&
			( itemList.Count < maxAlive || maxAlive == 0 ) &&
			( takenFromStock < stock || stock == 0 )
		){
			SpawnItem();
		}

		if( Input.GetKeyDown("p") )
		{
			Init();
		}
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// STATIC METHODS ///////////////////////////////////////////////////////////////

	// Sets all the spawners to initial state
	public static void ResetSpawners()
	{
		foreach( Spawner spawner in spawnerList )
			spawner.Init();
	}
//////////////////////////////////////////////////////////// EO STATIC METHODS //


// SPAWNER CONTROLS ///////////////////////////////////////////////////////////////

	// Set to initial state
	public void Init()
	{
		// Remove all instances created by this Spawner
		ItemCleanup();

		// Reset stock
		takenFromStock = 0;

		// Reset timer
		if( spawnAtStart )
			lastSpawnTime = Time.time - spawnDelay;
		else
			lastSpawnTime = Time.time;
	}

	// Spawn item
	public void SpawnItem()
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
	public void ItemCleanup()
	{
		// Destroy every item
		foreach( Item item in itemList )
			DestroyItem( item );

		itemList = new List<Item>();
	}


	// Item is destroyed by Spawner to cleanup references
	public void DestroyItem( Item item )
	{
		// Reset timer
		if( itemList.Count == maxAlive )
			lastSpawnTime = Time.time;

		// Destroy item
		itemList.Remove( item );
		Destroy( item.gameObject );
	}
//////////////////////////////////////////////////////////// EO SPAWNER CONTROLS //

	// Draw spawner area range
	void OnDrawGizmos()
	{
		if( area ){
			// Draw invisible layer to enable interactive selection in editor
	        Gizmos.color = new Color( 1, 0, 0, 0f );
	        Gizmos.DrawSphere( transform.position, range );
	        // Draw wireframe
	        Gizmos.color = Color.red;
	        Gizmos.DrawWireSphere( transform.position, range );
		}
		else{
			// Draw invisible layer to enable interactive selection in editor
	        Gizmos.color = new Color( 1, 0, 0, 0f );
	        Gizmos.DrawCube( transform.position, Vector3.one );
	        // Draw wireframe
	        Gizmos.color = Color.red;
	        Gizmos.DrawWireCube( transform.position, Vector3.one );
		}
    }
}
