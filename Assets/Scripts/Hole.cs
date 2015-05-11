using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {

	public HoleType type = HoleType.Hole;
	public Transform path;

	private BezierSpline spline;

	void Start()
	{
		spline = path.GetComponent<BezierSpline>();
	}

	public void RotatePeriscope( Transform target )
	{
		var lookPos = -( target.position - transform.position );
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation( lookPos );

		spline.transform.rotation = rotation;
		spline.BakeSpline();
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

public enum HoleType{
	Hole,
	Exit
}
