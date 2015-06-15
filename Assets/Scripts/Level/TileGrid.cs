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

	private Transform tile_square;
	private Transform tile_edge;
	private Transform tile_peak_1;
	private Transform tile_peak_2;
	private Transform tile_foothills_1;
	private Transform tile_foothills_2;
	private Transform tile_diagonal;
	private Transform tile_corner_small;
	private Transform tile_corner_medium;
	private Transform tile_corner_large;
	private Transform tile_knee_small;
	private Transform tile_knee_medium;
	private Transform tile_knee_large;

	void Awake()
	{
		// Get prefab refenrence to instantiate from
		tile_square = transform.Find("tile_square");
		tile_edge = transform.Find("tile_edge");
		tile_peak_1 = transform.Find("tile_peak_1");
		tile_peak_2 = transform.Find("tile_peak_2");
		tile_foothills_1 = transform.Find("tile_foothills_1");
		tile_foothills_2 = transform.Find("tile_foothills_2");
		tile_diagonal = transform.Find("tile_diagonal");
		tile_corner_small = transform.Find("tile_corner_small");
		tile_corner_medium = transform.Find("tile_corner_medium");
		tile_corner_large = transform.Find("tile_corner_large");
		tile_knee_small = transform.Find("tile_knee_small");
		tile_knee_medium = transform.Find("tile_knee_medium");
		tile_knee_large = transform.Find("tile_knee_large");

		// Load level data
		// List<int[][]> level_01_map = LoadLevelMap("Assets/Levels/Level 01/level 01.tmx");
		List<int[][]> level_01_map = LoadLevelMap("Assets/Levels/Level Pillars/level pillars.tmx");

		// GenerateLevelLayer( level_01_map[0], 0 );
		// GenerateLevelLayer( level_01_map[1], 1 );
		// GenerateLevelLayer( map, 0 );
		// GenerateLevelLayer( map2, 1 );

		// Instantiate tiles for the level
		GenerateLevel( level_01_map );
	}

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
						tile = Instantiate( tile_square, pos, Quaternion.identity ) as Transform;
						break;
				//////////////////////////////////////////////////////////// EO TILE SQUARE //
						
				// TILE EDGE ///////////////////////////////////////////////////////////////
					case 101:
						tile = Instantiate( tile_edge, pos, Quaternion.identity ) as Transform;
						break;
					case 102:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_edge, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 114:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_edge, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 113:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_edge, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE EDGE //

				// TILE PEAK 1 ///////////////////////////////////////////////////////////////
					case 125:
						tile = Instantiate( tile_peak_1, pos, Quaternion.identity ) as Transform;
						break;
					case 126:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_peak_1, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 138:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_peak_1, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 137:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_peak_1, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE PEAK 1 //

				// TILE PEAK 2 ///////////////////////////////////////////////////////////////
					case 128:
						tile = Instantiate( tile_peak_2, pos, Quaternion.identity ) as Transform;
						break;
					case 140:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_peak_2, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 139:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_peak_2, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 127:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_peak_2, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE PEAK 2 //

				// TILE FOOTHILS 1 ///////////////////////////////////////////////////////////////
					case 134:
						tile = Instantiate( tile_foothills_1, pos, Quaternion.identity ) as Transform;
						break;
					case 133:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_foothills_1, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 121:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_foothills_1, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 122:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_foothills_1, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE FOOTHILS 1 //

				// TILE FOOTHILS 2 ///////////////////////////////////////////////////////////////
					case 135:
						tile = Instantiate( tile_foothills_2, pos, Quaternion.identity ) as Transform;
						break;
					case 123:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_foothills_2, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 124:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_foothills_2, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 136:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_foothills_2, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE FOOTHILS 2 //

				// TILE DIAGONAL ///////////////////////////////////////////////////////////////
					case 107:
						tile = Instantiate( tile_diagonal, pos, Quaternion.identity ) as Transform;
						break;
					case 108:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_diagonal, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 120:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_diagonal, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 119:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_diagonal, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE DIAGONAL //

				// TILE CORNER SMALL ///////////////////////////////////////////////////////////////
					case 83:
						tile = Instantiate( tile_corner_small, pos, Quaternion.identity ) as Transform;
						break;
					case 84:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_corner_small, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 96:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_corner_small, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 95:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_corner_small, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE CORNER SMALL //

				// TILE CORNER MEDIUM ///////////////////////////////////////////////////////////////
					case 79:
						tile = Instantiate( tile_corner_medium, pos, Quaternion.identity ) as Transform;
						x += 1;
						break;
					case 81:
						pos += new Vector3(2,0,0) * tileSize;
						tile = Instantiate( tile_corner_medium, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						x += 1;
						break;
					case 105:
						pos += new Vector3(2,0,-2) * tileSize;
						tile = Instantiate( tile_corner_medium, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						x += 1;
						break;
					case 103:
						pos += new Vector3(0,0,-2) * tileSize;
						tile = Instantiate( tile_corner_medium, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						x += 1;
						break;
				//////////////////////////////////////////////////////////// EO TILE CORNER MEDIUM //

				// TILE CORNER LARGE ///////////////////////////////////////////////////////////////
					case 7:
						tile = Instantiate( tile_corner_large, pos, Quaternion.identity ) as Transform;
						x += 2;
						break;
					case 10:
						pos += new Vector3(3,0,0) * tileSize;
						tile = Instantiate( tile_corner_large, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						x += 2;
						break;
					case 46:
						pos += new Vector3(3,0,-3) * tileSize;
						tile = Instantiate( tile_corner_large, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						x += 2;
						break;
					case 43:
						pos += new Vector3(0,0,-3) * tileSize;
						tile = Instantiate( tile_corner_large, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						x += 2;
						break;
				//////////////////////////////////////////////////////////// EO TILE CORNER LARGE //

				// TILE KNEE SMALL ///////////////////////////////////////////////////////////////
					case 77:
						tile = Instantiate( tile_knee_small, pos, Quaternion.identity ) as Transform;
						break;
					case 78:
						pos += new Vector3(1,0,0) * tileSize;
						tile = Instantiate( tile_knee_small, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						break;
					case 90:
						pos += new Vector3(1,0,-1) * tileSize;
						tile = Instantiate( tile_knee_small, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						break;
					case 89:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_knee_small, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE KNEE SMALL //

				// TILE KNEE MEDIUM ///////////////////////////////////////////////////////////////
					case 73:
						tile = Instantiate( tile_knee_medium, pos, Quaternion.identity ) as Transform;
						x += 1;
						break;
					case 75:
						pos += new Vector3(2,0,0) * tileSize;
						tile = Instantiate( tile_knee_medium, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						x += 1;
						break;
					case 99:
						pos += new Vector3(2,0,-2) * tileSize;
						tile = Instantiate( tile_knee_medium, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						x += 1;
						break;
					case 97:
						pos += new Vector3(0,0,-2) * tileSize;
						tile = Instantiate( tile_knee_medium, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						x += 1;
						break;
				//////////////////////////////////////////////////////////// EO TILE KNEE MEDIUM //

				// TILE KNEE LARGE ///////////////////////////////////////////////////////////////
					case 1:
						tile = Instantiate( tile_knee_large, pos, Quaternion.identity ) as Transform;
						x += 2;
						break;
					case 4:
						pos += new Vector3(3,0,0) * tileSize;
						tile = Instantiate( tile_knee_large, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 90;
						x += 2;
						break;
					case 40:
						pos += new Vector3(3,0,-3) * tileSize;
						tile = Instantiate( tile_knee_large, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 180;
						x += 2;
						break;
					case 37:
						pos += new Vector3(0,0,-3) * tileSize;
						tile = Instantiate( tile_knee_large, pos, Quaternion.identity ) as Transform;
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