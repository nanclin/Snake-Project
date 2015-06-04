using UnityEngine;
using System.Collections;

public class SnakeBodyCell : MonoBehaviour {

	private static int TOTAL = 0;
	private string id;
	private string[] alphabeth = new string[] {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};

	// Settings
	public Color color;
	public float buffer = 0.55f;
	public float bondStrength = 1f;

	// System
	private bool _isTail;
	private bool _isHead;
	[HideInInspector] public SnakeBody2 body;
	[HideInInspector] public float relPos = 0;

	public ChainNode node;	// REMOVE????

// INIT ///////////////////////////////////////////////////////////////
	public void Init( bool isTail, SnakeBody2 body )
	{
	}
//////////////////////////////////////////////////////////// EO INIT //

// UNITY METHODS ///////////////////////////////////////////////////////////////

	void Awake()
	{
		id = alphabeth[ TOTAL++ % alphabeth.Length ];
	}

	void OnDrawGizmos()
	{
		if( isHead ){
			MyDraw.DrawCircle( transform.position, 0.3f, Color.blue );
		}
		if( isTail ){
			MyDraw.DrawCircle( transform.position, 0.3f, Color.red );
		}
	}

	void OnGUI()
	{
		Vector3 point = Camera.main.WorldToScreenPoint( transform.position );
		GUI.Box( new Rect( point.x - 10, Screen.height - point.y - 10, 20, 20 ), id );
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// OTHER METHODS ///////////////////////////////////////////////////////////////

	public void Destroy()
	{
		// Manage tail/head

		// Destroy
		Destroy( gameObject );
	}

	public bool isHead {
		get{ return _isHead; }
		set{
			_isHead = value;

			if( isHead )
			{
			}
		}
	}

	public bool isTail {
		get{ return _isTail; }
		set{
			_isTail = value;

			if( isTail )
			{
			}
		}
	}
	public void SetColor( Color color )
	{
		GetComponent<Renderer>().material.color = color;
	}
//////////////////////////////////////////////////////////// EO OTHER METHODS //

// DEBUG ///////////////////////////////////////////////////////////////

	override public string ToString()
	{
		return id;
	}
//////////////////////////////////////////////////////////// EO DEBUG //

	// public SnakeBodyCell GetPrevious(){ return node.previous.prefab.gameObject.GetComponent<SnakeBodyCell>(); }
	// public SnakeBodyCell GetNext(){ return node.next.prefab.gameObject.GetComponent<SnakeBodyCell>(); }
}
