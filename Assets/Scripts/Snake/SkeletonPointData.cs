using UnityEngine;
using System;
using System.Collections;

public struct SkeletonPointData {

	public Vector3    point;
	public Vector3    prevJoint;
	public Vector3    nextJoint;
	public int        pointID;
	public int        prevJointID;
	public int        nextJointID;
	public Quaternion angle;
	public bool       hasPrevious;
	public bool       hasNext;
	public bool       isOnJoint;

	override public string ToString ()
	{
		string s = "";
		s += " point:\t"         + pointID + " " + point        + "\n";
		s += " prevJoint:\t"     + prevJointID + ": ("+prevJoint.x+", "+prevJoint.y+", "+prevJoint.z+")\n";
		s += " nextJoint:\t"     + nextJointID + ": ("+nextJoint.x+", "+nextJoint.y+", "+nextJoint.z+")\n";
		// s += " pointID:\t"   	 + pointID      + "\n";
		// s += " prevJointID:\t"   + prevJointID  + "\n";
		// s += " nexJointID:\t"    + nextJointID  + "\n";
		s += " angle:\t"         + angle        + "\n";
		s += " hasPrev:\t"       + hasPrevious  + "\n";
		s += " hasNext:\t"    	 + hasNext      + "\n";
		s += " isOnJoint:\t"     + isOnJoint;
		return s;

	}
}