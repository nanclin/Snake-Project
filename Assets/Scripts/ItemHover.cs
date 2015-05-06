using UnityEngine;
using System.Collections;

public class ItemHover : MonoBehaviour {

	private Vector3 pos;
	private Vector3 rot;

	// Use this for initialization
	void Start () {
		pos = transform.position;
		rot = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		pos.y += Mathf.Sin( Time.time * 3 ) / 30;
		rot.y += 3;
		transform.position = pos;
		transform.eulerAngles = rot;
	}
}
