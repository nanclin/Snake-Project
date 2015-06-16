using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolSpawnTest : MonoBehaviour {

	private int i = 0;

	private List<GameObject> onScene = new List<GameObject>();
	private List<GameObject> pool = new List<GameObject>();
	private List<GameObject> list = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void Update ()
	{

		if( Input.GetKeyDown("space") )
		{
			GameObject go = ObjectPool.instance.GetObjectForType( "prefab_bullet", false );
			go.transform.position = Vector3.forward * i++;
			list.Insert( 0, go );
		}
		if( Input.GetKeyDown("backspace") )
		{
			GameObject go = list[0];
			ObjectPool.instance.PoolObject( go );
			list.Remove( go );
		}
	}
}
