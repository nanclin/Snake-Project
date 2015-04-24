using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	// System
	[HideInInspector]
	public int numOfInstances = 0;
	public float lastSpawnTime = 0;

	// Handling
	public float spawnDelay = 1;
	public int maxNumOfInstances = 100;
	public bool spawnAtStart;
	public bool area = true;
	public float range = 1;
	public List<Item> prefabList = new List<Item>();
	public List<float> prefabRare = new List<float>();


	// Use this for initialization
	void Start () {
		if( spawnAtStart )
			lastSpawnTime = -spawnDelay*2;
	}
	
	// Update is called once per frame
	void Update () {
		// if( Random.value < 0.001f && ( numOfInstances < maxNumOfInstances || maxNumOfInstances == 0 ) )

		if( ( Time.time - lastSpawnTime > spawnDelay ) && ( numOfInstances < maxNumOfInstances || maxNumOfInstances == 0 ) )
		{
			// Reset timer
			lastSpawnTime = Time.time;

			// Calculate position
			Vector3 spawnPosition;
			float distance = area ? Random.value * range : 0;
			spawnPosition = transform.position + Quaternion.Euler(0, Random.value * 360, 0) * Vector3.forward * distance;

			// Select prefab
			float r = Random.value;
			Item prefab = prefabList[0];

			for( int i = 1; i < prefabRare.Count; i++ ){
				float rare = prefabRare[i];
				if( r < rare ){
					prefab = prefabList[i];
					break;
				}
			}

			// Spawn prefab
			// Item prefab = prefabList[ (int) Random.Range(0, prefabList.Count) ];
			Item instance = Instantiate( prefab, spawnPosition, Quaternion.identity ) as Item;
			instance.spawner = this;
			numOfInstances++;
		}
	}

	// Draw spawner area range
	void OnDrawGizmos()
	{
		if( area ){
	        Gizmos.color = Color.red;
	        Gizmos.DrawWireSphere( transform.position, range );
		}
		else{
	        Gizmos.color = Color.red;
	        Gizmos.DrawWireCube( transform.position, Vector3.one );
		}
    }
}
