using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

	// Global
	public static int ROUND_DECIMALS = 5;
	
	// Handling
	public float maxSpeed = 5f;
	public float acceleration = 0.1f;
	public float rotationSpeed = 60;

	// System
	SnakeSkeleton skeleton = new SnakeSkeleton();
	private float dt;
	private float angle = 0;
	private float speed = 0;
	private Vector3 direction = Vector3.forward;
	private Vector3 currentPosition = Vector3.zero;


	// Use this for initialization
	void Start () {
		skeleton.AppendJoint( currentPosition );
	}
	
	// Update is called once per frame
	void Update () {

		dt = Time.deltaTime;

		skeleton.Draw();

		// Update speed
		// (accelareate, but not above maxSpeed)
		speed += acceleration * dt;
		speed = Mathf.Min( speed, maxSpeed * dt );

		// Update position
		direction = Quaternion.Euler( 0, Input.GetAxis( "Horizontal" ) * rotationSpeed * dt, 0 ) * direction;		// Rotate direction vector
		currentPosition += direction * speed;
		
		// Update skeleton
		skeleton.AppendJoint( currentPosition );
	}
}
