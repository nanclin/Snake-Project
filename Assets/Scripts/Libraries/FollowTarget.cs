using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	public Transform target;
	public float smoothTime = 0.1f;

	void FixedUpdate () {

		Vector3 velocity = Vector3.zero;
		transform.position = Vector3.SmoothDamp( transform.position, target.position, ref velocity, smoothTime );
		
		// transform.position = target.position;
	}
}
