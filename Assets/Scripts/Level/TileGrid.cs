using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

public class TileGrid : MonoBehaviour
{
	private float tileSize = 4f;
	private float tileHeight = 2f;
	private List<Transform> tiles = new List<Transform>();
	public string levelMapPath;

	void Start()
	{
		// Load level data
		// List<int[][]> level_01_map = LoadLevelMap("Assets/Levels/Level 01/level 01.tmx");
		// List<int[][]> level_map = LoadLevelMap("Assets/Levels/Level Pillars/level pillars.tmx");
		List<int[][]> level_map = LoadLevelMap( levelMapPath );

		// GenerateLevelLayer( level_map[0], 0 );
		// GenerateLevelLayer( level_map[1], 1 );

		// Instantiate tiles for the level
		GenerateLevel( level_map );

	}

	// int p = 0;
	// public string[] paths;

	// void Update()
	// {
	// 	if( Input.GetKeyDown("space") )
	// 	{
	// 		string path = paths[ p++ % paths.Length ];
	// 		List<int[][]> level_map = LoadLevelMap( path );
	// 		GenerateLevel( level_map );
	// 	}

	// }

	private List<int[][]> LoadLevelMap( string path )
	{
		// Load xml document of the level
		XmlDocument doc = new XmlDocument();
		doc.Load( path );
		
		// Get number of tiles per row and column
		int width = System.Int32.Parse( doc.DocumentElement.Attributes["width"].Value );
		int height = System.Int32.Parse( doc.DocumentElement.Attributes["height"].Value );

		// Get layer nodes
		XmlNodeList layerNodes = doc.SelectNodes("/map/layer");

		// Create list of layer maps
		List<int[][]> layerMapList = new List<int[][]>();

		// Loop trough layer nodes
		for( int i = 0; i < layerNodes.Count; i++ )
		{
			// TILE VALUES LIST ///////////////////////////////////////////////////////////////
			
			// Get layer node
			XmlNode layer = layerNodes[i];

			// Example of accessing layer attributes
			string layerName = layer.Attributes["name"].Value;
			print( "layerName: " + layerName );

			// Sample only visible layers
			if( layer.Attributes["visible"] == null ){

				// READING LAYER PROPERTIES ///////////////////////////////////////////////////////////////
				if( layer.SelectSingleNode(".//properties") != null )
				{
					// Select all property nodes
					XmlNodeList properties = layer.SelectSingleNode(".//properties").ChildNodes;
					
					for( int j = 0; j < properties.Count; j++ )
					{
						print( "property " + j + ": " + properties[j].Attributes["name"].Value );
					}
				}
				//////////////////////////////////////////////////////////// EO READING LAYER PROPERTIES //

				// Get layer data node
				XmlNodeList layerTileNodes = layer.SelectSingleNode(".//data").ChildNodes;

				// List of all tile values
				int[] tiles = new int[ layerTileNodes.Count ];

				// Convert and store all tile values to the list
				for( int j = 0; j < layerTileNodes.Count; j++ )
				{
					XmlNode tile = layerTileNodes[j];
					tiles[j] = System.Int32.Parse( tile.Attributes["gid"].Value );
				}
				//////////////////////////////////////////////////////////// EO TILE VALUES LIST //

				// GENERATE MAP ///////////////////////////////////////////////////////////////

				int[][] layerMap = new int[height][];
				for( int y = 0; y < height; y++ ){
					layerMap[y] = new int[width];
					for( int x = 0; x < height; x++ ){
						layerMap[y][x] = tiles[ x + y * width ];
					}
				}

				// Add current layer map to the list of all layer maps of current level
				layerMapList.Add( layerMap );

				// // Debug
				// for( int y = 0; y < height; y++ ){
				// 	for( int x = 0; x < height; x++ ){
				// 		print("layerMap["+y+"]["+x+"]: " + layerMap[y][x] );
				// 	}
				// }

				//////////////////////////////////////////////////////////// EO GENERATE MAP //			
			}
		}
		return layerMapList;
	}

	private void GenerateLevel( List<int[][]> layerList )
	{
		// RESET PREVIOUS TILE GRID ///////////////////////////////////////////////////////////////
		
		// Pool tiles
		foreach( Transform tile in tiles ){
			ObjectPool.instance.PoolObject( tile.gameObject );
		}

		// Empty tiles list
		tiles = new List<Transform>();
		//////////////////////////////////////////////////////////// EO RESET PREVIOUS TILE GRID //

		// Generate layers of current map
		for( int i = 0; i < layerList.Count; i++ )
			GenerateLevelLayer( layerList[i], i );
	}

