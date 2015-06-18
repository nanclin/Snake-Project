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
	private bool _isItemOn;
	[HideInInspector] public SnakeBody body;
	[HideInInspector] public float relPos = 0;

// INIT ///////////////////////////////////////////////////////////////
	public void Init( bool isTail, SnakeBody body )
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
		// if( isHead ){
		// 	MyDraw.DrawCircle( transform.position, 0.3f, Color.blue );
		// }
		// if( isTail ){
		// 	MyDraw.DrawCircle( transform.position, 0.2f, Color.yellow );
		// }
	}

	void OnGUI()
	{
		// // Show id next to the cell
		// Vector3 point = Camera.main.WorldToScreenPoint( transform.position + Vector3.right );
		// GUI.Box( new Rect( point.x - 10, Screen.height - point.y - 10, 20, 20 ), id );
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// OTHER METHODS ///////////////////////////////////////////////////////////////

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

// GETTERS / SETTERS ///////////////////////////////////////////////////////////////

	public bool isHead {
		get{ return _isHead; }
		set{ _isHead = value; }
	}

	public bool isTail {
		get{ return _isTail; }
		set{ _isTail = value; }
	}

	public bool isItemOn {
		get{ return _isItemOn; }
		set{
			_isItemOn = value;

			if( _isItemOn )
				transform.Find( "Item" ).gameObject.SetActive( true );
			else if( !_isItemOn )
				transform.Find( "Item" ).gameObject.SetActive( false );
		}
	}

	public SnakeBodyCell previous
	{
		get{
			int prevId = body.cellList.IndexOf( this ) - 1;
			return body.cellList[ prevId ];
		}
	}

	public SnakeBodyCell next
	{
		get{
			int nextId = body.cellList.IndexOf( this ) + 1;
			return nextId >= body.cellList.Count ? null : body.cellList[ nextId ];
		}
	}

	public float distanceToNext
	{
		get{
			return next.relPos - relPos;
		}
	}

	public float gapToNext
	{
		get{
			return Mathf.Abs( distanceToNext - buffer - next.buffer );
		}
	}
//////////////////////////////////////////////////////////// EO GETTERS / SETTERS //
}
