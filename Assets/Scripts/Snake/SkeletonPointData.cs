using UnityEngine;
using System;
using System.Collections;

public struct SkeletonPointData {

	public Vector3 position;
	public Quaternion rotation;

	override public string ToString ()
	{
		string s = "";
		
		s += " position:\t"         + position        + "\n";
		s += " rotation:\t"         + rotation        + "\n";
		
		return s;
	}
}