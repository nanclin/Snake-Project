using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CameraSelector : MonoBehaviour {

	public List<Transform> cameras = new List<Transform>();
	public Transform currentCamera;

	// Use this for initialization
	void Start () {
		currentCamera.gameObject.SetActive( true );

		// foreach( Transform camera in cameras )
			// camera.gameObject.SetActive( false );
		// 	cameras.Add( camera );

	}

	private int c = 0;

	void Update()
	{
		if( Input.GetKeyDown("c") && cameras.Count > 0 ){
			c = ( c + 1 ) % cameras.Count;
			print( "c: " + c );
			SwitchCamera(c);
		}
	}

	void OnGUI()
	{
		int i = 0;
		float x = 10;
		float y = 10;
		// foreach( Transform camera in transform )
		foreach( Transform camera in cameras )
		{
			string cameraName = camera.ToString().Split('-')[1].Split('(')[0];
			float width = (float)cameraName.Length * 10;
			if( GUI.Toggle( new Rect( 10, y, width, 15 ), ( currentCamera == camera ), cameraName ) )
			{
				SwitchCamera(i);
				c = i;
			}
			x += width + 10;
			y += 15 + 10;
			i++;
		}
	}

	private void SwitchCamera( int i )
	{
		currentCamera.gameObject.SetActive( false );
		currentCamera = cameras[i];
		currentCamera.gameObject.SetActive( true );
	}
}
