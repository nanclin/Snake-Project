using UnityEngine;
using System;
using System.Collections;

public struct SkeletonPointData {

	public Vector3 point;
	public Quaternion angle;

	override public string ToString ()
	{
		string s = "";
		
		s += " point:\t"         + point        + "\n";
		s += " angle:\t"         + angle        + "\n";
		
		return s;
	}
}