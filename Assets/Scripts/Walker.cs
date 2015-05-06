using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour {

	public BezierSpline spline;
	
	private float t = 0;
	[Range(0f,100f)]
	public float tRange;

	void OnEnable()
	{
		
	}

	// Update is called once per frame
	void Update () {
		t += 0.01f;
		if( t > spline.length )
			t = 0;

		TransformData data = spline.GetPointBaked( t );
		transform.position = data.position;
		// transform.rotation = data.rotation;
	}

	// void OnGUI()
	// void OnRenderObject()
	void OnDrawGizmos()
	{
		// print("anclin");
		TransformData data = spline.GetPointBaked( 0.5f );
		transform.position = data.position;
	}
}
