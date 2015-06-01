using UnityEngine;
// using System;
using System.Collections;

public class RandomColor : MonoBehaviour {

	// string[] colors = new string[] {"c4cbb3", "a6b4a4", "c0d2e6", "839493", "514f61"};
	// string[] colors = new string[] {"c4cbb3", "a6b4a4", "2a2b2e", "2c4a4a", "545454"};

	// Green tints reflective
	// string[] colors = new string[] {"2c4a4a", "426a4d", "44ae7f", "46f2a9", "44ae7f", "426a4d"};

	// Green tints steps
	string[] colors = new string[] { "44ae7f", "6fb993", "93c7aa", "b6d7c3", "d8e9de" };

	// Basic colors 6
	// string[] colors = new string[] {"ff0000", "00ff00", "0000ff", "ff00ff", "ffff00", "00ffff"};
	private static int i = 0;

	// Use this for initialization
	void Start () {
		// Change color to random
		// GetComponent<SpriteRenderer>().color = new Vector4(Random.value, Random.value, Random.value, .5f);
		// renderer.material.color = new Vector4(Random.value/2, Random.value/2, Random.value/2, .5f);
		// renderer.material.color = HexToColor( "ff0000" );
		// renderer.material.color = ( HexToColor( colors[ i++ % colors.Length ] ) + HexToColor("f0f0f0") ) / 3;
	GetComponent<Renderer>().material.color = HexToColor( colors[ i++ % colors.Length ] );
	// renderer.material.color = Color.cyan;
		// renderer.material.color = Convert.ToInt32("FFFFFF", 16)
	}

	Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}

	public static Color GetRandomColor()
	{
		return new Vector4( Random.value, Random.value, Random.value, 1f );
	}
	
}
