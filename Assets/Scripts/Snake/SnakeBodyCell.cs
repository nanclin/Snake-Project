using UnityEngine;
using System.Collections;

public class SnakeBodyCell : MonoBehaviour {

	public delegate void DestroyCell( ChainNode node );
    public event DestroyCell OnCellDestroy;

	public Color color;
	private bool _isTail;
	[HideInInspector] public SnakeBody body;
	[HideInInspector] public ChainNode node;

	public void Init( bool isTail, SnakeBody body, ChainNode node )
	{
		this.node = node;
	}

	// void Update()
	// {
	// 	float dis = Vector3.Distance( body.head.position, transform.position );
	// 	// print( "body.head.position: " + body.head.position );
	// 	print( "dis: " + dis );
	// }

	void OnDrawGizmos()
	{
		if( isTail ){
			MyDraw.DrawCircle( transform.position, 0.3f, Color.blue );
		}
	}

	void Awake()
	{
	}

	public void Destroy()
	{
		// Manage tail reference
		SnakeBodyCell previousCell = node.previous.prefab.GetComponent<SnakeBodyCell>();
		if( isTail && previousCell != null )
		{
			previousCell.isTail = true;
			// Color tail
			previousCell.gameObject.GetComponent<Renderer>().material.color = Color.blue;
		}

		// Destroy
		OnCellDestroy( node );
		Destroy( gameObject );
	}

	// public void SetTail( bool value )
	// {
	// 	if( value ){
			
	// 	}
	// 	else{
	// 		isTail = false;
	// 	}
	// }
	public bool isTail {
		get{
			return _isTail;
		}
		set{
			_isTail = value;

			if( isTail )
			{
				// SnakeBodyCell previousCell = node.previous.GetCellScript();
				// if( previousCell != null ){
				// 	previousCell.gameObject.GetComponent<Renderer>().material.color = Color.white;
				// 	gameObject.GetComponent<Renderer>().material.color = Color.blue;
				// }

			}

			else if( !isTail )
			{
				gameObject.GetComponent<Renderer>().material.color = Color.white;
			}
		}
	}
	public void SetColor( Color color )
	{
		GetComponent<Renderer>().material.color = color;
	}

	// public SnakeBodyCell GetPrevious(){ return node.previous.prefab.gameObject.GetComponent<SnakeBodyCell>(); }
	// public SnakeBodyCell GetNext(){ return node.next.prefab.gameObject.GetComponent<SnakeBodyCell>(); }
}
