using UnityEngine;
using System.Collections;

public class TileGrid : MonoBehaviour
{
	private float tileSize = 4f;

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

	private int[][] map = 
	{
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,79,80,102,124,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,91,92,0,0,139,10,11,12,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,140,0,0,0,0,22,23,24,129,129,129,129,129,129},
		new int[]{129,129,129,129,79,80,124,129,129,129,107,0,73,74,75,76,34,35,36,129,129,129,129,129,129},
		new int[]{129,129,129,129,91,92,0,139,102,138,0,0,85,86,87,88,0,0,137,129,129,129,129,129,129},
		new int[]{129,129,129,129,103,104,0,125,113,75,76,0,97,98,99,100,0,0,0,108,129,129,129,129,129},
		new int[]{129,129,129,129,115,116,134,129,129,87,88,0,109,110,111,112,0,0,0,0,122,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,133,0,0,0,0,0,0,0,0,105,106,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,119,0,0,0,125,113,113,113,117,118,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,135,113,134,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129},
		new int[]{129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129,129}
	};

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

		GenerateMap( map );
	}

	private void GenerateMap( int[][] map )
	{	
		for( int y = 0; y < map.Length; y++ )
		{
			for( int x = 0; x < map[y].Length; x++ )
			{	
				// Get tile id
				int t = map[y][x];

				// Set current tile position vector
				Vector3 pos = new Vector3( x * tileSize, 0, -y * tileSize );

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
					case 113:
						pos += new Vector3(0,0,-1) * tileSize;
						tile = Instantiate( tile_edge, pos, Quaternion.identity ) as Transform;
						tile.eulerAngles = Vector3.up * 270;
						break;
					case 114:
						// pos += new Vector3(0,0,-1) * tileSize;
						// tile = Instantiate( tile_edge, pos, Quaternion.identity ) as Transform;
						// tile.eulerAngles = Vector3.up * 270;
						break;
				//////////////////////////////////////////////////////////// EO TILE EDGE //

				// TILE PEAK 1 ///////////////////////////////////////////////////////////////
					case 125:
						tile = Instantiate( tile_peak_1, pos, Quaternion.identity ) as Transform;
						break;
					case 88:
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
					case 41:
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