	private void GenerateLevelLayer( int[][] map, int level = 0 )
	{	
		for( int y = 0; y < map.Length; y++ )
		{
			for( int x = 0; x < map[y].Length; x++ )
			{	
				// Get tile id
				int t = map[y][x];
				// print( x + ", " + y + ": " + t );

				// Set current tile position vector
				Vector3 pos = new Vector3( x * tileSize, level * tileHeight, -y * tileSize );

				// Tile transform reference
				Transform tile = null;

				// Instantiate appropriate tile model
				switch( t )
				{
				// TILE SQUARE ///////////////////////////////////////////////////////////////
					case 129:
						tile = ObjectPool.instance.GetObjectForType( "tile_square", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
				//////////////////////////////////////////////////////////// EO TILE SQUARE //
						
				// TILE EDGE ///////////////////////////////////////////////////////////////
					case 101:
						tile = ObjectPool.instance.GetObjectForType( "tile_edge", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 102:
						tile = ObjectPool.instance.GetObjectForType( "tile_edge", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 114:
						tile = ObjectPool.instance.GetObjectForType( "tile_edge", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 113:
						tile = ObjectPool.instance.GetObjectForType( "tile_edge", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE EDGE //

				// TILE PEAK 1 ///////////////////////////////////////////////////////////////
					case 125:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 126:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 138:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 137:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE PEAK 1 //

				// TILE PEAK 2 ///////////////////////////////////////////////////////////////
					case 128:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 140:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 139:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 127:
						tile = ObjectPool.instance.GetObjectForType( "tile_peak_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE PEAK 2 //

				// TILE FOOTHILS 1 ///////////////////////////////////////////////////////////////
					case 134:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 133:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 121:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 122:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_1", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE FOOTHILS 1 //

				// TILE FOOTHILS 2 ///////////////////////////////////////////////////////////////
					case 135:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 123:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 124:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 136:
						tile = ObjectPool.instance.GetObjectForType( "tile_foothills_2", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE FOOTHILS 2 //

				// TILE DIAGONAL ///////////////////////////////////////////////////////////////
					case 107:
						tile = ObjectPool.instance.GetObjectForType( "tile_diagonal", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 108:
						tile = ObjectPool.instance.GetObjectForType( "tile_diagonal", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 120:
						tile = ObjectPool.instance.GetObjectForType( "tile_diagonal", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 119:
						tile = ObjectPool.instance.GetObjectForType( "tile_diagonal", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE DIAGONAL //

				// TILE CORNER SMALL ///////////////////////////////////////////////////////////////
					case 83:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 84:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 96:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 95:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE CORNER SMALL //

				// TILE CORNER MEDIUM ///////////////////////////////////////////////////////////////
					case 79:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						x += 1;
						tile.eulerAngles = Vector3.zero;
						break;
					case 81:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(2,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						x += 1;
						break;
					case 105:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(2,0,-2) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						x += 1;
						break;
					case 103:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-2) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						x += 1;
						break;
				//////////////////////////////////////////////////////////// EO TILE CORNER MEDIUM //

				// TILE CORNER LARGE ///////////////////////////////////////////////////////////////
					case 7:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						x += 2;
						tile.eulerAngles = Vector3.zero;
						break;
					case 10:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(3,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						x += 2;
						break;
					case 46:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(3,0,-3) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						x += 2;
						break;
					case 43:
						tile = ObjectPool.instance.GetObjectForType( "tile_corner_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-3) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						x += 2;
						break;
				//////////////////////////////////////////////////////////// EO TILE CORNER LARGE //

				// TILE KNEE SMALL ///////////////////////////////////////////////////////////////
					case 77:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						tile.eulerAngles = Vector3.zero;
						break;
					case 78:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 90:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(1,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 89:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_small", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-1) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE KNEE SMALL //

				// TILE KNEE MEDIUM ///////////////////////////////////////////////////////////////
					case 73:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						x += 1;
						tile.eulerAngles = Vector3.zero;
						break;
					case 75:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(2,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						x += 1;
						break;
					case 99:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(2,0,-2) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						x += 1;
						break;
					case 97:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_medium", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-2) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						x += 1;
						break;
				//////////////////////////////////////////////////////////// EO TILE KNEE MEDIUM //

				// TILE KNEE LARGE ///////////////////////////////////////////////////////////////
					case 1:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos;
						x += 2;
						tile.eulerAngles = Vector3.zero;
						break;
					case 4:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(3,0,0) * tileSize;
						tile.eulerAngles = Vector3.up * 90;
						x += 2;
						break;
					case 40:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(3,0,-3) * tileSize;
						tile.eulerAngles = Vector3.up * 180;
						x += 2;
						break;
					case 37:
						tile = ObjectPool.instance.GetObjectForType( "tile_knee_large", false ).transform;
						tiles.Add( tile );
						tile.position = pos + new Vector3(0,0,-3) * tileSize;
						tile.eulerAngles = Vector3.up * 270;
						x += 2;
						break;
				//////////////////////////////////////////////////////////// EO TILE KNEE LARGE //
				}

				// Turn on object visibility
				if( tile != null )
					tile.gameObject.SetActive( true );
			}
		}
	}
}