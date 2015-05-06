using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {

	public Transform target;
	// public Transform ground;
	public Transform path;
	public bool rotate = true;

	private BezierSpline spline;

	void Start()
	{
		// spline = path.GetComponent<BezierSpline>();
	}

	
	// Update is called once per frame
	void Update ()
	{
	}


	void OnTriggerEnter( Collider other )
	{
		// print("trigger enter");
		// Color color = ground.GetComponent<Renderer>().material.color;
		// ground.GetComponent<Renderer>().material.color = new Color( color.r, color.g, color.b, 0.5f );
	}

	void OnTriggerExit( Collider other )
	{
		// print("trigger exit");
		// GetComponent<CapsuleCollider>().enabled = true;
		// ground.GetComponent<Renderer>().material.color = Color.white;
	}
}
