using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	// public GameObject target;
	// public float smoothTime = 0.1f;

	private Transform target;

	// Use this for initialization
	void Start () {
		target = GetComponentInChildren<Chain>().head.prefab.transform;
		// print( GetComponent<Chain>().head.prefab.transform );
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate () {

		// Vector3 velocity = Vector3.zero;
		// transform.position = Vector3.SmoothDamp(this.transform.position, target.transform.position, ref velocity, smoothTime);
		
		transform.position = target.position;
	}
}
