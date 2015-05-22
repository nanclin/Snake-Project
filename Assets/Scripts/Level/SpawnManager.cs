using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : Initializer
{

	public int stock;
	public Item itemPrefab;
	protected int currentStock;
	private List<Item> itemList = new List<Item>();

	override public void Init()
	{
		// Reset stock
		currentStock = stock;

		// Clean spawned items
		foreach( Item item in itemList ){
			Destroy( item.gameObject );
		}
		itemList = new List<Item>();
	}

	// Spawn item
	public Item SpawnItem( Vector3 spawnPosition, Item itemPrefab )
	{
		if( currentStock > 0 )
		{
			// Take out from stock
			currentStock--;
			
			// Spawn prefab
			Item item = Instantiate( itemPrefab, spawnPosition, Quaternion.identity ) as Item;
			item.spawnManager = this;

			// Add item to the list (for later cleanup)
			itemList.Add( item );

			return item;
		}
		return null;
	}

	// Item is destroyed by Spawner to cleanup references
	public void DestroyItem( Item item )
	{
		// Destroy item
		itemList.Remove( item );
		Destroy( item.gameObject );
	}

}
