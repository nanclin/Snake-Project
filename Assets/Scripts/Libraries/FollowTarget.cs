using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	// public GameObject target;
	// public float smoothTime = 0.1f;

	public Transform target;


	void LateUpdate () {

		// Vector3 velocity = Vector3.zero;
		// transform.position = Vector3.SmoothDamp(this.transform.position, target.transform.position, ref velocity, smoothTime);
		
		transform.position = target.position;
	}
}
