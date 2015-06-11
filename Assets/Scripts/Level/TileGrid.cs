using UnityEngine;
using System.Collections;

public class TileGrid : MonoBehaviour
{

	public Transform tile_1x1;
	public Transform tile_2x2;
	public Transform tile_2x1;

	private int mapHeight = 10;
	private int mapWidth = 10;
	public Tile[][] map;

	void Start ()
	{		
		map = new Tile[2][];
		map[0] = new Tile[2];
		map[1] = new Tile[2];

		map[0][0] = new Tile( 0, 2, 2 );
		map[0][1] = new Tile( 0, 1, 1 );
		map[1][0] = new Tile( 0, 2, 1 );
		map[1][1] = new Tile( 0, 2, 1 );

		for( int y = 0; y < map.Length; y++ )
		{
			int yOff = y;
			int xOff = 0;
			for( int x = 0; x < map[y].Length; x++ )
			{	
				// Check if tile above block current tile position
				if( y >= 1 ){
					Tile tileAbove = map[y-1][x];
					if( tileAbove.height > 1 )
						xOff += tileAbove.width;
				}

				// Set current tile position vector
				Vector3 pos = new Vector3( xOff, 0, -yOff );

				// Get tile reference
				Tile tile = map[y][x];

				// Instantiate tile graphics
				if( tile.width == 1 && tile.height == 1 ){
					Instantiate( tile_1x1, pos, Quaternion.identity );
				}
				else if( tile.width == 2 && tile.height == 2 ){
					Instantiate( tile_2x2, pos, Quaternion.identity );
				}
				else if( tile.width == 2 && tile.height == 1 ){
					Instantiate( tile_2x1, pos, Quaternion.identity );
				}

				// Move position pointer to the end of current tile
				xOff += tile.width;
			}
		}
			
		// Random map
		// for( int y = 0; y < mapHeight; y++ )
		// {
		// 	map[y] = new Tile[ mapWidth ];

		// 	for( int x = 0; x < mapWidth; x++ )
		// 	{
		// 		Tile tile = new Tile( Random.Range(0,3), Random.Range(1,3), Random.Range(1,3) );
		// 		map[y][x] = tile;
		// 		print( "map[y][x]: " + map[y][x] );
		// 	}
		// }
	}
}

public class Tile
{
	public int type;
	public int width;
	public int height;

	public Tile( int type, int width, int height )
	{
		this.type = type;
		this.width = width;
		this.height = height;
	}

	override public string ToString()
	{
		string trace = "";
		trace += "type: " + type + "\n";
		trace += "width: " + width + "\n";
		trace += "height: " + height + "\n";
		return trace;
	}
}

// public class TileData : MonoBehaviour{
//      public static int[,] tile;
//      int x, y;
 
//      // Use this for initialization
//      void Awake ()
//      {
//          int mapHeight = TileMap.SIZE_Z;
//          int mapWidth = TileMap.SIZE_X;;
//          float tileSize = TileMap.TILESIZE;
 
//          tile = new int[mapWidth, mapHeight];
 
//          for (int x = 0; x < mapWidth; x++) 
//          {
//              for (int y = 0; y < mapHeight; y++)
//              {
//                  tile[x, y] = Random.Range (0, 4);
//              }
//          }
//      }
 
//      public int GetTileAt(int x, int y) {
//          return tile[x,y];
//      }
//  }